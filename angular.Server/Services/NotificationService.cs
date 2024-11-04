using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class NotificationService
{
    private readonly IConfiguration _config;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IConfiguration config, ILogger<NotificationService> logger)
    {
        _config = config;
        _logger = logger;

        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        try
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                var path = _config["Firebase:ServiceAccountKeyPath"];
                if (string.IsNullOrEmpty(path))
                {
                    throw new InvalidOperationException("Firebase:ServiceAccountKeyPath is not set in configuration.");
                }

                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(path)
                });
                _logger.LogInformation("Firebase initialized successfully.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Firebase.");
            throw; // Rethrow the exception to prevent the application from starting with an improperly initialized Firebase
        }
    }

    public async Task<NotificationResult> SendNotificationAsync(string token, string title, string body)
    {
        try
        {
            var message = new Message()
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                }
            };

            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            _logger.LogInformation("Successfully sent message: {Response}", response);
            return new NotificationResult { Success = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification.");
            return new NotificationResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<NotificationResult> SendNotificationToMultipleDevicesAsync(List<string> tokens, string title, string body)
    {
        try
        {
            _logger.LogInformation("Intentando enviar notificación a {TokenCount} dispositivos", tokens.Count);
            var message = new MulticastMessage()
            {
                Tokens = tokens,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                }
            };

            var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            _logger.LogInformation("Respuesta de Firebase: Éxito: {SuccessCount}, Fallos: {FailureCount}",
                                   response.SuccessCount, response.FailureCount);

            if (response.FailureCount > 0)
            {
                foreach (var error in response.Responses.Where(r => !r.IsSuccess))
                {
                    _logger.LogError("Error al enviar notificación: {ErrorCode}", error.Exception?.Message);
                }
                return new NotificationResult { Success = false, ErrorMessage = $"{response.FailureCount} mensajes fallaron al enviarse." };
            }

            return new NotificationResult { Success = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al enviar notificaciones múltiples");
            return new NotificationResult { Success = false, ErrorMessage = ex.Message };
        }
    }
}