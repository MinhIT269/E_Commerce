﻿namespace E_Commerce.API.Models.Responses
{
    public class CategoryResponseDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
