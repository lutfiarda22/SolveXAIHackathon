# FlowMind – Sistem Mimarisi

> **YZ Destekli İş Akışı Orkestrasyon Ajanı**
> Tekrarlayan manuel işleri en az %50 azaltmak.

---

## 1. Sistem Genel Bakış

FlowMind, **ASP.NET Core MVC** (.NET 9) üzerine kurulu YZ destekli iş akışı platformudur:

- **İş akışları tanımlama** — çok adımlı onay ve veri senkronizasyon süreçleri
- **Otomatik yürütme** — YZ Karar Motoru tetikleyicileri değerlendirir ve görevleri yönlendirir
- **İlerleme takibi** — gerçek zamanlı gösterge paneli
- **Sahte entegrasyonlar** — CRM, E-posta, Drive, Slack, ERP simülasyonu

Tüm veriler **SQL Server veritabanında** (Entity Framework Core ile) saklanır.

---

## 2. Mimari Diyagramı

```
┌────────────────────────────────────────────────────────────┐
│                     TARAYICI (Arayüz)                      │
│  Gösterge Paneli │ İş Akışı Oluşturucu │ Görev Kutusu     │
└──────┬───────────┴──────────┬───────────┴──────┬───────────┘
       │                      │                  │
       ▼                      ▼                  ▼
┌────────────────────────────────────────────────────────────┐
│                 ASP.NET Core MVC (.NET 9)                   │
│                                                            │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────────────┐  │
│  │ Controller   │  │ Görünümler   │  │ Statik Dosyalar  │  │
│  └──────┬──────┘  └──────────────┘  └──────────────────┘  │
│         │                                                  │
│  ┌──────▼────────────────────────────────────────────┐     │
│  │              SERVİS KATMANI                       │     │
│  │  İşAkışıServisi │ YZ Karar Motoru │ Entegrasyon   │     │
│  └──────────────────────────┬────────────────────────┘     │
│                             │                              │
│  ┌──────────────────────────▼────────────────────────┐     │
│  │           SQL SERVER VERİTABANI (EF Core)         │     │
│  │  İşAkışları[] Görevler[] Kayıtlar[] Kullanıcılar[]│     │
│  └───────────────────────────────────────────────────┘     │
└────────────────────────────────────────────────────────────┘
```

---

## 3. MVC Bileşenleri

### 3.1 Modeller (`/Models`)

| Model | Amaç |
|---|---|
| `Workflow` | İş akışı şablonu (ad, açıklama, adımlar, tetikleyici) |
| `WorkflowStep` | Tek bir adım (eylem türü, atanan kişi, koşullar, sıra) |
| `WorkflowInstance` | Çalışan iş akışı örneği (durum, mevcut adım, veri) |
| `TaskItem` | Kullanıcıya atanan görev (onayla/reddet/tamamla) |
| `User` | Kullanıcı modeli (ad, rol, departman) |
| `AuditLog` | Denetim kaydı (kim, ne, ne zaman) |
| `Notification` | Uygulama içi bildirim |
| `IntegrationLog` | Sahte API çağrı kayıtları |
| `AIDecision` | YZ kararları (bağlam, eylem, güven skoru) |

### 3.2 Controller'lar (`/Controllers`)

| Controller | Rotalar | Sorumluluk |
|---|---|---|
| `HomeController` | `/` | Gösterge Paneli |
| `WorkflowController` | `/Workflow/*` | İş akışı CRUD, başlat/durdur |
| `TaskController` | `/Task/*` | Görev kutusu, onayla/reddet |
| `IntegrationController` | `/Integration/*` | Entegrasyon durumu |
| `AuditController` | `/Audit/*` | Denetim günlüğü |
| `ApiController` | `/api/*` | AJAX için JSON uç noktaları |

### 3.3 Servisler (`/Services`)

| Servis | Sorumluluk |
|---|---|
| `WorkflowService` | İş akışı örneklerini oluştur, başlat, ilerlet, tamamla |
| `AIDecisionEngine` | Yönlendirme, önceliklendirme, otomatik onay kararları |
| `IntegrationService` | CRM, E-posta, Drive, Slack, ERP sahte bağlayıcıları |
| `NotificationService` | Uygulama içi bildirim oluştur ve ilet |
| `AuditService` | Denetim günlüğü kayıtları yaz |
| `FlowMindDbContext` | Merkezi, SQL Server tabanlı EF Core veritabanı bağlamı |

