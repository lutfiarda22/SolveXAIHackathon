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
        private readonly AIDecisionEngine _aiDecisionEngine;
        private readonly MockIntegrationService _integrationService;

        public IsAkisiServisi(InMemoryDataStore store, ILogger<IsAkisiServisi> logger, AIDecisionEngine aiDecisionEngine, MockIntegrationService integrationService)
        {
            _store = store;
            _logger = logger;
            _aiDecisionEngine = aiDecisionEngine;
            _integrationService = integrationService;
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

        public FlowMind.Models.WorkflowInstance IsAkisiBaslat(string workflowId, string baslatanId, Dictionary<string, string> formVerisi)
        {
            var workflow = _store.Get(_store.Workflows, workflowId);
            if (workflow == null) throw new Exception("İş akışı bulunamadı");

            var instance = new FlowMind.Models.WorkflowInstance
            {
                Id = "WFI-" + new Random().Next(10000, 99999),
                WorkflowId = workflow.Id,
                WorkflowAdi = workflow.Ad,
                Durum = FlowMind.Models.WorkflowStatus.Calisıyor,
                MevcutAdim = 1,
                BaslatanId = baslatanId,
                BaslangicTarihi = DateTime.Now,
                FormVerisi = formVerisi
            };

            _store.WorkflowInstances.TryAdd(instance.Id, instance);
            _logger.LogInformation("İş akışı başlatıldı: {InstanceId} ({WorkflowAdi})", instance.Id, instance.WorkflowAdi);

            return instance;
        }

        public async Task<FlowMind.Models.AIDecision?> AdimiIslet(string instanceId)
        {
            var instance = _store.Get(_store.WorkflowInstances, instanceId);
            if (instance == null || instance.Durum != FlowMind.Models.WorkflowStatus.Calisıyor)
                return null;

            var workflow = _store.Get(_store.Workflows, instance.WorkflowId);
            if (workflow == null || workflow.Adimlar == null) return null;

            var currentStep = workflow.Adimlar.FirstOrDefault(s => s.Sira == instance.MevcutAdim);
            if (currentStep == null)
            {
                // Akış bitti
                instance.Durum = FlowMind.Models.WorkflowStatus.Tamamlandı;
                instance.BitisTarihi = DateTime.Now;
                return null;
            }

            // AI Karar Motoruna gönder
            // AI TRACEABILITY: İş akışı adımının durumuna göre YZ karar motoru (AIDecisionEngine) tetiklenir.
            // Karar sonucu (Onay/Ret/İnceleme/Atla) burada değerlendirilir. (Issue #16)
            var decision = _aiDecisionEngine.Degerlendir(instance, currentStep);

            // Karara göre işlem yap
            // AI TRACEABILITY: Kararın SuggestedAction enum'una göre sistemin bir sonraki adımı belirlenir.
            switch (decision.SuggestedAction)
            {
                case FlowMind.Models.AIAction.OtomatikOnayla:
                    _logger.LogInformation("Adım {Sira} otomatik onaylandı.", currentStep.Sira);
                    instance.MevcutAdim++;
                    if (currentStep.EylemTuru == FlowMind.Models.StepActionType.Entegrasyon)
                    {
                        if (currentStep.EntegrasyonTuru == FlowMind.Models.IntegrationType.CRM)
                            await _integrationService.CrmKayitOlustur(instance.Id, "Otomatik", "Kayıt");
                        else if (currentStep.EntegrasyonTuru == FlowMind.Models.IntegrationType.ERP)
                            await _integrationService.ErpSatinalmaEmri(instance.Id, "Otomatik", 100);
                        // vb.
                    }
                    instance.OtomatikTamamlanan++;
                    break;
                case FlowMind.Models.AIAction.ManuelInceleme:
                case FlowMind.Models.AIAction.OncelikliYonlendir:
                    _logger.LogInformation("Adım {Sira} manuel incelemeye yönlendirildi.", currentStep.Sira);
                    // Görev (Task) oluşturulacak (basit mock)
                    var task = new FlowMind.Models.TaskItem
                    {
                        Id = "TSK-" + new Random().Next(1000, 9999),
                        Baslik = currentStep.Ad + " İncelemesi",
                        Aciklama = decision.Reason,
                        WorkflowInstanceId = instance.Id,
                        StepId = currentStep.Id,
                        Durum = FlowMind.Models.TaskStatus.Atandı,
                        YZTarafindan = true,
                        OlusturmaTarihi = DateTime.Now
                    };
                    _store.Tasks.TryAdd(task.Id, task);
                    // Beklemeye alıyoruz
                    instance.Durum = FlowMind.Models.WorkflowStatus.Beklemede;
                    break;
                case FlowMind.Models.AIAction.AdimAtla:
                    _logger.LogInformation("Adım {Sira} atlandı.", currentStep.Sira);
                    instance.MevcutAdim++;
                    break;
                case FlowMind.Models.AIAction.Reddet:
                    instance.Durum = FlowMind.Models.WorkflowStatus.İptalEdildi;
                    instance.BitisTarihi = DateTime.Now;
                    break;
            }

            return decision;
        }
    }
}
