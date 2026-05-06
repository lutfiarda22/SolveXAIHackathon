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
    }
}
