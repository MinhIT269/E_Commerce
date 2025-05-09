﻿using AutoMapper;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Requests;
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
                if (promo != null)
                {
                    discount = promo.Percentage;
                }
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
        private string GeneratePaymentHtml(string message, string transactionId, decimal amount, string statusText, string statusClass)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Kết quả thanh toán</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            display: flex;
            align-items: center;
            justify-content: center;
            height: 100vh;
            margin: 0;
            background-color: #f0f2f5;
        }}
        .payment-result-container {{
            text-align: center;
            padding: 2rem;
            border-radius: 8px;
            max-width: 400px;
            background-color: #fff;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }}
        .payment-status.success {{
            color: #4CAF50;
        }}
        .payment-status.failed {{
            color: #E74C3C;
        }}
        .payment-details {{
            margin-top: 1rem;
            font-size: 1rem;
            color: #333;
        }}
        .return-button {{
            margin-top: 1.5rem;
            display: inline-block;
            padding: 0.6rem 1.2rem;
            font-size: 1rem;
            color: #fff;
            background-color: #3498DB;
            text-decoration: none;
            border-radius: 4px;
            transition: background-color 0.3s;
        }}
        .return-button:hover {{
            background-color: #2980B9;
        }}
    </style>
</head>
<body>

<div class='payment-result-container'>
    <h1 class='payment-status {statusClass}'>{message}</h1>
    <div class='payment-details'>
        <p><strong>Mã giao dịch:</strong> {transactionId}</p>
        <p><strong>Số tiền:</strong> {amount.ToString("C0", new System.Globalization.CultureInfo("vi-VN"))}</p>
        <p><strong>Trạng thái:</strong> {statusText}</p>
    </div>
    <a href='/' class='return-button'>Quay về trang chủ</a>
</div>

</body>
</html>";
        }
        public async Task<string> HandlePaymentCallbackAsync(IQueryCollection query)
        {
            var response = _vnPayService.PaymentExeCute(query);
            if (response == null || !Guid.TryParse(response.OrderDescription, out var orderId))
            {
                return GeneratePaymentHtml("Giao dịch thất bại", "Không xác định", 0, "Thất bại", "failed");
            }
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            var transaction = await _transactionRepository.GetTransactionByOrderIdAsync(orderId);

            if (order == null || transaction == null)
            {
                return GeneratePaymentHtml("Không tìm thấy đơn hàng hoặc giao dịch", orderId.ToString(), 0, "Thất bại", "failed");
            }

            if (response.VnPayResponseCode != "00")
            {
                order.Status = "cancelled";
                transaction.Status = "cancelled";
                await _orderRepository.SaveChangesAsync();

                return GeneratePaymentHtml("Giao dịch thất bại", transaction.TransactionId.ToString(), transaction.Amount, "Thất bại", "failed");
            }
            order.Status = "done";
            transaction.Status = "done";
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
            return GeneratePaymentHtml("Giao dịch thành công", transaction.TransactionId.ToString(), transaction.Amount, "Hoàn thành", "success");
        }
    }
}
