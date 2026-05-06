# FlowMind: Uçtan Uca (E2E) Demo Senaryosu

Bu doküman, Jüri sunumu ve uçtan uca testler için hazırlanmış Seed Data (başlangıç verileri) üzerinden test senaryolarını açıklar (Issue #17).

## Senaryo 1: Düşük Riskli Otomatik Onay (Satınalma)
1. **Veri:** `InMemoryDataStore` içinde `WFI-002` id'li iş akışı.
2. **Durum:** Tutar 2.500₺. 
3. **Beklenen YZ Kararı:** Tutar, 5.000₺'lik `OtomatikOnayEsik` değerinin altında olduğu için YZ otomatik onay verecek.
4. **Entegrasyon:** Başarılı onayın ardından sahte CRM ve ERP entegrasyonları tetiklenecek.

## Senaryo 2: Yüksek Riskli Manuel İnceleme (Satınalma)
1. **Veri:** `WFI-001` id'li iş akışı.
2. **Durum:** Tutar 120.000₺.
3. **Beklenen YZ Kararı:** Tutar 50.000₺'den yüksek olduğu için manuel incelemeye yönlendirilir ve Güven Skoru düşüktür (%40).
4. **Görev:** İlgili yöneticiye (Finans veya Birim Yöneticisi) bir onay Task'ı düşer.

## Senaryo 3: İşe Alım Akışı
1. **Veri:** `WFI-003` id'li iş akışı.
2. **Durum:** İK Değerlendirme süreci tamamlandı, pozisyon açık ve bütçe uygun.
3. **Beklenen YZ Kararı:** Sistem güven skoru yüksek olduğu için bir sonraki adıma geçirir ve Slack duyurusu gibi entegrasyonları çalıştırır.

## Test Adımları
Projeyi çalıştırdığınızda (`dotnet run`):
1. Veriler RAM üzerinde anında oluşur (SeedData).
2. /Workflow sayfasına giderek bu 3 akışın canlı örneklerini görebilirsiniz.
3. Loglarda `[AI]`, `[AUDIT]` ve `[NOTIFICATION]` etiketli işlemleri konsoldan izleyebilirsiniz.
