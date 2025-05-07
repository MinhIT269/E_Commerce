using E_Commerce.API.Helpers;
using E_Commerce.API.Models.Requests;
using E_Commerce.API.Models.Responses;
using E_Commerce.API.Services.IService;

namespace E_Commerce.API.Services.Service
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;
        public VnPayService(IConfiguration config)
        {
            _config = config;
        }
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestDto model)
        {
            var tick = DateTime.Now.Ticks.ToString();

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]!);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]!);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]!);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());

            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]!);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:locale"]!);


            vnpay.AddRequestData("vnp_OrderInfo", model.OrderId.ToString());
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnUrl"]!);
            vnpay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"]!, _config["VnPay:HashSecret"]!);
            return paymentUrl;
        }
        public VnPaymentResponseDto PaymentExeCute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach(var (key, value) in collections)
            {
               if(!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
               {
                    vnpay.AddResponseData(key, value.ToString());
               }
            }

            var vnp_TxnRef = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");

            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash!, _config["VnPay:HashSecret"]!);

            if (!checkSignature)
            {
                return new VnPaymentResponseDto
                {
                    Success = false,
                };
            }

            return new VnPaymentResponseDto
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                TxnRef = vnp_TxnRef.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash.ToString(),
                VnPayResponseCode = vnp_ResponseCode.ToString(),
            };
        }
    }
}
