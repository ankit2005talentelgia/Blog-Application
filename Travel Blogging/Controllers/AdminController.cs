using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Travel_Blogging.DTOs.ContactDto;
using Travel_Blogging.Services.Interfaces;

namespace Travel_Blogging.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IToastNotification _toastNotification;

        public AdminController(IEmailService emailService, IConfiguration config, IToastNotification toastNotification)
        {
            _emailService = emailService;
            _config = config;
            _toastNotification = toastNotification;
        }



        // Added explicit route to match @Url.Action("Contact", "Admin")
        [HttpGet("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        // Add route for handle contact details
        [HttpPost("contact")]
        public async Task<IActionResult> Contact(ContactDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try 
            {
                // Get admin email from config
                var adminEmail = _config["EmailSettings:SenderEmail"];
                // Construct the email details
                var emailSubject = $"New Contact Message: {dto.Subject} from {dto.Name}";
                var emailBody = $"<h3>New Message from Contact Form</h3>" +
                                $"<p><strong>Name:</strong> {dto.Name}</p>" +
                                $"<p><strong>Email:</strong> {dto.Email}</p>" +
                                $"<p><strong>Subject:</strong> {dto.Subject}</p>" +
                                $"<p><strong>Message:</strong><br/>{dto.Message.Replace("\n", "<br/>")}</p>";

                // Send email to admin using Brevo email service
                await _emailService.SendEmailAsync(adminEmail, emailSubject, emailBody);
                
                // Use TempData to signal success to the View after redirect
                TempData["ContactSuccess"] = true;
                
                // Redirect to the GET action to clear the form and prevent resubmission on refresh
                return RedirectToAction(nameof(Contact));
            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage("Something went wrong. Please try again later.");
                return View(dto);
            }
        }
    }
}
