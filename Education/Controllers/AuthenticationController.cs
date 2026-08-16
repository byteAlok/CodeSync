using Education.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Security.Claims;

namespace Education.Controllers
{
    public class AuthenticationController(MyConnection _db) : Controller 
    {
        private readonly MyConnection db = _db;
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string NameorEmail, string pass)
        {

            var UserDetails = db.User.FirstOrDefault(a => (a.UserName == NameorEmail || a.UserEmail == NameorEmail)
                                && a.UserPassword == pass && a.UserExist == true);

            var AdminDetails = db.Admin.FirstOrDefault(a => (a.AdminName == NameorEmail || a.AdminEmail == NameorEmail)
                                && a.AdminPassword == pass);

            if (UserDetails != null)
            {

                HttpContext.Session.SetInt32("UserId", UserDetails.UserId);
                HttpContext.Session.SetString("UserFullName", UserDetails.UserFullName);
                HttpContext.Session.SetString("UserName", UserDetails.UserName);
                HttpContext.Session.SetString("UserDOB", UserDetails.UserDOB ?? "");
                HttpContext.Session.SetString("UserEmail", UserDetails.UserEmail);
                HttpContext.Session.SetString("UserImage", UserDetails.UserImage ?? "");
                HttpContext.Session.SetString("UserRegisterDate", UserDetails.UserRegisterDate);
                HttpContext.Session.SetString("UserExist", UserDetails.UserExist.ToString());

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, UserDetails.UserName),
                    new Claim(ClaimTypes.Role, "User")                    
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                var props = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(300)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

                return RedirectToAction("Dashboard", "User");
            }

            else if (AdminDetails != null)
            {
                HttpContext.Session.SetInt32("AdminId", AdminDetails.AdminId);
                HttpContext.Session.SetString("AdminName", AdminDetails.AdminName);
                HttpContext.Session.SetString("AdminDOB", AdminDetails.AdminDOB ?? "");
                HttpContext.Session.SetString("AdminEmail", AdminDetails.AdminEmail);
                HttpContext.Session.SetString("AdminPhone", AdminDetails.AdminPhone);
                HttpContext.Session.SetString("AdminImage", AdminDetails.AdminImage ?? "");
                HttpContext.Session.SetString("AdminRegisterDate", AdminDetails.AdminRegisterDate);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, AdminDetails.AdminEmail),
                    new Claim(ClaimTypes.Role, "Admin")                    
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                var props = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(300)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

                return RedirectToAction("Index", "Admin");
            }
            TempData["invalid"] = "Invalid Username or Password";
            return View();
        }
        public IActionResult Register() => View();
        [HttpPost]
        public IActionResult Register(UserClass usr, string CaptchaInput)
        {
            string str = Submit(CaptchaInput);
            TempData["capMsg"] = str;

            var check = db.User.Any(x => x.UserName == usr.UserName && x.UserEmail == usr.UserEmail);

            if (check)
            {
                TempData["Error"] = "UserName ya Email already exists!";
                return View();
            }

            if (ModelState.IsValid && str == "Captcha Verified!")
            {
                db.User.Add(usr);
                db.SaveChanges();
                TempData["regMsg"] = "Registration Successfull!";
                return RedirectToAction("Login", "Authentication");
            }

            TempData["Error"] = "Registration Failed!";
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("EducationSessionCookie");

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult GetImage(string file)
        {
            var defaultImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "defaultLogo.png");

            // 1. If no file, return default
            if (string.IsNullOrEmpty(file))
                return PhysicalFile(defaultImgPath, "image/png");

            // 2. Security check
            if (file.Contains("..") || file.Contains("/") || file.Contains("\\"))
                return Unauthorized();

            // 3. Build user image path
            var path = Path.Combine(Directory.GetCurrentDirectory(), "PrivateUpload", file);

            // 4. If user image missing → return default
            if (!System.IO.File.Exists(path))
                return PhysicalFile(defaultImgPath, "image/png");

            // 5. Return actual file
            return PhysicalFile(path, "image/png");
        }


        public string Submit(string CaptchaInput)
        {
            var storedCaptcha = HttpContext.Session.GetString("CaptchaCode");

            if (CaptchaInput != storedCaptcha)
            {
                
                return "Invalid Captcha!";
            }
            
            return "Captcha Verified!";
        }


    }
}