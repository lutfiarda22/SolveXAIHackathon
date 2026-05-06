namespace FlowMind.Services
{
    /// <summary>
    /// İş akışı yönetim servisi arayüzü.
    /// Tüm iş akışı operasyonları bu arayüz üzerinden gerçekleştirilir.
    /// </summary>
    public interface IIsAkisiServisi
    {
        /// <summary>
        /// Sistemdeki toplam iş akışı sayısını döndürür.
        /// </summary>
        int ToplamIsAkisiSayisi();

        /// <summary>
        /// Aktif (devam eden) iş akışı sayısını döndürür.
        /// </summary>
        int AktifIsAkisiSayisi();

        /// <summary>
        /// Tamamlanan iş akışı sayısını döndürür.
        /// </summary>
        int TamamlananIsAkisiSayisi();

        /// <summary>
        /// Sistem durumu özetini döndürür.
        /// </summary>
        string SistemDurumuOzeti();

        /// <summary>Tüm iş akışlarını listeler</summary>
        List<FlowMind.Models.Workflow> TumIsAkislari();

        /// <summary>ID'ye göre iş akışını getirir</summary>
        FlowMind.Models.Workflow? IsAkisiGetir(string id);

        /// <summary>Yeni iş akışı oluşturur</summary>
        FlowMind.Models.Workflow IsAkisiOlustur(FlowMind.Models.Workflow workflow);
    }
}
