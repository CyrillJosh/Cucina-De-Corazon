using Cucina_De_Corazon.Context;
using Microsoft.AspNetCore.Mvc;

namespace Cucina_De_Corazon.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly MyDBContext _context;

        public FeedbackController(MyDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var feedbacks = _context.Feedbacks.ToList();
            return View(feedbacks);
        }


        public IActionResult SubmitFeedback(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return Json(new { success = false, message = "Feedback message cannot be empty." });
            }
            var feedback = new Models.Feedback
            {
                Message = message,
                SubmittedAt = DateTime.Now
            };
            return Json(new { success = true, message = "Thank you for your feedback!" });
        }
    }
}
