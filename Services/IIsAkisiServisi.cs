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
    }
}
