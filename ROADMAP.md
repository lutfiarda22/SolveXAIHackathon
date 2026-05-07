# FlowMind - 5 Saatlik Hackathon Yol Haritası 🚀

## Hedef
Yapay zeka odaklı iş akışı orkestrasyonunu; sahte entegrasyonlar, SQL Server veritabanı ve Entity Framework Core ile gösteren, yerel olarak çalışan bir ASP.NET Core MVC (.NET 9) web uygulaması teslim etmek.

## Ekip Yapısı
- **Esma (esmamol):** YZ Karar Motoru, servis katmanı ve temel modeller.
- **Lütfi (lutfiarda22):** Backend servisleri, YZ entegrasyonları, mock servisler.
- **Sinem (snmmdogann):** Görünümler (Views), Layout, Dashboard arayüzü, CSS/JS.

---

## 1. Saat: Proje Kurulumu ve Temel Altyapı
**Durum:** Tamamlandı ✅

- [x] ASP.NET Core MVC (.NET 9) projesini oluşturun (`dotnet new mvc`)
- [x] Tüm modelleri oluşturun: `Workflow`, `WorkflowStep`, `WorkflowInstance`, `TaskItem`, `User`, `AuditLog`, `Notification`, `IntegrationLog`, `AIDecision`
- [x] `FlowMindDbContext` bağlamını oluşturun (SQL Server ve EF Core tabanlı)
- [x] `_Layout.cshtml` ve `_Sidebar.cshtml` ile temel arayüz iskeletini kurun
- [x] Proje klasör yapısını oluşturun: `/Models`, `/Services`, `/Views`, `/wwwroot`

---

## 2. Saat: Servisler ve YZ Karar Motoru
**Durum:** Tamamlandı ✅

- [x] `AIDecisionEngine` servisini oluşturun — otomatik onay, öncelik yönlendirme, adım atlama, anomali tespiti, akıllı atama mantıkları
- [x] `WorkflowService`'i oluşturun — iş akışı örnekleme, adım ilerletme, durum yönetimi
- [x] `IntegrationService`'i oluşturun — SahteCRM, SahteEPosta, SahteDrive, SahteSlack, SahteERP bağlayıcıları
- [x] `NotificationService` ve `AuditService` servislerini yazın
- [x] `DbInitializer` ile başlangıç verilerini yükleyin — örnek kullanıcılar, iş akışı şablonları, çalışan örnekler

---

## 3. Saat: Controller'lar ve Kullanıcı Etkileşimi
**Durum:** Kısmen Tamamlandı 🟡

- [x] `HomeController` — gösterge paneli metrikleri (toplam iş akışı, YZ otomasyonu oranı, bekleyen görevler)
- [x] `WorkflowController` — iş akışı listele, oluştur, detay, başlat/durdur
- [x] `TaskController` — görev kutusu, onayla/reddet/tamamla işlemleri
- [ ] `ApiController` — AJAX çağrıları için JSON uç noktaları
- [x] `Home/Index.cshtml` (gösterge paneli), `Workflow/Index.cshtml`, `Workflow/Create.cshtml` görünümlerini oluşturun

---

## 4. Saat: Arayüz İyileştirme ve Uçtan Uca Test
**Durum:** Kısmen Tamamlandı 🟡

- [x] Dashboard'a dinamik kartlar ekleyin: YZ kararları akışı, entegrasyon durumları, son bildirimler
- [x] `Task/Details.cshtml` veya Liste — YZ gerekçesini ve güven skorunu kullanıcıya gösterin
- [x] `IntegrationController` ve `AuditController` UI sayfalarını tamamlayın
- [ ] Uçtan uca test: iş akışı oluştur → YZ değerlendir → görev ata → onayla → entegrasyon tetikle → denetim kaydı yaz
- [ ] Hata düzeltme ve veri akışı doğrulama

---

## 5. Saat: Son İnceleme ve Sunum Hazırlığı
**Durum:** Devam Ediyor ⏳

- [x] `AI_TRACEABILITY_PR_TEMPLATE.md` belgesini son haline getirin
- [ ] Kodu temizleyin, `dotnet run` ile sorunsuz çalıştığını doğrulayın
- [x] Demo senaryosunu hazırlayın (`DemoScenario.md`)
- [ ] Demo akışı: Satın Alma Onayı örneği üzerinden YZ'nin otomatik onay → finans incelemesi → CRM/ERP entegrasyonu → denetim izi sürecini canlı gösterin

---

## Demo Senaryosu Özeti

```text
1. Gösterge panelini göster → genel metrikleri açıkla
2. Yeni satın alma talebi oluştur (₺3.000, Bilişim departmanı)
3. YZ otomatik onayı göster → güven skoru ve gerekçeyi göster
4. Yüksek tutarlı talep oluştur (₺15.000) → manuel inceleme tetiklenir
5. Görev kutusundan finans onayı ver
6. Entegrasyon panelinde CRM/ERP kayıtlarını göster
7. Denetim günlüğünde tüm adımları göster
8. "Öncesi vs. Sonrası" karşılaştırmasını sun
```
