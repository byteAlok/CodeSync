using Education.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace Education.Controllers
{
    [Authorize]
    public class UserController(MyConnection abc) : Controller
    {
        public readonly MyConnection db = abc;

        [HttpGet]
        public IActionResult Dashboard()
        {
            var Uid = HttpContext.Session.GetInt32("UserId");
            if (Uid == null) return RedirectToAction("Login", "Authentication");

            var userDetails = db.User.Where(x => x.UserId == Uid).FirstOrDefault();
            int courseCount = db.Course.Count();
            int userCourseCount = db.UserCourse.Where(x => x.UserId == Uid).Count();
            int contactCount = db.Contact.Where(x => x.UserId == Uid).Count();
            int feedbackCount = db.Feedback.Where(x => x.UserId == Uid).Count();

            int freeCourse = db.Course.Where(x => x.CoursePrice == 0).Count();

            ViewBag.UserFullName = userDetails.UserFullName;
            ViewBag.UserName = userDetails.UserName;
            ViewBag.UserImage = userDetails.UserImage ?? "";
            ViewBag.CourseCount = courseCount;
            ViewBag.UserCourseCount = userCourseCount + freeCourse;
            ViewBag.ContactCount = contactCount;
            ViewBag.FeedbackCount = feedbackCount;

            return View();
        }
        [HttpGet]
        public IActionResult Courses()
        {
            var Uid = HttpContext.Session.GetInt32("UserId");
            if (Uid == null) return RedirectToAction("Login", "Authentication");

            List <CourseClass> course = new List<CourseClass>();

            var userCourseId = db.UserCourse.Where(x => x.UserId == Uid).Select(x => x.CourseId).ToList();
            var freeCourse = db.Course.Where(x =>  x.CoursePrice == 0).ToList();

            if (userCourseId != null && userCourseId.Count > 0)
            {
                foreach (var item in userCourseId)
                {
                    var list = db.Course.FirstOrDefault(x => x.CourseId == item);

                    if (list != null) course.Add(list);
                }
            }
            if (freeCourse != null)
            {
                foreach (var i in freeCourse)
                {
                    course.Add(i);
                }
            }
             
            
            return View(course);
        
        }
        [HttpGet]
        public IActionResult AvailableCourses()
        {
            var Uid = HttpContext.Session.GetInt32("UserId");
            if (Uid == null) return RedirectToAction("Login", "Authentication");

            var courseList = db.Course.ToList();

            var check = db.UserCourse.Where(x => x.UserId == Uid).Select(x => x.CourseId).ToArray();
           
            ViewBag.check = check;

            return View(courseList);
        }
        public IActionResult Feedback()
        {
            var Uid = HttpContext.Session.GetInt32("UserId");
            if (Uid == null) return RedirectToAction("Login", "Authentication");

            var data = db.Feedback.Where(x => x.UserId == Uid).ToList();

            return View(data);
        }
        [HttpGet]
        public IActionResult Contacts()
        {
            var Uid = HttpContext.Session.GetInt32("UserId");
            if (Uid == null) return RedirectToAction("Login", "Authentication");

            var data = db.Contact.Where(x => x.UserId == Uid).ToList();

            return View(data);
        }
        [HttpGet]
        public IActionResult Account()
        {
            var Uid = HttpContext.Session.GetInt32("UserId");
            if (Uid == null) return RedirectToAction("Login", "Authentication");

            ViewBag.uFullName = HttpContext.Session.GetString("UserFullName");
            ViewBag.uName = HttpContext.Session.GetString("UserName");
            ViewBag.uEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.uImage = HttpContext.Session.GetString("UserImage");

            ViewBag.uDob = DateTime.Parse(HttpContext.Session.GetString("UserDOB")).ToString("yyyy-MM-dd");

            var data = db.User.FirstOrDefault(x => x.UserId == Uid);

            if (data == null)
                return NotFound();

            // ⭐ Logic: agar db me image nahi hai → default image
            if (string.IsNullOrEmpty(data.UserImage))
            {
                data.UserImage = "/images/defaultLogo.png";  // static file in wwwroot
            }
            else
            {
                // ⭐ Agar db me image hai → private folder se get karna hoga
                data.UserImage = "/Authentication/GetImage?file=" +
                                 Uri.EscapeDataString(data.UserImage);
            }

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserClass user, IFormFile file)
        {
            var Uid = HttpContext.Session.GetInt32("UserId");
            if (Uid == null)
                return RedirectToAction("Login", "Authentication");

            var UserDetails = db.User.FirstOrDefault(x => x.UserId == Uid);
            if (UserDetails == null)
                return NotFound();

            // BASIC DETAILS UPDATE
            UserDetails.UserFullName = string.IsNullOrWhiteSpace(user.UserFullName) ? UserDetails.UserFullName : user.UserFullName;
            UserDetails.UserName = string.IsNullOrWhiteSpace(user.UserName) ? UserDetails.UserName : user.UserName;
            UserDetails.UserEmail = string.IsNullOrWhiteSpace(user.UserEmail) ? UserDetails.UserEmail : user.UserEmail;
            UserDetails.UserDOB = string.IsNullOrWhiteSpace(user.UserDOB) ? UserDetails.UserDOB : user.UserDOB;

            // IMAGE UPLOAD + PNG CONVERSION
            if (file != null && file.Length > 0)
            {
                // delete old
                DeleteOldImage(UserDetails.UserImage);

                // convert uploaded image → PNG with unique filename
                string convertedFile = await ConvertToPngAsync(file);

                // save in DB
                UserDetails.UserImage = convertedFile;
            }

            db.User.Update(UserDetails);
            db.SaveChanges();

            // UPDATE SESSION
            HttpContext.Session.SetString("UserFullName", UserDetails.UserFullName);
            HttpContext.Session.SetString("UserName", UserDetails.UserName);
            HttpContext.Session.SetString("UserEmail", UserDetails.UserEmail);
            HttpContext.Session.SetString("UserDOB", UserDetails.UserDOB);
            HttpContext.Session.SetString("UserImage", UserDetails.UserImage);

            TempData["Msg"] = "Details Updated Successfully!";
            return RedirectToAction("Account", "User");
        }


        [HttpGet]
        public IActionResult Explore(int Cid)
        {
            var Uid = HttpContext.Session.GetInt32("UserId");
            if (Uid == null) return RedirectToAction("Login", "Authentication");

            var free = db.Course.Where(x => x.CoursePrice == 0 && x.CourseId == Cid).Select(a => a.CourseId).ToArray();
            var check = db.UserCourse.Where(x => x.CourseId == Cid && x.UserId == Uid).ToList();
            if (check.Count != 0 || free.Contains(Cid))
            {
                ViewBag.Btn = "Let's Study";
                ViewBag.action = "Study";
            }
            else
            {
                ViewBag.Btn = "Buy Now!";
                ViewBag.action = "Purchase";
            }

            var data = db.Course.FirstOrDefault(x => x.CourseId == Cid);

            if (data == null)
            {
                return RedirectToAction("AvailableCourse", "User"); // or return NotFound(); 
            }
            return View(data);

        }
        [HttpPost]
        public IActionResult Purchase(int Cid)
        {
            var Uid = HttpContext.Session.GetInt32("UserId");
            if (Uid == null) return RedirectToAction("Login", "Authentication");

            UserCourseClass userCourse = new UserCourseClass();

            userCourse.CourseId = Cid;

            userCourse.UserId = Uid.Value;

            var check = db.UserCourse.Where(x => x.CourseId == Cid && x.UserId == Uid).ToList();

            if (check.Count == 0)
            {
                db.UserCourse.Add(userCourse);
                db.SaveChanges();

                TempData["msg"] = "Congrats! Course Purchased Successfully";
            }
            return RedirectToAction("Explore", "User", new { Cid });
        }

      
        public IActionResult Study(int Cid)
        {
            var studyContent = db.Course.FirstOrDefault(x => x.CourseId == Cid);
            if (studyContent == null)
            {
                ViewBag.warning = "Invalid Course";
                return View();
            }
            return View(studyContent);
        }


        // ANY IMAGE → PNG CONVERSION (inside same file)
        // -------------------------------------------
        private async Task<string> ConvertToPngAsync(IFormFile file)
        {
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "PrivateUpload");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // UNIQUE FILENAME: DATETIME + RANDOM NUMBER
            string uniqueName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + new Random().Next(1000, 9999);
            string newFileName = uniqueName + ".png";

            string newFilePath = Path.Combine(uploadPath, newFileName);

            using (var image = await Image.LoadAsync(file.OpenReadStream()))
            {
                await image.SaveAsPngAsync(newFilePath);
            }

            return newFileName;
        }

        // DELETE OLD IMAGE (inside same file)
        // -------------------------------------------
        private void DeleteOldImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "PrivateUpload");
            string fullPath = Path.Combine(uploadPath, fileName);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

    }
}

