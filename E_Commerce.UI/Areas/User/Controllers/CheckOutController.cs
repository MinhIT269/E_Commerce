﻿using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.UI.Areas.User.Controllers
{
    [Area("User")]
    public class CheckOutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
