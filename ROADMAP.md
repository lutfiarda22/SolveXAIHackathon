# FlowMind - 5 Saatlik Hackathon Yol Haritası

## Hedef
Yapay zeka odaklı iş akışı orkestrasyonunu; sahte entegrasyonlar ve bellek içi veritabanı (`ConcurrentDictionary`) ile gösteren, yerel olarak çalışan bir ASP.NET Core MVC (.NET 9) web uygulaması teslim etmek.

## Ekip Yapısı
- **Geliştirici 1 (Mimar/YZ):** YZ Karar Motoru, servis katmanı (`AIDecisionEngine`, `WorkflowService`).
- **Geliştirici 2 (Backend):** Modeller, Controller'lar, `InMemoryDataStore`, sahte entegrasyonlar.
- **Geliştirici 3 (Frontend):** Görünümler (Views), Layout, Dashboard arayüzü, CSS/JS.

---

## 1. Saat: Proje Kurulumu ve Temel Altyapı

**Hedef:** Çalışan bir MVC projesi, veri modelleri ve bellek içi depo hazır olmalı.

| Kişi | Görev |
|---|---|
| **Dev 1 & 2** | ASP.NET Core MVC (.NET 9) projesini oluşturun (`dotnet new mvc`) |
| **Dev 2** | Tüm modelleri oluşturun: `Workflow`, `WorkflowStep`, `WorkflowInstance`, `TaskItem`, `User`, `AuditLog`, `Notification`, `IntegrationLog`, `AIDecision` |
| **Dev 2** | `InMemoryDataStore` servisini yazın (`ConcurrentDictionary` tabanlı, Singleton) |
| **Dev 3** | `_Layout.cshtml` ve `_Sidebar.cshtml` ile temel arayüz iskeletini kurun |
| **Dev 1** | Proje klasör yapısını oluşturun: `/Models`, `/Services`, `/Views`, `/wwwroot` |

---

## 2. Saat: Servisler ve YZ Karar Motoru

**Hedef:** İş mantığı ve YZ simülasyonu çalışır durumda olmalı.

| Kişi | Görev |
|---|---|
| **Dev 1** | `AIDecisionEngine` servisini oluşturun — otomatik onay, öncelik yönlendirme, adım atlama, anomali tespiti, akıllı atama mantıkları |
| **Dev 1** | `WorkflowService`'i oluşturun — iş akışı örnekleme, adım ilerletme, durum yönetimi |
| **Dev 2** | `IntegrationService`'i oluşturun — SahteCRM, SahteEPosta, SahteDrive, SahteSlack, SahteERP bağlayıcıları (`Task.Delay` ile gecikme simülasyonu) |
| **Dev 2** | `NotificationService` ve `AuditService` servislerini yazın |
| **Dev 3** | `DbInitializer` ile başlangıç verilerini yükleyin — örnek kullanıcılar, iş akışı şablonları, çalışan örnekler |

---

## 3. Saat: Controller'lar ve Kullanıcı Etkileşimi

**Hedef:** Tüm sayfalar çalışır ve backend'e bağlı olmalı.

| Kişi | Görev |
|---|---|
| **Dev 2** | `HomeController` — gösterge paneli metrikleri (toplam iş akışı, YZ otomasyonu oranı, bekleyen görevler) |
| **Dev 2** | `WorkflowController` — iş akışı listele, oluştur, detay, başlat/durdur |
| **Dev 2** | `TaskController` — görev kutusu, onayla/reddet/tamamla işlemleri |
| **Dev 1** | `ApiController` — AJAX çağrıları için JSON uç noktaları |
| **Dev 3** | `Home/Index.cshtml` (gösterge paneli), `Workflow/Index.cshtml`, `Workflow/Create.cshtml` görünümlerini oluşturun |

---

## 4. Saat: Arayüz İyileştirme ve Uçtan Uca Test

**Hedef:** Profesyonel görünüm ve veri akışının eksiksiz çalışması.

| Kişi | Görev |
|---|---|
| **Dev 3** | Dashboard'a dinamik kartlar ekleyin: YZ kararları akışı, entegrasyon durumları, son bildirimler |
| **Dev 3** | `Task/Details.cshtml` — YZ gerekçesini ve güven skorunu kullanıcıya gösterin |
| **Dev 1** | `IntegrationController` ve `AuditController` sayfalarını tamamlayın |
| **Dev 1 & 2** | Uçtan uca test: iş akışı oluştur → YZ değerlendir → görev ata → onayla → entegrasyon tetikle → denetim kaydı yaz |
| **Tümü** | Hata düzeltme ve veri akışı doğrulama |

---

## 5. Saat: Son İnceleme ve Sunum Hazırlığı

**Hedef:** Kod dondurma (code freeze) ve demo senaryosu hazır.

| Kişi | Görev |
|---|---|
| **Dev 1** | `AI_TRACEABILITY.md` belgesini son haline getirin |
| **Dev 2** | Kodu temizleyin, `dotnet run` ile sorunsuz çalıştığını doğrulayın |
| **Dev 3** | Demo senaryosunu hazırlayın ve çalışın |
| **Tümü** | Demo akışı: Satın Alma Onayı örneği üzerinden YZ'nin otomatik onay → finans incelemesi → CRM/ERP entegrasyonu → denetim izi sürecini canlı gösterin |

---

## Demo Senaryosu Özeti

```
1. Gösterge panelini göster → genel metrikleri açıkla
2. Yeni satın alma talebi oluştur (₺3.000, Bilişim departmanı)
3. YZ otomatik onayı göster → güven skoru ve gerekçeyi göster
4. Yüksek tutarlı talep oluştur (₺15.000) → manuel inceleme tetiklenir
5. Görev kutusundan finans onayı ver
6. Entegrasyon panelinde CRM/ERP kayıtlarını göster
7. Denetim günlüğünde tüm adımları göster
8. "Öncesi vs. Sonrası" karşılaştırmasını sun
```
