namespace FlowMind.Services
{
    /// <summary>
    /// AI Karar Motoru arayüzü.
    /// İş akışlarını analiz eder ve yönlendirme kararları verir.
    /// </summary>
    public interface IKararMotoru
    {
        /// <summary>
        /// Verilen iş akışı için bir karar üretir.
        /// </summary>
        /// <param name="isAkisiTuru">İş akışının türü (örn: "siparis", "destek", "onay")</param>
        /// <returns>Karar sonucu açıklaması</returns>
        string KararUret(string isAkisiTuru);

        /// <summary>
        /// İş akışını uygun servise yönlendirir.
        /// </summary>
        /// <param name="isAkisiTuru">İş akışının türü</param>
        /// <returns>Yönlendirilecek servis adı</returns>
        string Yonlendir(string isAkisiTuru);
    }
}
