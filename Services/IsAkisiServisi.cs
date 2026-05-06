namespace FlowMind.Services
{
    /// <summary>
    /// İş akışı yönetim servisinin varsayılan implementasyonu.
    /// Şimdilik in-memory mock veriler kullanır, ileride veritabanına bağlanacak.
    /// </summary>
    public class IsAkisiServisi : IIsAkisiServisi
    {
        private readonly InMemoryDataStore _store;
        private readonly ILogger<IsAkisiServisi> _logger;

        public IsAkisiServisi(InMemoryDataStore store, ILogger<IsAkisiServisi> logger)
        {
            _store = store;
            _logger = logger;
        }

        public int ToplamIsAkisiSayisi()
        {
            return _store.Workflows.Count;
        }

        public int AktifIsAkisiSayisi()
        {
            return _store.WorkflowInstances.Values.Count(i => i.Durum == Models.WorkflowStatus.Calisıyor);
        }

        public int TamamlananIsAkisiSayisi()
        {
            return _store.WorkflowInstances.Values.Count(i => i.Durum == Models.WorkflowStatus.Tamamlandı);
        }

        public string SistemDurumuOzeti()
        {
            _logger.LogInformation("Sistem durumu özeti oluşturuluyor.");
            return "Sistem aktif ve çalışıyor. Tüm servisler normal.";
        }

        public List<FlowMind.Models.Workflow> TumIsAkislari()
        {
            return _store.GetAll(_store.Workflows).OrderByDescending(w => w.OlusturmaTarihi).ToList();
        }

        public FlowMind.Models.Workflow? IsAkisiGetir(string id)
        {
            return _store.Get(_store.Workflows, id);
        }

        public FlowMind.Models.Workflow IsAkisiOlustur(FlowMind.Models.Workflow workflow)
        {
            workflow.Id = "WF-" + new Random().Next(1000, 9999);
            workflow.OlusturmaTarihi = DateTime.Now;
            
            // Adımlara benzersiz ID ata
            if (workflow.Adimlar != null)
            {
                for (int i = 0; i < workflow.Adimlar.Count; i++)
                {
                    workflow.Adimlar[i].Id = $"STP-{Guid.NewGuid().ToString("N")[..6]}";
                    workflow.Adimlar[i].Sira = i + 1;
                }
            }

            _store.Workflows.TryAdd(workflow.Id, workflow);
            _logger.LogInformation("Yeni iş akışı oluşturuldu: {Id} - {Ad}", workflow.Id, workflow.Ad);
            
            return workflow;
        }
    }
}
