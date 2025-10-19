using Cucina_De_Corazon.Context;
using Cucina_De_Corazon.Models;
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

        [HttpPost]
        public IActionResult SubmitFeedback([FromBody] FeedbackRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
                return Json(new { success = false, message = "Feedback message cannot be empty." });

            var feedback = new Feedback
            {
                Message = request.Message,
                SubmittedAt = DateTime.Now
            };

            _context.Feedbacks.Add(feedback);
            _context.SaveChanges();

            return Json(new { success = true, message = "Thank you for your feedback!" });
        }

        public class FeedbackRequest
        {
            public string Message { get; set; }
        }
    }
}
