namespace FlowMind.Services
{
    /// <summary>
    /// İş akışı yönetim servisinin varsayılan implementasyonu.
    /// Şimdilik in-memory mock veriler kullanır, ileride veritabanına bağlanacak.
    /// </summary>
    public class IsAkisiServisi : IIsAkisiServisi
    {
        private readonly ILogger<IsAkisiServisi> _logger;

        public IsAkisiServisi(ILogger<IsAkisiServisi> logger)
        {
            _logger = logger;
        }

        public int ToplamIsAkisiSayisi()
        {
            // TODO: Veritabanı bağlandığında gerçek veri çekilecek
            _logger.LogInformation("Toplam iş akışı sayısı sorgulandı.");
            return 12;
        }

        public int AktifIsAkisiSayisi()
        {
            _logger.LogInformation("Aktif iş akışı sayısı sorgulandı.");
            return 5;
        }

        public int TamamlananIsAkisiSayisi()
        {
            _logger.LogInformation("Tamamlanan iş akışı sayısı sorgulandı.");
            return 7;
        }

        public string SistemDurumuOzeti()
        {
            _logger.LogInformation("Sistem durumu özeti oluşturuluyor.");
            return "Sistem aktif ve çalışıyor. Tüm servisler normal.";
        }
    }
}
