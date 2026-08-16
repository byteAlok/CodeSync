using Education.Models;
using Microsoft.AspNetCore.Mvc;

namespace Education.Controllers
{
    public class ServicesController (MyConnection _db): Controller
    {
        private readonly MyConnection db = _db;
        public IActionResult Services()
        {
            var Uid = HttpContext.Session.GetInt32("UserId");
            if (Uid != null)
            {
                var check = db.UserCourse.Where(x => x.UserId == Uid).Select(x => x.CourseId).ToArray();
                ViewBag.check = check;
            }

            var data = db.Course.ToList();
            return View(data);
        }
    }
}
