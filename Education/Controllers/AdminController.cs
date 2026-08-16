using System.Data.Common;
using System.Globalization;
using AspNetCoreGeneratedDocument;
using Education.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Education.Controllers
{   
    [Authorize(Policy = "AdminOnly")]
    public class AdminController(MyConnection db) : Controller
    {
        private readonly MyConnection _db = db;

        public IActionResult Index()
        {
            var AdminList = _db.Admin.ToList();
            ViewBag.TotalAdmin = AdminList.Count;
            ViewBag.TotalStudent = _db.User.ToList().Count;
            ViewBag.TotalCourse = _db.Course.ToList().Count;
            return View(AdminList);
        }
        [HttpGet]
        public IActionResult ShowAdminImage(string FileName)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                return NotFound();
            }

            string ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "PrivateUpload", FileName);

            if (!System.IO.File.Exists(ImagePath))
            {
                return NotFound();
            }

            return PhysicalFile(ImagePath, "image/jpeg");
        }
        [HttpPost]
        public IActionResult CreateAdmin(Admin admin, string ConfirmPassword, IFormFile AdminImage)
        {

            if (admin.AdminPassword != ConfirmPassword)
            {
                TempData["AdminCreateErrorMessage"] = "Passwords & Confirm Password do not match!";
                return RedirectToAction("Index");
            }

            if (AdminImage != null && AdminImage.Length > 0)
            {
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(),"PrivateUpload");

                string FileExtension = Path.GetExtension(AdminImage.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                string uniqueFileName = $"admin_{timestamp}{FileExtension}";
                string FilePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(FilePath, FileMode.Create))
                {
                    AdminImage.CopyTo(fileStream);
                }

                admin.AdminImage = uniqueFileName;

            }

            _db.Admin.Add(admin);
            _db.SaveChanges();
            TempData["AdminCreateSuccessMessage"] = "Admin created successfully!";
            return RedirectToAction("Index");



        }

        [HttpPost]
        public IActionResult Edit(Admin admin, IFormFile AdminImage)
        {
                var AdminIdFetch = _db.Admin.Find(admin.AdminId);

                AdminIdFetch.AdminName = admin.AdminName;
                AdminIdFetch.AdminEmail = admin.AdminEmail;
                AdminIdFetch.AdminDOB = admin.AdminDOB;
                AdminIdFetch.AdminPhone = admin.AdminPhone;

                if(AdminImage!=null && AdminImage.Length > 0)
                {
                        string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "PrivateUpload");
                    if (!string.IsNullOrEmpty(AdminIdFetch.AdminImage))
                    {
                    string oldImagePath = Path.Combine(uploadFolder, AdminIdFetch.AdminImage);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                    }
                string FileExtension = Path.GetExtension(AdminImage.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                string uniqueFileName = $"admin_{timestamp}{FileExtension}";
                string FilePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(FilePath, FileMode.Create))
                {
                    AdminImage.CopyTo(fileStream);
                }

                AdminIdFetch.AdminImage = uniqueFileName;
            }


            _db.SaveChanges();

            // for session update
            HttpContext.Session.SetString("AdminName", AdminIdFetch.AdminName);
            HttpContext.Session.SetString("AdminDOB", AdminIdFetch.AdminDOB ?? "");
            HttpContext.Session.SetString("AdminEmail", AdminIdFetch.AdminEmail);
            HttpContext.Session.SetString("AdminPhone", AdminIdFetch.AdminPhone);
            HttpContext.Session.SetString("AdminImage", AdminIdFetch.AdminImage ?? "");

            TempData["SuccessMessage"] = "Admin updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int AdminId)
        {
            var admin = _db.Admin.Find(AdminId);
            if (admin != null)
            {
                _db.Admin.Remove(admin);
                _db.SaveChanges();
                TempData["SuccessMessage"] = "Admin deleted successfully!";
            }
            return RedirectToAction("Index");
        }


        // Index page end



        public IActionResult students()
        {
            var studentlist = _db.User.ToList();

            ViewBag.Totalstudents = studentlist.Count;
            ViewBag.AdminCount = _db.Admin.ToList().Count;
            ViewBag.TotalCourse = _db.Course.ToList().Count;
            ViewBag.Activestudents = studentlist.Count(s => s.UserExist == true);
            ViewBag.Inactivestudents = studentlist.Count(s => s.UserExist == false);

            int[] RegisteredstudentForGraph = new int[12];
            int[] ActivestudentForGraph = new int[12];

            foreach (var n in studentlist)
            {
                var date = DateTime.ParseExact( n.UserRegisterDate, "dddd, dd-MMMM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

                int monthIndex = date.Month - 1;
                RegisteredstudentForGraph[monthIndex]++;

                if (n.UserExist == true)
                {
                    ActivestudentForGraph[monthIndex]++;
                }
            }
            ViewBag.RegisteredstudentForGraph = RegisteredstudentForGraph;
            ViewBag.ActivestudentForGraph = ActivestudentForGraph;

            return View(studentlist);
        }


        public IActionResult ShowStudentImage(string FileName)
        {

            if (string.IsNullOrEmpty(FileName))
            {
                return NotFound();
            }

            string ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "PrivateUpload", FileName);

            if (!System.IO.File.Exists(ImagePath))
            {
                return NotFound();
            }

            return PhysicalFile(ImagePath, "image/jpeg");

        }

        [HttpPost]
        public IActionResult Updatestudent(UserClass student, IFormFile UserImage)
        {
            var studentIdFetch = _db.User.Find(student.UserId);
            studentIdFetch.UserFullName=student.UserFullName;
            studentIdFetch.UserName=student.UserName;
            studentIdFetch.UserDOB = student.UserDOB;
            studentIdFetch.UserEmail = student.UserEmail;


            if (UserImage != null && UserImage.Length > 0)
            {
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "PrivateUpload");
                if (!string.IsNullOrEmpty(student.UserImage))
                {
                    string oldImagePath = Path.Combine(uploadFolder, student.UserImage);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                string FileExtension = Path.GetExtension(UserImage.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                string uniqueFileName = $"admin_{timestamp}{FileExtension}";
                string FilePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(FilePath, FileMode.Create))
                {
                    UserImage.CopyTo(fileStream);
                }

                studentIdFetch.UserImage = uniqueFileName;
            }



            _db.SaveChanges();
            TempData["UpdateSuccessMessage"] = "student updated successfully!";

            return RedirectToAction("Students");
        }

        [HttpPost]
        public IActionResult DeleteStudent(int StudentId)
        {
            var Student = _db.User.Find(StudentId);
            if (Student != null)
            {
                _db.Remove(Student);
                _db.SaveChanges();
                TempData["StudentDeleteSuccessMessage"] = "Student deleted successfully!";
            }
            return RedirectToAction("Students");
        }



        public IActionResult Course()
        {
            var CourseList = _db.Course.ToList();
            ViewBag.TotaCourse = CourseList.Count;
            ViewBag.TotalAdmin = _db.Admin.ToList().Count;
            ViewBag.TotalStudent = _db.User.ToList().Count;
            var UserCourseList = _db.UserCourse.ToList();

            string[] CourserNameG = new string[CourseList.Count];
            string[] StudentCountG = new string[CourseList.Count];
            decimal[] RevenueG = new decimal[CourseList.Count];

            for(int i = 0; i < CourseList.Count; i++)
            {
                int count = 0;
                foreach(var usercount in UserCourseList)
                {
                    if(usercount.CourseId == CourseList[i].CourseId)
                    {
                        count++;
                    }
                }

                CourserNameG[i] = CourseList[i].CourseName;
                StudentCountG[i] = count.ToString();
                RevenueG[i] = CourseList[i].CoursePrice * count;
            }

            ViewBag.CourseNamesG = CourserNameG;
            ViewBag.StudentCountG = StudentCountG;
            ViewBag.RevenueG = RevenueG;


            return View(CourseList);
        }
        public IActionResult ShowCourseImage(string FileName)
        {

            if (string.IsNullOrEmpty(FileName))
            {
                return NotFound();
            }

            string ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "course-images", FileName);

            if (!System.IO.File.Exists(ImagePath))
            {
                return NotFound();
            }

            return PhysicalFile(ImagePath, "image/jpg");

        }

        [HttpPost]
        public IActionResult CreateCourse(CourseClass course, IFormFile CourseImage)
        {
            if (CourseImage != null && CourseImage.Length > 0)
            {
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "course-images");


                string FileExtension = Path.GetExtension(CourseImage.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                string uniqueFileName = $"Course_{timestamp}{FileExtension}";
                string FilePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(FilePath, FileMode.Create))
                {
                    CourseImage.CopyTo(fileStream);
                }

                course.CourseImage = uniqueFileName;

            }

            _db.Add(course);
            _db.SaveChanges();
            TempData["CreateCourseSuccessMessage"] = "Course Added Succesfully";
            return RedirectToAction("Course");
        }


        [HttpPost]
        public IActionResult UpdateCourse(CourseClass course, IFormFile cImg)
        {
            var courseIdFetch = _db.Course.Find(course.CourseId);

            courseIdFetch.CourseName = course.CourseName;
            courseIdFetch.CoursePrice = course.CoursePrice;
            courseIdFetch.CourseStartDate = course.CourseStartDate;
            courseIdFetch.CourseEndDate = course.CourseEndDate;
            courseIdFetch.CourseDuration = course.CourseDuration;
            courseIdFetch.CourseLanguage = course.CourseLanguage;

            if (cImg != null && cImg.Length > 0)
            {
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "course-images");

                string oldImagePath = Path.Combine(uploadFolder, courseIdFetch.CourseImage);
                if (System.IO.File.Exists(oldImagePath))
                {
                   System.IO.File.Delete(oldImagePath);
                }
                
                string FileExtension = Path.GetExtension(cImg.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");

                string sanitizedCourseName = courseIdFetch.CourseName.Replace(" ", "_");
                string uniqueFileName = $"{sanitizedCourseName}_{timestamp}{FileExtension}";

                string FilePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(FilePath, FileMode.Create))
                {
                    cImg.CopyTo(fileStream);
                }
                courseIdFetch.CourseImage = uniqueFileName;
            }
            _db.Course.Update(courseIdFetch);
            _db.SaveChanges();
            TempData["CourseUpdateSuccesfull"] = "Course Updated Succesfully";
            return RedirectToAction("Course");
        }

        [HttpPost]

        public IActionResult DeleteCourse(int CourseId)
        {
            var CourseRemoveId = _db.Course.Find(CourseId);
            _db.Remove(CourseRemoveId);
            _db.SaveChanges();
            TempData["CourseDeleteSuccess"] = "Course Deleted Succesfully";
            return RedirectToAction("Course");
        }


        public IActionResult Contact()
        {
            var ContactList = _db.Contact.ToList();
            return View(ContactList);
        }

        public IActionResult Feedback()
        {
            var FeedbackList = _db.Feedback.ToList();
            return View(FeedbackList);
        }
        public IActionResult Teacher()
        {
            return View();
        }
        public IActionResult Product()
        {
            return View();
        }

        public IActionResult AdminSetting()
        {
            var AdminId = HttpContext.Session.GetInt32("AdminId");
            if (AdminId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
          
            var AdminDetail = _db.Admin.Find(AdminId);

            return View(AdminDetail);
        }

      /*  public IActionResult AdminSettingImageShow(string FileName)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                return NotFound();
            }

            string ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "PrivateUpload", FileName);

            if (!System.IO.File.Exists(ImagePath))
            {
                return NotFound();
            }

            return PhysicalFile(ImagePath, "image/jpeg");
        }*/
          
   
        public IActionResult AdminSettingImageShow(string FileName)
        {
            // Correct absolute default image path
            var defaultImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "defaultLogo.png" );

            // 1. If no file → show default
            if (string.IsNullOrEmpty(FileName))
                return PhysicalFile(defaultImgPath, "image/png");

            // 2. Security check
            if (FileName.Contains("..") || FileName.Contains("/") || FileName.Contains("\\"))
                return Unauthorized();

            // 3. Actual image path
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "PrivateUpload", FileName);

            // 4. If missing → show default
            if (!System.IO.File.Exists(imagePath))
                return PhysicalFile(defaultImgPath, "image/png");

            // 5. Return real image
            return PhysicalFile(imagePath, "image/png");
        }


        [HttpPost]
        public IActionResult AdminAccountUpdate(Admin admin, IFormFile AdminImage)
        {
            var AdminIdFetch = _db.Admin.Find(admin.AdminId);
            AdminIdFetch.AdminName = admin.AdminName;
            AdminIdFetch.AdminEmail = admin.AdminEmail;
            AdminIdFetch.AdminDOB = admin.AdminDOB;
            AdminIdFetch.AdminPhone = admin.AdminPhone;

            if (AdminImage != null && AdminImage.Length > 0)
            {
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "PrivateUpload");
                if (!string.IsNullOrEmpty(AdminIdFetch.AdminImage))
                {
                    string oldImagePath = Path.Combine(uploadFolder, AdminIdFetch.AdminImage);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                string FileExtension = Path.GetExtension(AdminImage.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                string uniqueFileName = $"admin_{timestamp}{FileExtension}";
                string FilePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(FilePath, FileMode.Create))
                {
                    AdminImage.CopyTo(fileStream);
                }

                AdminIdFetch.AdminImage = uniqueFileName;
            }


            _db.SaveChanges();
            TempData["SuccessMessage"] = "Details successfully updated";

            return RedirectToAction("AdminSetting");
        }

        [HttpPost]
        public IActionResult AdminAccountDelete(int AdminId)
        {
            var admin = _db.Admin.Find(AdminId);
            if (admin != null)
            {
                _db.Admin.Remove(admin);
                _db.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}