namespace FlowMind.Models
{
    /// <summary>
    /// Ana panel (dashboard) sayfasının ViewModel'i.
    /// </summary>
    public class PanelViewModel
    {
        /// <summary>
        /// Toplam iş akışı sayısı
        /// </summary>
        public int ToplamIsAkisi { get; set; }

        /// <summary>
        /// Aktif iş akışı sayısı
        /// </summary>
        public int AktifIsAkisi { get; set; }

        /// <summary>
        /// Tamamlanan iş akışı sayısı
        /// </summary>
        public int TamamlananIsAkisi { get; set; }

        /// <summary>
        /// Sistem durumu özet mesajı
        /// </summary>
        public string SistemDurumu { get; set; } = string.Empty;

        /// <summary>
        /// Hoş geldiniz mesajı
        /// </summary>
        public string HosgeldinMesaji { get; set; } = "FlowMind AI Orkestratör'e Hoş Geldiniz!";
        /// <summary>
        /// AI tarafından otomatik yönetilen süreçlerin oranı (%)
        /// </summary>
        public int AIOtomasyonOrani { get; set; }

        /// <summary>
        /// Toplam işlem (task/step) sayısı
        /// </summary>
        public int ToplamIslemSayisi { get; set; }

        /// <summary>
        /// Son aktiviteler (Activity Feed)
        /// </summary>
        public List<ActivityItem> Aktiviteler { get; set; } = new List<ActivityItem>();
    }

    public class ActivityItem
    {
        public string Baslik { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public DateTime Tarih { get; set; }
        public string Ikon { get; set; } = "📝";
        public string Tur { get; set; } = "info"; // success, warning, danger, info
    }
}
