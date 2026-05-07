using System.Text.Json;
using FlowMind.Models;
using Microsoft.EntityFrameworkCore;
using TaskStatus = FlowMind.Models.TaskStatus;

namespace FlowMind.Data;

public class FlowMindDbContext : DbContext
{
    public FlowMindDbContext(DbContextOptions<FlowMindDbContext> options) : base(options)
    {
    }

    public DbSet<Workflow> Workflows { get; set; }
    public DbSet<WorkflowStep> WorkflowSteps { get; set; }
    public DbSet<WorkflowInstance> WorkflowInstances { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<IntegrationLog> IntegrationLogs { get; set; }
    public DbSet<AIDecision> AIDecisions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Model Configurations ---

        // Workflow
        modelBuilder.Entity<Workflow>(entity =>
        {
            entity.HasMany(w => w.Adimlar)
                  .WithOne()
                  .HasForeignKey(s => s.WorkflowId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(w => w.FormVerisi)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                      v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null) ?? new Dictionary<string, string>()
                  );
        });

        // WorkflowInstance
        modelBuilder.Entity<WorkflowInstance>(entity =>
        {
            entity.Property(w => w.FormVerisi)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                      v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null) ?? new Dictionary<string, string>()
                  );

            entity.Property(w => w.YZKararIds)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                      v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
                  );
        });

        // --- Seed Data ---

        // Users
        var users = new[]
        {
            new User { Id = "USR-001", Ad = "Ahmet", Soyad = "Yılmaz", EPosta = "ahmet@flowmind.com", Rol = UserRole.Calisan, Departman = "Bilişim", KayitTarihi = DateTime.Now },
            new User { Id = "USR-002", Ad = "Lütfi", Soyad = "Arda", EPosta = "lutfi@flowmind.com", Rol = UserRole.Yonetici, Departman = "Yönetici", KayitTarihi = DateTime.Now },
            new User { Id = "USR-003", Ad = "Esma", Soyad = "Mol", EPosta = "esma@flowmind.com", Rol = UserRole.Finans, Departman = "Finans", KayitTarihi = DateTime.Now },
            new User { Id = "USR-004", Ad = "Sinem", Soyad = "Doğan", EPosta = "sinem@flowmind.com", Rol = UserRole.Admin, Departman = "İnsan Kaynakları", KayitTarihi = DateTime.Now },
            new User { Id = "USR-005", Ad = "Ali", Soyad = "Öztürk", EPosta = "ali@flowmind.com", Rol = UserRole.Calisan, Departman = "Satış", KayitTarihi = DateTime.Now }
        };
        modelBuilder.Entity<User>().HasData(users);

        // Workflows
        var wf1Id = "WF-001";
        var wf2Id = "WF-002";

        var workflows = new[]
        {
            new Workflow { Id = wf1Id, Ad = "Satınalma Onay Süreci", Aciklama = "Belirli tutar üzerindeki satınalmalar için çok adımlı onay süreci", Tetikleyici = "Satınalma talebi oluşturulduğunda", OlusturanId = "USR-004", Aktif = true, OlusturmaTarihi = DateTime.Now, FormVerisi = new Dictionary<string, string>() },
            new Workflow { Id = wf2Id, Ad = "İşe Alım Süreci", Aciklama = "Yeni personel işe alım ve onboarding adımları", Tetikleyici = "Aday onaylandığında", OlusturanId = "USR-004", Aktif = true, OlusturmaTarihi = DateTime.Now, FormVerisi = new Dictionary<string, string>() }
        };
        modelBuilder.Entity<Workflow>().HasData(workflows);

        // WorkflowSteps
        var steps = new[]
        {
            new WorkflowStep { Id = "STP-001", WorkflowId = wf1Id, Ad = "Yönetici Onayı", Sira = 1, EylemTuru = StepActionType.Onay, AtananKisi = "Yonetici", YZAtlayabilir = true, Aciklama = "", Kosul = "" },
            new WorkflowStep { Id = "STP-002", WorkflowId = wf1Id, Ad = "Finans Kontrolü", Sira = 2, EylemTuru = StepActionType.Onay, AtananKisi = "Finans", Kosul = "Tutar > 5000", Aciklama = "" },
            new WorkflowStep { Id = "STP-003", WorkflowId = wf1Id, Ad = "CRM Kaydı", Sira = 3, EylemTuru = StepActionType.Entegrasyon, EntegrasyonTuru = IntegrationType.CRM, Aciklama = "", AtananKisi = "", Kosul = "" },
            new WorkflowStep { Id = "STP-004", WorkflowId = wf1Id, Ad = "ERP Sipariş", Sira = 4, EylemTuru = StepActionType.Entegrasyon, EntegrasyonTuru = IntegrationType.ERP, Aciklama = "", AtananKisi = "", Kosul = "" },
            new WorkflowStep { Id = "STP-005", WorkflowId = wf1Id, Ad = "Bildirim Gönder", Sira = 5, EylemTuru = StepActionType.Bildirim, Aciklama = "", AtananKisi = "", Kosul = "" },
            
            new WorkflowStep { Id = "STP-011", WorkflowId = wf2Id, Ad = "İK Değerlendirme", Sira = 1, EylemTuru = StepActionType.Onay, AtananKisi = "Yonetici", Aciklama = "", Kosul = "" },
            new WorkflowStep { Id = "STP-012", WorkflowId = wf2Id, Ad = "Sözleşme Gönder", Sira = 2, EylemTuru = StepActionType.Entegrasyon, EntegrasyonTuru = IntegrationType.EPosta, Aciklama = "", AtananKisi = "", Kosul = "" },
            new WorkflowStep { Id = "STP-013", WorkflowId = wf2Id, Ad = "IT Ekipman Talebi", Sira = 3, EylemTuru = StepActionType.Entegrasyon, EntegrasyonTuru = IntegrationType.ERP, Aciklama = "", AtananKisi = "", Kosul = "" },
            new WorkflowStep { Id = "STP-014", WorkflowId = wf2Id, Ad = "Slack Duyurusu", Sira = 4, EylemTuru = StepActionType.Entegrasyon, EntegrasyonTuru = IntegrationType.Slack, Aciklama = "", AtananKisi = "", Kosul = "" }
        };
        modelBuilder.Entity<WorkflowStep>().HasData(steps);

        // WorkflowInstances
        var instances = new[]
        {
            new WorkflowInstance
            {
                Id = "WFI-001", WorkflowId = "WF-001", WorkflowAdi = "Satınalma Onay Süreci",
                Durum = WorkflowStatus.Beklemede, MevcutAdim = 2, BaslatanId = "USR-001",
                FormVerisi = new Dictionary<string, string>
                {
                    { "Ürün", "MacBook Pro M3 Max" }, { "Tutar", "120000" }, { "Departman", "Bilişim" }, { "Gerekçe", "Geliştirici iş istasyonu" }
                },
                BaslangicTarihi = DateTime.Now.AddHours(-3), YZKararIds = new List<string>()
            },
            new WorkflowInstance
            {
                Id = "WFI-002", WorkflowId = "WF-001", WorkflowAdi = "Satınalma Onay Süreci",
                Durum = WorkflowStatus.Tamamlandı, MevcutAdim = 5, BaslatanId = "USR-005",
                FormVerisi = new Dictionary<string, string>
                {
                    { "Ürün", "Ofis Malzemesi Paketi" }, { "Tutar", "2500" }, { "Departman", "Satış" }, { "Gerekçe", "Aylık rutin sipariş" }
                },
                BaslangicTarihi = DateTime.Now.AddDays(-1), BitisTarihi = DateTime.Now.AddHours(-20),
                OtomatikTamamlanan = 3, YZKararIds = new List<string>()
            },
            new WorkflowInstance
            {
                Id = "WFI-003", WorkflowId = "WF-002", WorkflowAdi = "İşe Alım Süreci",
                Durum = WorkflowStatus.Calisıyor, MevcutAdim = 2, BaslatanId = "USR-004",
                FormVerisi = new Dictionary<string, string>
                {
                    { "AdayAd", "Zeynep Arslan" }, { "Pozisyon", "Backend Developer" }, { "Departman", "Bilişim" }
                },
                BaslangicTarihi = DateTime.Now.AddHours(-5), YZKararIds = new List<string>()
            }
        };
        modelBuilder.Entity<WorkflowInstance>().HasData(instances);

        // Tasks
        var tasks = new[]
        {
            new TaskItem
            {
                Id = "TSK-001", Baslik = "Yüksek Tutarlı Satınalma Onayı",
                Aciklama = "MacBook Pro M3 Max donanım talebi onayınızı bekliyor. Tutar: ₺120.000",
                WorkflowInstanceId = "WFI-001", StepId = "STP-002",
                AtananKisiId = "USR-003", AtananKisiAd = "Esma Mol",
                Durum = TaskStatus.Atandı, Oncelik = 5, YZTarafindan = true,
                OlusturmaTarihi = DateTime.Now.AddHours(-2)
            },
            new TaskItem
            {
                Id = "TSK-002", Baslik = "Sözleşme İncelemesi",
                Aciklama = "Yeni tedarikçi sözleşmesinin hukuk departmanı tarafından incelenmesi.",
                WorkflowInstanceId = "WFI-001", StepId = "STP-001",
                AtananKisiId = "USR-002", AtananKisiAd = "Lütfi Arda",
                Durum = TaskStatus.Atandı, Oncelik = 3,
                OlusturmaTarihi = DateTime.Now.AddDays(-1),
                SonTarih = DateTime.Now.AddHours(-5) // süresi geçmiş
            },
            new TaskItem
            {
                Id = "TSK-003", Baslik = "İzin Talebi Onayı",
                Aciklama = "Ali Öztürk — Yıllık izin talebi (5 gün)",
                WorkflowInstanceId = "WFI-002", StepId = "STP-001",
                AtananKisiId = "USR-002", AtananKisiAd = "Lütfi Arda",
                Durum = TaskStatus.Onaylandı, Oncelik = 2, YZTarafindan = true,
                OlusturmaTarihi = DateTime.Now.AddDays(-3), TamamlanmaTarihi = DateTime.Now.AddDays(-2)
            }
        };
        modelBuilder.Entity<TaskItem>().HasData(tasks);

        // IntegrationLogs
        var intLogs = new[]
        {
            new IntegrationLog { Id = "INT-001", WorkflowInstanceId = "WFI-002", HedefSistem = IntegrationType.CRM, Islem = "KayıtOluştur", GonderilenVeri = "{\"Ad\":\"Ofis Malzemesi\"}", Yanit = "{\"CrmId\":\"CRM-4821\",\"Status\":\"Created\"}", Durum = IntegrationStatus.Basarili, SureMs = 145, Zaman = DateTime.Now.AddHours(-20) },
            new IntegrationLog { Id = "INT-002", WorkflowInstanceId = "WFI-002", HedefSistem = IntegrationType.ERP, Islem = "SatınAlmaEmri", GonderilenVeri = "{\"Ürün\":\"Ofis Malzemesi\",\"Tutar\":2500}", Yanit = "{\"EmirNo\":\"PO-2026-0312\",\"Status\":\"Created\"}", Durum = IntegrationStatus.Basarili, SureMs = 198, Zaman = DateTime.Now.AddHours(-20) },
            new IntegrationLog { Id = "INT-003", WorkflowInstanceId = "WFI-003", HedefSistem = IntegrationType.EPosta, Islem = "SözleşmeGönder", GonderilenVeri = "{\"To\":\"zeynep@email.com\",\"Subject\":\"İş Teklifi\"}", Yanit = "{\"MessageId\":\"msg_01\",\"Status\":\"Sent\"}", Durum = IntegrationStatus.Basarili, SureMs = 82, Zaman = DateTime.Now.AddHours(-4) }
        };
        modelBuilder.Entity<IntegrationLog>().HasData(intLogs);

        // AIDecisions
        var aiDecisions = new[]
        {
            new AIDecision
            {
                Id = "AI-001", WorkflowInstanceId = "WFI-002", StepId = "STP-001",
                SuggestedAction = AIAction.OtomatikOnayla, ConfidenceScore = 0.95,
                Reason = "Tutar (₺2.500) eşik değerin altında ve talep eden kişinin geçmişi temiz. Otomatik onay verildi.",
                Baglan = "Tutar: 2500, Departman: Satış, Geçmiş Red: 0",
                IslemSuresiMs = 12, KararZamani = DateTime.Now
            },
            new AIDecision
            {
                Id = "AI-002", WorkflowInstanceId = "WFI-001", StepId = "STP-001",
                SuggestedAction = AIAction.ManuelInceleme, ConfidenceScore = 0.40,
                Reason = "Tutar (₺120.000) çok yüksek. Otomatik onay verilemez, yönetici incelemesi gerekli.",
                Baglan = "Tutar: 120000, Departman: Bilişim, Risk: Yüksek",
                IslemSuresiMs = 8, KararZamani = DateTime.Now
            },
            new AIDecision
            {
                Id = "AI-003", WorkflowInstanceId = "WFI-003", StepId = "STP-011",
                SuggestedAction = AIAction.OtomatikOnayla, ConfidenceScore = 0.88,
                Reason = "Pozisyon açık ve bütçe uygun. İK değerlendirmesi otomatik olarak tamamlandı.",
                Baglan = "Pozisyon: Backend Developer, Bütçe Durumu: Uygun",
                IslemSuresiMs = 15, KararZamani = DateTime.Now
            }
        };
        modelBuilder.Entity<AIDecision>().HasData(aiDecisions);

        // AuditLogs
        var auditLogs = new[]
        {
            new AuditLog { Id = "AUD-001", KullaniciId = "YZ", KullaniciAd = "AI Karar Motoru", Eylem = "Otomatik onay verildi", HedefTur = "WorkflowInstance", HedefId = "WFI-002", Detay = "Tutar eşik altında, güven skoru: 0.95", YZTetikledi = true, Zaman = DateTime.Now.AddDays(-1) },
            new AuditLog { Id = "AUD-002", KullaniciId = "YZ", KullaniciAd = "AI Karar Motoru", Eylem = "Manuel incelemeye yönlendirildi", HedefTur = "WorkflowInstance", HedefId = "WFI-001", Detay = "Yüksek tutar tespit edildi, güven skoru: 0.40", YZTetikledi = true, Zaman = DateTime.Now.AddHours(-3) },
            new AuditLog { Id = "AUD-003", KullaniciId = "USR-001", KullaniciAd = "Ahmet Yılmaz", Eylem = "Satınalma talebi oluşturdu", HedefTur = "WorkflowInstance", HedefId = "WFI-001", Detay = "", YZTetikledi = false, Zaman = DateTime.Now.AddHours(-3) }
        };
        modelBuilder.Entity<AuditLog>().HasData(auditLogs);

        // Notifications
        var notifications = new[]
        {
            new Notification { Id = "NOT-001", KullaniciId = "USR-003", Baslik = "Yeni Görev Atandı", Mesaj = "MacBook Pro M3 Max satınalma talebi onayınızı bekliyor.", Tur = NotificationType.GorevAtandi, OlusturmaTarihi = DateTime.Now },
            new Notification { Id = "NOT-002", KullaniciId = "USR-005", Baslik = "İş Akışı Tamamlandı", Mesaj = "Ofis Malzemesi Paketi siparişi başarıyla tamamlandı.", Tur = NotificationType.IsAkisiTamamlandi, Okundu = true, OlusturmaTarihi = DateTime.Now },
            new Notification { Id = "NOT-003", KullaniciId = "USR-001", Baslik = "AI Kararı Verildi", Mesaj = "Satınalma talebiniz yüksek tutar nedeniyle manuel incelemeye yönlendirildi.", Tur = NotificationType.YZKarariVerildi, OlusturmaTarihi = DateTime.Now }
        };
        modelBuilder.Entity<Notification>().HasData(notifications);
    }
}
