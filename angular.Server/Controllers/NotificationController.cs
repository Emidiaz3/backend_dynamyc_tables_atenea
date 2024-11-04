
using ApiRestCuestionario.Context;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace angular.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly NotificationService notificationService;

        public NotificationController(AppDbContext context, NotificationService notificationService)
        {
            this.context = context;
            this.notificationService = notificationService;
        }

        [HttpPost("SendNotifications")]
        public async Task<NotificationResult> SendNotifications()
        {
            try
            {
                var tokens = await context.T_MAE_USUARIO_TOKEN
                                .Where(t => t.Activo)
                                .Select(t => t.Token)
                                .ToListAsync();

                await notificationService.SendNotificationToMultipleDevicesAsync(tokens, "ALERTA!!!", "me están secuestrando");
                return new NotificationResult { Success = true };
            }
            catch (Exception ex)
            {
                return new NotificationResult { Success = false, ErrorMessage = ex.Message };
            }
        }

    }
}

public class NotificationResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}
