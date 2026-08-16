using Microsoft.AspNetCore.Mvc;
using Education.Models;
using System;
using Microsoft.AspNetCore.Authorization;


namespace Education.Controllers
{
    [Authorize]
    public class ContactController : Controller
    {
        private readonly MyConnection db;

        public ContactController(MyConnection _db)
        {
            db = _db;
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Contact( ContactClass contact)
        {
            var Uid = HttpContext.Session.GetInt32("UserId");
            if (Uid == null) return RedirectToAction("Login", "Authentication");

            contact.UserId = (int)Uid;

            if (!ModelState.IsValid)
            {
                db.Contact.Add(contact);
                db.SaveChanges();
                ViewBag.Msg = "Message Sent Successfully";
                TempData["Msg"] = "Message Sent Successfully";
                return View();
            }
            ViewBag.Msg = "Failed!";
            TempData["Msg"] = "Failed";
            return View();
        }


    }
}
