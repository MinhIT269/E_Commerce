﻿using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReviewController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
