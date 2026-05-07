using FlowMind.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowMind.Services
{
    /// <summary>
    /// İş akışı yönetim servisinin varsayılan implementasyonu.
    /// Şimdilik in-memory mock veriler kullanır, ileride veritabanına bağlanacak.
    /// </summary>
    public class IsAkisiServisi : IIsAkisiServisi
    {
        private readonly FlowMindDbContext _context;
        private readonly ILogger<IsAkisiServisi> _logger;
        private readonly AIDecisionEngine _aiDecisionEngine;
        private readonly MockIntegrationService _integrationService;
        private readonly IEmailService _emailService;

        public IsAkisiServisi(FlowMindDbContext context, ILogger<IsAkisiServisi> logger, AIDecisionEngine aiDecisionEngine, MockIntegrationService integrationService, IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _aiDecisionEngine = aiDecisionEngine;
            _integrationService = integrationService;
            _emailService = emailService;
        }

        public int ToplamIsAkisiSayisi()
        {
            return _context.Workflows.Count();
        }

        public int AktifIsAkisiSayisi()
        {
            return _context.WorkflowInstances.Count(i => i.Durum == Models.WorkflowStatus.Calisıyor);
        }

        public int TamamlananIsAkisiSayisi()
        {
            return _context.WorkflowInstances.Count(i => i.Durum == Models.WorkflowStatus.Tamamlandı);
        }

        public string SistemDurumuOzeti()
        {
            _logger.LogInformation("Sistem durumu özeti oluşturuluyor.");
            return "Sistem aktif ve çalışıyor. Tüm servisler normal.";
        }

        public List<FlowMind.Models.Workflow> TumIsAkislari()
        {
            return _context.Workflows.Include(w => w.Adimlar).OrderByDescending(w => w.OlusturmaTarihi).ToList();
        }

        public FlowMind.Models.Workflow? IsAkisiGetir(string id)
        {
            return _context.Workflows.Include(w => w.Adimlar).FirstOrDefault(w => w.Id == id);
        }

        public FlowMind.Models.Workflow IsAkisiOlustur(FlowMind.Models.Workflow workflow)
        {
            workflow.Id = "WF-" + new Random().Next(1000, 9999);
            workflow.OlusturmaTarihi = DateTime.Now;

            if (workflow.Adimlar == null) workflow.Adimlar = new List<FlowMind.Models.WorkflowStep>();

            // --- Otomatik Adım Oluşturma Mantığı ---
            var konu = workflow.FormVerisi?.GetValueOrDefault("Konu", "");
            var departman = workflow.FormVerisi?.GetValueOrDefault("Departman", "");
            
            int sira = 1;

            if (departman == "Hukuk" || departman == "Yönetici")
            {
                workflow.Adimlar.Add(new FlowMind.Models.WorkflowStep { 
                    Id = $"STP-{Guid.NewGuid().ToString("N")[..6]}", 
                    Ad = departman == "Hukuk" ? "Hukuksal İnceleme ve Onay" : "Yönetici Onayı", 
                    Sira = sira++, 
                    EylemTuru = FlowMind.Models.StepActionType.Onay, 
                    AtananKisi = "Yonetici",
                    Aciklama = departman == "Hukuk" ? "Hukuk departmanı talebi doğrudan yönetici onayına sunulur." : "Yönetici departmanı talepleri doğrudan onay sürecine girer."
                });
            }
            else if (konu == "Satınalma" || konu == "Sipariş")
            {
                workflow.Adimlar.Add(new FlowMind.Models.WorkflowStep { 
                    Id = $"STP-{Guid.NewGuid().ToString("N")[..6]}", 
                    Ad = "YZ Risk Analizi ve Karar", 
                    Sira = sira++, 
                    EylemTuru = FlowMind.Models.StepActionType.YZKarar, 
                    Aciklama = "Yapay zeka departman ve tutara göre risk değerlendirmesi yapar." 
                });

                workflow.Adimlar.Add(new FlowMind.Models.WorkflowStep { 
                    Id = $"STP-{Guid.NewGuid().ToString("N")[..6]}", 
                    Ad = "ERP Sipariş Kaydı", 
                    Sira = sira++, 
                    EylemTuru = FlowMind.Models.StepActionType.Entegrasyon, 
                    EntegrasyonTuru = FlowMind.Models.IntegrationType.ERP,
                    Aciklama = "Onay sonrası sipariş otomatik ERP sistemine işlenir." 
                });
            }
            else if (konu == "İzin")
            {
                workflow.Adimlar.Add(new FlowMind.Models.WorkflowStep { 
                    Id = $"STP-{Guid.NewGuid().ToString("N")[..6]}", 
                    Ad = "Yönetici Onayı", 
                    Sira = sira++, 
                    EylemTuru = FlowMind.Models.StepActionType.Onay, 
                    AtananKisi = "Yonetici",
                    Aciklama = "İzin talebi için yönetici onayı." 
                });

                workflow.Adimlar.Add(new FlowMind.Models.WorkflowStep { 
                    Id = $"STP-{Guid.NewGuid().ToString("N")[..6]}", 
                    Ad = "İK Kaydı ve Bildirim", 
                    Sira = sira++, 
                    EylemTuru = FlowMind.Models.StepActionType.Entegrasyon, 
                    EntegrasyonTuru = FlowMind.Models.IntegrationType.EPosta,
                    Aciklama = "Onaylanan iznin İK sistemine ve çalışana iletilmesi." 
                });
            }
            else if (konu == "Fatura")
            {
                workflow.Adimlar.Add(new FlowMind.Models.WorkflowStep { 
                    Id = $"STP-{Guid.NewGuid().ToString("N")[..6]}", 
                    Ad = "Finans Onayı", 
                    Sira = sira++, 
                    EylemTuru = FlowMind.Models.StepActionType.Onay, 
                    AtananKisi = "Finans",
                    Aciklama = "Fatura için finans departmanının ön onayı." 
                });

                workflow.Adimlar.Add(new FlowMind.Models.WorkflowStep { 
                    Id = $"STP-{Guid.NewGuid().ToString("N")[..6]}", 
                    Ad = "Ödeme Planlama", 
                    Sira = sira++, 
                    EylemTuru = FlowMind.Models.StepActionType.YZKarar, 
                    Aciklama = "YZ tarafından vade analizinin yapılması." 
                });
            }
            else 
            {
                // Default
                workflow.Adimlar.Add(new FlowMind.Models.WorkflowStep { 
                    Id = $"STP-{Guid.NewGuid().ToString("N")[..6]}", 
                    Ad = "Genel Değerlendirme (YZ)", 
                    Sira = sira++, 
                    EylemTuru = FlowMind.Models.StepActionType.YZKarar, 
                    Aciklama = "Talebin yapay zeka tarafından genel değerlendirilmesi." 
                });
            }

            _context.Workflows.Add(workflow);
            _context.SaveChanges();
            _logger.LogInformation("Yeni iş akışı oluşturuldu (Otomatik Adımlı): {Id} - {Ad}", workflow.Id, workflow.Ad);

            return workflow;
        }

        public FlowMind.Models.WorkflowInstance IsAkisiBaslat(string workflowId, string baslatanId, Dictionary<string, string> formVerisi)
        {
            var workflow = _context.Workflows.FirstOrDefault(w => w.Id == workflowId);
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

            _context.WorkflowInstances.Add(instance);
            _context.SaveChanges();
            _logger.LogInformation("İş akışı başlatıldı: {InstanceId} ({WorkflowAdi})", instance.Id, instance.WorkflowAdi);

            return instance;
        }

        public void AdimEkle(string workflowId, FlowMind.Models.WorkflowStep adim)
        {
            var workflow = _context.Workflows.Include(w => w.Adimlar).FirstOrDefault(w => w.Id == workflowId);
            if (workflow != null)
            {
                if (workflow.Adimlar == null) workflow.Adimlar = new List<FlowMind.Models.WorkflowStep>();
                adim.Id = $"STP-{Guid.NewGuid().ToString("N")[..6]}";
                adim.Sira = workflow.Adimlar.Count + 1; // En sona ekle
                workflow.Adimlar.Add(adim);
                _context.SaveChanges();
                _logger.LogInformation("İş akışına yeni adım eklendi: {WorkflowId} -> {AdimAd}", workflowId, adim.Ad);
            }
        }

        public async Task<FlowMind.Models.AIDecision?> AdimiIslet(string instanceId)
        {
            var instance = _context.WorkflowInstances.FirstOrDefault(i => i.Id == instanceId);
            if (instance == null || instance.Durum != FlowMind.Models.WorkflowStatus.Calisıyor)
                return null;

            var workflow = _context.Workflows.Include(w => w.Adimlar).FirstOrDefault(w => w.Id == instance.WorkflowId);
            if (workflow == null || workflow.Adimlar == null) return null;

            var currentStep = workflow.Adimlar.FirstOrDefault(s => s.Sira == instance.MevcutAdim);
            if (currentStep == null)
            {
                // Akış bitti
                instance.Durum = FlowMind.Models.WorkflowStatus.Tamamlandı;
                instance.BitisTarihi = DateTime.Now;

                // E-posta gönder
                var baslatanUser = _context.Users.FirstOrDefault(u => u.Id == instance.BaslatanId);
                if (baslatanUser != null)
                {
                    await _emailService.SendEmailAsync(
                        baslatanUser.EPosta,
                        $"İş Akışı Tamamlandı: {instance.WorkflowAdi}",
                        $"Merhaba {baslatanUser.Ad},\n\nBaşlatmış olduğunuz '{instance.WorkflowAdi}' adlı iş akışı başarıyla tamamlanmıştır.",
                        instance.Id
                    );
                }

                await _context.SaveChangesAsync();
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
                    }
                    instance.OtomatikTamamlanan++;

                    // Kalan adım var mı kontrol et — yoksa Tamamlandı yap
                    if (!workflow.Adimlar.Any(a => a.Sira >= instance.MevcutAdim))
                    {
                        instance.Durum = FlowMind.Models.WorkflowStatus.Tamamlandı;
                        instance.BitisTarihi = DateTime.Now;
                    }
                    break;
                case FlowMind.Models.AIAction.ManuelInceleme:
                case FlowMind.Models.AIAction.OncelikliYonlendir:
                    _logger.LogInformation("Adım {Sira} manuel incelemeye yönlendirildi.", currentStep.Sira);
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

                    string atananKisiId = currentStep.AtananKisi == "Yonetici" ? "USR-002" :
                                          currentStep.AtananKisi == "Finans" ? "USR-003" : "USR-004";

                    var departman = instance.FormVerisi.GetValueOrDefault("Departman", "");
                    var konu = instance.FormVerisi.GetValueOrDefault("Konu", "");

                    if (departman == "Hukuk" || departman == "Yönetici")
                    {
                        atananKisiId = "USR-002"; // Doğrudan Yöneticiye (Lütfi Arda)
                    }
                    else if (konu == "İzin") 
                    {
                        atananKisiId = "USR-004";
                    }
                    else if (konu == "İade") 
                    {
                        atananKisiId = "USR-003";
                    }

                    task.AtananKisiId = atananKisiId;

                    var atananKisiUser = _context.Users.FirstOrDefault(u => u.Id == atananKisiId);
                    if (atananKisiUser != null)
                        task.AtananKisiAd = atananKisiUser.TamAd;

                    _context.Tasks.Add(task);

                    var atananUser = _context.Users.FirstOrDefault(u => u.Id == atananKisiId);
                    if (atananUser != null)
                    {
                        await _emailService.SendEmailAsync(
                            atananUser.EPosta,
                            $"Yeni Onay Görevi: {task.Baslik}",
                            $"Merhaba {atananUser.Ad},\n\nSize atanmış yeni bir onay görevi bulunmaktadır.\n\nİlgili Akış: {instance.WorkflowAdi}\nDetay: {task.Aciklama}",
                            instance.Id
                        );
                    }

                    instance.Durum = FlowMind.Models.WorkflowStatus.Beklemede;
                    break;
                case FlowMind.Models.AIAction.AdimAtla:
                    _logger.LogInformation("Adım {Sira} atlandı.", currentStep.Sira);
                    instance.MevcutAdim++;
                    if (!workflow.Adimlar.Any(a => a.Sira >= instance.MevcutAdim))
                    {
                        instance.Durum = FlowMind.Models.WorkflowStatus.Tamamlandı;
                        instance.BitisTarihi = DateTime.Now;
                    }
                    break;
                case FlowMind.Models.AIAction.Reddet:
                    instance.Durum = FlowMind.Models.WorkflowStatus.İptalEdildi;
                    instance.BitisTarihi = DateTime.Now;
                    break;
            }

            await _context.SaveChangesAsync();
            return decision;
        }
    }
}
