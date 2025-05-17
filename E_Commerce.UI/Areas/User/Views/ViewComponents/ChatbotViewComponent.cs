using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.UI.Areas.User.Views.ViewComponents
{
    public class ChatbotViewComponent : ViewComponent
    {
        private readonly IConfiguration _config;
        public ChatbotViewComponent(IConfiguration configuration)
        {
            _config = configuration;
        }
        public IViewComponentResult Invoke()
        {
            var apiUrl = _config["Chatbot:ApiUrl"];
            ViewBag.apiUrl_ChatBot = apiUrl;
            return View();
        }
    }
}
