using FlowMind.Data;
using FlowMind.Models;

namespace FlowMind.Services
{
    public class FakeEmailService : IEmailService
    {
        private readonly ILogger<FakeEmailService> _logger;
        private readonly FlowMindDbContext _context;

        public FakeEmailService(ILogger<FakeEmailService> logger, FlowMindDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task SendEmailAsync(string to, string subject, string body, string? workflowInstanceId = null)
        {
            // 1. Console Log
            _logger.LogInformation("--- FAKE EMAIL SENT ---");
            _logger.LogInformation("To      : {To}", to);
            _logger.LogInformation("Subject : {Subject}", subject);
            _logger.LogInformation("Body    : {Body}", body);
            _logger.LogInformation("-----------------------");

            // 2. IntegrationLog Kaydı
            var log = new IntegrationLog
            {
                WorkflowInstanceId = workflowInstanceId ?? "SİSTEM",
                HedefSistem = IntegrationType.EPosta,
                Islem = "SendEmail",
                GonderilenVeri = $"{{\"To\":\"{to}\",\"Subject\":\"{subject}\"}}",
                Yanit = "{\"Status\":\"Sent via FakeEmailService\"}",
                Durum = IntegrationStatus.Basarili,
                SureMs = new Random().Next(50, 150),
                Zaman = DateTime.Now
            };

            _context.IntegrationLogs.Add(log);

            // 3. Arayüzde görünmesi için Notification kaydı oluştur
            var notification = new Notification
            {
                KullaniciId = "USR-004", // Hackathon için varsayılan aktif kullanıcıya (veya global görsün diye birine) atayalım
                Baslik = "E-Posta Gönderildi",
                Mesaj = $"Kime: {to} | Konu: {subject}",
                Tur = NotificationType.EntegrasyonSonucu,
                Okundu = false,
                OlusturmaTarihi = DateTime.Now
            };
            
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
        }
    }
}
