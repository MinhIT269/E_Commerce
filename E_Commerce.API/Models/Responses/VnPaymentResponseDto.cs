﻿namespace E_Commerce.API.Models.Responses
{
    public class VnPaymentResponseDto
    {
        public string PaymentId { get; set; } = string.Empty;
        public bool Success { get; set; } 
        public string PaymentMethod { get; set; } = string.Empty;
        public string OrderDescription { get; set; } = string.Empty;
        public string TxnRef { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string VnPayResponseCode { get; set; } = string.Empty;

    }
}
