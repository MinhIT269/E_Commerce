﻿using System.ComponentModel.DataAnnotations;

namespace E_Commerce.API.Models.Requests
{
    public class RegisterRequestDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public string[] Roles { get; set; }

    }
}
