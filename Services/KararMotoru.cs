namespace FlowMind.Services
{
    /// <summary>
    /// AI Karar Motoru implementasyonu.
    /// Kural tabanlı basit bir karar mekanizması kullanır.
    /// Hackathon demosu için mock yönlendirme yapar.
    /// </summary>
    public class KararMotoru : IKararMotoru
    {
        private readonly ILogger<KararMotoru> _logger;

        public KararMotoru(ILogger<KararMotoru> logger)
        {
            _logger = logger;
        }

        public string KararUret(string isAkisiTuru)
        {
            _logger.LogInformation("Karar üretiliyor: {IsAkisiTuru}", isAkisiTuru);

            return isAkisiTuru.ToLower() switch
            {
                "siparis" => "Sipariş iş akışı → ERP sistemine yönlendirildi.",
                "destek" => "Destek talebi → Müşteri hizmetlerine yönlendirildi.",
                "onay" => "Onay süreci → Yönetici paneline yönlendirildi.",
                "bildirim" => "Bildirim → İletişim servisine yönlendirildi.",
                _ => $"Bilinmeyen tür '{isAkisiTuru}' → Manuel incelemeye alındı."
            };
        }

        public string Yonlendir(string isAkisiTuru)
        {
            _logger.LogInformation("Yönlendirme yapılıyor: {IsAkisiTuru}", isAkisiTuru);

            return isAkisiTuru.ToLower() switch
            {
                "siparis" => "ERP Servisi",
                "destek" => "CRM Servisi",
                "onay" => "Onay Servisi",
                "bildirim" => "İletişim Servisi",
                _ => "Manuel İnceleme"
            };
        }
    }
}
