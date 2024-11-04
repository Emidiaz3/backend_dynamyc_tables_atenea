using angular.Server.Model;
using ApiRestCuestionario.Context;
using DotLiquid;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiRestCuestionario.Utils;
using Hangfire;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace angular.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SendingPhonesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly NotificationService _notificationService;
        private readonly IGmailSender _emailSender;
        private readonly ILogger<SendingPhonesController> _logger;

        public SendingPhonesController(AppDbContext context, HttpClient httpClient, NotificationService notificationService, IGmailSender emailSender, ILogger<SendingPhonesController> logger)
        {
            _context = context;
            _httpClient = httpClient;
            _notificationService = notificationService;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetSendingPhones()
        {
            var phones = await _context.SendingPhone.ToListAsync();
            return Ok(phones);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSendingPhone(int id)
        {
            var phone = await _context.SendingPhone.FindAsync(id);
            if (phone == null)
            {
                return NotFound();
            }
            return Ok(phone);
        }

        [HttpPost]
        public async Task<IActionResult> AddSendingPhone([FromBody] SendingPhone phone)
        {
            if (phone.ClientId == Guid.Empty)
            {
                phone.ClientId = Guid.NewGuid();
            }
            _context.SendingPhone.Add(phone);
            await _context.SaveChangesAsync();
            return Ok(phone);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSendingPhone(int id, [FromBody] SendingPhone updatedPhone)
        {
            var phone = await _context.SendingPhone.FindAsync(id);
            if (phone == null)
            {
                return NotFound();
            }

            phone.PhoneNumber = updatedPhone.PhoneNumber;
            phone.IsActive = updatedPhone.IsActive;
            phone.ClientId = updatedPhone.ClientId;

            await _context.SaveChangesAsync();
            return Ok(phone);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSendingPhone(int id)
        {
            var phone = await _context.SendingPhone.FindAsync(id);
            if (phone == null)
            {
                return NotFound();
            }

            _context.SendingPhone.Remove(phone);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("qr/{clientId}")]
        public async Task<IActionResult> GetQRCode(Guid clientId)
        {
            var response = await _httpClient.GetStringAsync($"http://localhost:3000/qr/{clientId}");
            return Content(response, "text/html");
        }

        [HttpGet("status/{clientId}")]
        public async Task<IActionResult> GetConnectionStatus(Guid clientId)
        {
            var response = await _httpClient.GetStringAsync($"http://localhost:3000/status/{clientId}");
            return Content(response, "application/json");
        }

        [HttpPost("send-message")]
        public IActionResult SendMessage([FromBody] AlertMessageRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            BackgroundJob.Enqueue(() => ProcessSendMessage(userId, request.Message));
            return Ok(new { message = "Message sending has been initiated." });
        }

        [NonAction]
        public async Task ProcessSendMessage(string userId, string message)
        {
            var tokens = await _context.T_MAE_USUARIO_TOKEN
                                       .Where(t => t.Activo)
                                       .Select(t => t.Token)
                                       .ToListAsync();
            var notificationMethod = "Push Notification";
            var emailMethod = "Email";
            var smsMethod = "SMS";
            var notificationResponse = "Success";
            var emailResponse = "Success";
            var smsResponse = "Success";

            if (tokens.Any())
            {
                try
                {
                    var notificationResult = await _notificationService.SendNotificationToMultipleDevicesAsync(tokens, "ALERTA!!!", message);
                    notificationResponse = notificationResult.Success ? "Success" : notificationResult.ErrorMessage;
                    if (!notificationResult.Success)
                    {
                        _logger.LogError("Failed to send notifications: {ErrorMessage}", notificationResult.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception occurred while sending push notifications");
                    notificationResponse = $"Exception: {ex.Message}";
                }
            }
            else
            {
                _logger.LogWarning("No active tokens found for push notifications");
                notificationResponse = "No active tokens";
            }


            var emails = await _context.Email
                                       .Where(e => e.IsActive)
                                       .Select(e => e.Address)
                                       .ToListAsync();

            foreach (var email in emails)
            {
                try
                {
                    await _emailSender.SendEmailAsync(email, "ALERTA!!!", message);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to send email to {Email}: {ErrorMessage}", email, ex.Message);
                    emailResponse = ex.Message;
                }
            }

            var receivingPhones = await _context.ReceivingPhone.ToListAsync();
            var sendingPhone = await _context.SendingPhone.FirstOrDefaultAsync(phone => phone.IsActive);

            if (sendingPhone != null && receivingPhones.Any())
            {
                foreach (var receivingPhone in receivingPhones)
                {
                    var sendMessageRequest = new SendMessageRequest
                    {
                        ClientId = sendingPhone.ClientId.ToString(),
                        PhoneNumber = receivingPhone.PhoneNumber,
                        Message = message
                    };
                    var response = await _httpClient.PostAsJsonAsync("http://localhost:3000/send-message", sendMessageRequest);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("Failed to send message to {PhoneNumber}: {ErrorMessage}", receivingPhone.PhoneNumber, await response.Content.ReadAsStringAsync());
                        smsResponse = await response.Content.ReadAsStringAsync();
                    }
                }
            }

            var logEntry = new AlertLog
            {
                UserId = userId,
                Message = message,
                Date = DateTime.UtcNow,
                Method = $"{notificationMethod}, {emailMethod}, {smsMethod}",
                Response = $"{notificationResponse}, {emailResponse}, {smsResponse}"
            };

            _context.AlertLogs.Add(logEntry);
            await _context.SaveChangesAsync();
        }
    }

    public class SendMessageRequest
    {
        public string ClientId { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
    }

    public class AlertMessageRequest
    {
        public string Message { get; set; }
    }
}
