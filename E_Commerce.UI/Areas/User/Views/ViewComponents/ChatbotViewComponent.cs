using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.UI.Areas.User.Views.ViewComponents
{
    public class ChatbotViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
