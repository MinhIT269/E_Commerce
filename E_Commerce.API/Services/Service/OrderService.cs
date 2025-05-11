using AutoMapper;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;
using E_Commerce.API.Repositories.IRepository;
using E_Commerce.API.Services.IService;
using Microsoft.AspNetCore.Identity;
namespace E_Commerce.API.Services.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IVnPayService _vnPayService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        public OrderService(IOrderRepository orderRepository, IMapper mapper, IProductRepository productRepository, IVnPayService vnPayService, ITransactionRepository transaction, IPromotionRepository promotionRepository, ICartRepository cartRepository, IUserInfoRepository userInfoRepository, UserManager<User> userManager)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _productRepository = productRepository;
            _vnPayService = vnPayService;
            _transactionRepository = transaction;
            _userManager = userManager;
            _promotionRepository = promotionRepository;
            _userInfoRepository = userInfoRepository;
            _cartRepository = cartRepository;
            _userInfoRepository = userInfoRepository;
        }

        public async Task<string> CreateOrderAndPaymentAsync(OrderRequestDto request, HttpContext httpContext)
        {
            var orderId = Guid.NewGuid();
            decimal totalAmount = 0;
            var orderDetails = new List<OrderDetail>();

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Product with ID {item.ProductId} not found");

                if (item.Quantity > product.Quantity)
                    throw new Exception($"Sản phẩm '{product.Name}' chỉ còn {product.Quantity} trong kho, không đủ để đặt {item.Quantity}");

                var unitPrice = (product.PromotionPrice.HasValue && product.PromotionPrice.Value > 0) ? product.PromotionPrice.Value : product.Price;
                totalAmount += unitPrice * item.Quantity;

                orderDetails.Add(new OrderDetail
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice
                });
            }

            var userInfo = new UserInfo
            {
                UserId = request.UserInfo.UserId,
                FirstName = request.UserInfo.FirstName,
                LastName = request.UserInfo.LastName,
                Address = request.UserInfo.Address,
                Gender = request.UserInfo.Gender
            };

            await _userInfoRepository.UpdateAsync(request.UserInfo.UserId, userInfo);

            var user = await _userManager.FindByIdAsync(request.UserInfo.UserId);
            if (user != null)
            {
                user.PhoneNumber = request.UserInfo.PhoneNumber;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to update phone number.");
                }
            }
            Promotion? promo = null;
            decimal discount = 0;

            if (!string.IsNullOrWhiteSpace(request.PromotionCode))
            {
                promo = await _promotionRepository.GetByCodeAsync(request.PromotionCode);
                if (promo == null)
                    throw new Exception("Mã khuyến mãi không tồn tại.");

                if (promo.EndDate < DateTime.UtcNow)
                    throw new Exception("Mã khuyến mãi đã hết hạn.");

                if (promo.MaxUsage <= 0)
                    throw new Exception("Mã khuyến mãi đã được sử dụng hết.");

                discount = promo.Percentage;
            }

            decimal discountAmount = 0;
            if (discount > 0)
            {
                discountAmount = (totalAmount * discount) / 100;
                totalAmount -= discountAmount;
            }

            var order = new Order
            {
                OrderId = orderId,
                UserId = request.UserInfo.UserId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = "pending",
                OrderDetails = orderDetails,
                DiscountPercentage = discount,
                PromotionId = promo?.PromotionId,
            };
            await _orderRepository.CreateOrderAsync(order);

            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                OrderId = orderId,
                TransactionDate = DateTime.UtcNow,
                Amount = totalAmount,
                Status = "pending",
                TransactionDetails = $"Thanh toán đơn hàng {orderId}",
                PaymentMethodId = Guid.Parse("26965F00-7D93-4150-812B-0E97F9AF785C") // Vnpay
            };

            await _transactionRepository.CreateTransactionAsync(transaction);

            await _orderRepository.SaveChangesAsync();
            var fullName = $"{request.UserInfo.FirstName} {request.UserInfo.LastName}";
            var vnPayModel = new VnPaymentRequestDto
            {
                Amount = (int)totalAmount,
                CreatedDate = DateTime.UtcNow,
                Description = transaction.TransactionDetails,
                FullName = fullName,
                OrderId = orderId
            };
            var paymentUrl = _vnPayService.CreatePaymentUrl(httpContext, vnPayModel);
            return paymentUrl;
        }
        public async Task<PaymentResponseDto> HandlePaymentCallbackAsync(IQueryCollection query)
        {
            var response = _vnPayService.PaymentExeCute(query);
            if (response == null || !Guid.TryParse(response.OrderDescription, out var orderId))
            {
                return new PaymentResponseDto
                {
                    Message = "Giao dịch thất bại",
                    TransactionId = "Không xác định",
                    Amount = 0,
                    StatusText = "Thất bại",
                    StatusClass = "danger"
                };
            }
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            var transaction = await _transactionRepository.GetTransactionByOrderIdAsync(orderId);

            if (order == null || transaction == null)
            {
                return new PaymentResponseDto
                {
                    Message = "Không tìm thấy đơn hàng hoặc giao dịch",
                    TransactionId = orderId.ToString().ToUpper(),
                    Amount = 0,
                    StatusText = "Thất bại",
                    StatusClass = "danger"
                };
            }

            if (response.VnPayResponseCode != "00")
            {
                order.Status = "cancelled";
                transaction.Status = "cancelled";
                await _orderRepository.SaveChangesAsync();

                return new PaymentResponseDto
                {
                    Message = "Giao dịch thất bại",
                    TransactionId = transaction.TransactionId.ToString().Substring(transaction.TransactionId.ToString().Length - 8).ToUpper(),
                    Amount = transaction.Amount,
                    StatusText = "Thất bại",
                    StatusClass = "danger"
                };
            }
            order.Status = "done";
            transaction.Status = "done";

            if (!string.IsNullOrEmpty(order.Promotion?.Code))
            {
                var promo = await _promotionRepository.GetByCodeAsync(order.Promotion.Code);
                if (promo != null && promo.MaxUsage > 0)
                {
                    promo.MaxUsage--;
                    await _promotionRepository.UpdatePromotionAsync(promo);
                }
            }

            // Cập nhật số lượng tồn kho
            foreach (var item in order.OrderDetails!)
            {
                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                    await _productRepository.UpdateProductAsync(product);
                }
            }
            await _cartRepository.ClearCartAsync(order.UserId);
            await _orderRepository.SaveChangesAsync();
            return new PaymentResponseDto
            {
                Message = "Giao dịch thành công",
                TransactionId = transaction.TransactionId.ToString().Substring(transaction.TransactionId.ToString().Length - 8).ToUpper(),
                Amount = transaction.Amount,
                StatusText = "Hoàn thành",
                StatusClass = "success"
            };
        }
    }
}
