using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FashionHouse.Application.Contracts.Services;
using FashionHouse.Web.Models; 

namespace FashionHouse.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<ContactController> _logger;

        private const string ContactRecipientName = "Fashion House";
        private const string ContactRecipientEmail = "info@fashionhouse.com";

        public ContactController(IEmailService emailService, ILogger<ContactController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(ContactFormModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Website))
            {
                return Ok(new { success = true });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Please fill in all fields correctly." });
            }

            try
            {
                var subject = $"New contact form message from {model.Name}";
                var body = $@"
                    <p><strong>Name:</strong> {WebUtility.HtmlEncode(model.Name)}</p>
                    <p><strong>Email:</strong> {WebUtility.HtmlEncode(model.Email)}</p>
                    <p><strong>Message:</strong></p>
                    <p>{WebUtility.HtmlEncode(model.Message).Replace(Environment.NewLine, "<br/>")}</p>
                    <hr/>
                    <p style='color:#888;font-size:12px;'>Reply directly to this sender at {WebUtility.HtmlEncode(model.Email)} — IEmailService doesn't set a Reply-To header.</p>";

                await _emailService.SendEmailAsync(ContactRecipientName, ContactRecipientEmail, subject, body);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send contact email");
                return StatusCode(500, new { success = false, message = "Sorry, something went wrong sending your message. Please try again later." });
            }
        }
    }
}