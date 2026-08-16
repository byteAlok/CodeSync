using System.Drawing.Imaging;
using System.Drawing;
using Microsoft.AspNetCore.Mvc;

namespace Education.Controllers
{
    public class CaptchaController : Controller
    {
        // Step 1: Generate Random Captcha Text
        private string GenerateCaptchaText()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Avoid confusing chars
            var random = new Random();
            int length = 5;

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Step 2: Generate Image and Return as PNG
        [HttpGet]
        public IActionResult Generate()
        {
            var captchaText = GenerateCaptchaText();
            HttpContext.Session.SetString("CaptchaCode", captchaText); // store in session

            using var bmp = new Bitmap(150, 50);
            using var gfx = Graphics.FromImage(bmp);
            gfx.Clear(Color.White);

            var random = new Random();

            // Draw random background lines
            for (int i = 0; i < 5; i++)
            {
                gfx.DrawLine(new Pen(Color.LightGray, 1),
                    random.Next(0, bmp.Width),
                    random.Next(0, bmp.Height),
                    random.Next(0, bmp.Width),
                    random.Next(0, bmp.Height));
            }

            // Draw characters
            var fonts = new[] { "Arial", "Tahoma", "Verdana", "Times New Roman" };
            for (int i = 0; i < captchaText.Length; i++)
            {
                using var font = new Font(fonts[random.Next(fonts.Length)], 22, FontStyle.Bold);
                var brush = new SolidBrush(Color.FromArgb(random.Next(100), random.Next(255), random.Next(255)));
                float x = 10 + i * 25;
                float y = random.Next(5, 15);

                gfx.TranslateTransform(x, y);
                gfx.RotateTransform(random.Next(-20, 20));
                gfx.DrawString(captchaText[i].ToString(), font, brush, 0, 0);
                gfx.ResetTransform();
            }

            // Add noise dots
            for (int i = 0; i < 100; i++)
            {
                bmp.SetPixel(random.Next(bmp.Width), random.Next(bmp.Height),
                             Color.FromArgb(random.Next(255), random.Next(255), random.Next(255)));
            }

            // Convert image to memory stream
            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);

            return File(ms.ToArray(), "image/png");
        }
    }
}
