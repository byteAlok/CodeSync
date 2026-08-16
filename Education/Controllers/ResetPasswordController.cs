using Microsoft.AspNetCore.Mvc;
using Education.Models;
using System.ComponentModel;

namespace Education.Controllers
{
    public class ResetPasswordController(MyConnection _db) : Controller
    {
        private readonly MyConnection db = _db;

        // ============================
        // 1. Forget Password
        // ============================
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var checkUser = db.User.Any(x => x.UserEmail == email && x.UserExist == true);
            var checkAdmin = db.Admin.Any(x => x.AdminEmail == email);

            if (checkUser)
                HttpContext.Session.SetString("role", "User");
            else if (checkAdmin)
                HttpContext.Session.SetString("role", "Admin");

            if (checkUser || checkAdmin)
            {
                // Call email OTP sender
                EmailServices serve = new EmailServices();
                var data = await serve.Email(email);

                // Get OTP from email controller
                int otp = data.Otp;

                // Store in session
                HttpContext.Session.SetInt32("otp", otp);
                HttpContext.Session.SetString("email", email);
                HttpContext.Session.SetInt32("otpCount", 0);

                TempData["sentOtp"] = data.Msg;
                return RedirectToAction("VerifyOTP", "ResetPassword");
            }

            TempData["foundError"] = "Details Not Found!";
            return View("ForgetPassword");
        }

        // ============================
        // 2. OTP Verification
        // ============================
        public IActionResult VerifyOTP()
        {
            return View();
        }
        [HttpPost]
        public IActionResult VerifyOTP(int userOtp)
        {
            int? serverOtp = HttpContext.Session.GetInt32("otp");

            if (serverOtp == null)
            {
                TempData["timeOut"] = "OTP expired! Try again.";
                return RedirectToAction("ForgetPassword");
            }

            int count = HttpContext.Session.GetInt32("otpCount") ?? 0;

            if (userOtp == serverOtp)
            {
                TempData["verifyOtp"] = "OTP Verified : Now Create Your New Password";
                
                HttpContext.Session.Remove("otpCount"); // Reset counter
                return RedirectToAction("CreatePassword", "ResetPassword");
            }

            // Wrong OTP
            count++;
            HttpContext.Session.SetInt32("otpCount", count);
            TempData["errorOtp"] = "Invalid OTP : Enter Correct OTP";

            if (count >= 3)
            {
                TempData["errorOtp"] = null;
                HttpContext.Session.Remove("otpCount");
                TempData["errOtp"] = "Invalid OTP : Click to resend OTP";
                return RedirectToAction("ForgetPassword", "ResetPassword");
            }

            return View();
        }

        // ============================
        // 3. Create New Password
        // ============================
        public IActionResult CreatePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreatePassword(string pass, string cpass)
        {
            var email = HttpContext.Session.GetString("email");
            var role = HttpContext.Session.GetString("role");

            if (email == null || role == null)
            {
                TempData["timeOut"] = "Session Timeout : Try Again!";
                return RedirectToAction("ForgetPassword", "ResetPassword");
            }

            dynamic account = null;

            if (pass == cpass)
            {
                if (role == "User")
                    account = db.User.FirstOrDefault(x => x.UserEmail == email);
                else if (role == "Admin")
                    account = db.Admin.FirstOrDefault(x => x.AdminEmail == email);

                if (account != null)
                {
                    if (role == "User")
                        account.UserPassword = pass;
                    else
                        account.AdminPassword = pass;

                    db.SaveChanges();

                    TempData["success"] = "Password Updated Successfully!";
                    return RedirectToAction("Login", "Authentication");
                }
            }
            else
            {
                TempData["mismatch"] = "Password & Confirm Password do not match!";
            }

            return View();
        }
    }
}