### 3.4 Görünümler (`/Views`)

| Klasör | Sayfalar |
|---|---|
| `Home/` | `Index.cshtml` — Ana gösterge paneli |
| `Workflow/` | `Index`, `Create`, `Details`, `Run` |
| `Task/` | `Index` (görev kutusu), `Details` |
| `Integration/` | `Index` (entegrasyon paneli) |
| `Audit/` | `Index` (denetim günlüğü) |
| `Shared/` | `_Layout`, `_Sidebar`, `Error` |

---

## 4. İş Akışı Yürütme Mantığı

```
 Kullanıcı İş Akışı Şablonu oluşturur
            │
            ▼
 İş Akışı Örneği başlatılır
            │
            ▼
 ┌──────────────────────────────────┐
 │  YZ Karar Motoru değerlendirir:  │
 │  • Otomatik onayla?              │
 │  • Kime yönlendir?               │
 │  • Yüksek öncelik mi?            │
 │  • Adımı atla?                   │
 └──────────┬───────────────────────┘
            │
            ▼
 Görev oluşturulur → kullanıcıya atanır
            │
            ▼
 Kullanıcı işlem yapar (Onayla / Reddet / Tamamla)
            │
            ├── ONAYLANDI → sonraki adıma ilerle → YZ tekrar değerlendirir
            ├── REDDEDİLDİ → iş akışı duraklatılır → başlatana bildirim
            └── SON ADIM → iş akışı TAMAMLANDI → özet bildirim
```

### YZ Karar Noktaları:

| Karar Noktası | YZ Mantığı |
|---|---|
| **Otomatik Onay** | Tutar < eşik VE geçmiş temiz → otomatik onayla |
| **Öncelik Yönlendirme** | Son tarih < 24 saat → üst onaylayıcıya yönlendir |
| **Adım Atlama** | Hedef sistemde veri mevcutsa → adımı atla |
| **Anomali Tespiti** | Normal kalıptan sapma → manuel inceleme |
| **Akıllı Atama** | En az yüklü onaylayıcıya ata |

---

## 5. Sahte Entegrasyon Mimarisi

| Bağlayıcı | Simülasyon |
|---|---|
| **SahteCRM** | Kayıt oluşturma ve sorgulama |
| **SahteEPosta** | Onay talebi e-postası gönderme |
| **SahteDrive** | Dosya yükleme ve bağlantı oluşturma |
| **SahteSlack** | Kanal bildirimi gönderme |
| **SahteERP** | Envanter güncelleme ve satın alma emri |

Her sahte bağlayıcı: gecikme simüle eder (50–200ms), gerçekçi yanıt döndürür, çağrıyı `IntegrationLog`'a kaydeder.

---

## 6. Veri Akışı Örneği: "Satın Alma Onayı"

```
1. Çalışan satın alma talebi gönderir (ürün, tutar, gerekçe, departman)
2. İşAkışıServisi örnek oluşturur: [Yönetici] → [Finans] → [CRM] → [ERP]
3. YZ değerlendirir: Tutar < ₺5.000 → OTOMATİK ONAYLA ✓
4. Finans adımı: Tutar > ₺2.000 → finans onayı gerekir → Görev oluşturulur
5. Finans onaylar → DenetimGünlüğü kaydı
6. SahteCRM.KayıtOlustur() → { basarili: true, crmId: "CRM-4821" }
7. SahteERP.SatınAlmaEmri() → { basarili: true, emirNo: "PO-2026-0312" }
8. İş akışı TAMAMLANDI → çalışana bildirim → denetim izi mevcut
```

---

## 7. Temel Tasarım İlkeleri

| İlke | Uygulama |
|---|---|
| **Kalıcılık** | SQL Server veritabanı, EF Core ORM kullanımı |
| **Sorumluluk Ayrımı** | Controller → Servis → Veri Deposu |
| **YZ Şeffaflığı** | Her YZ kararı gerekçesiyle kaydedilir |
| **Demo Hazırlığı** | Önceden yüklenmiş veriler, anında yanıt |
| **Genişletilebilirlik** | Sahte bağlayıcılar gerçek API'lerle değiştirilebilir |
