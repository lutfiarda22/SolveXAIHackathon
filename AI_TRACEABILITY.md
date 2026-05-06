# FlowMind - Yapay Zeka İzlenebilirlik Belgesi

> Bu belge, FlowMind sisteminde Yapay Zekanın nerelerde ve nasıl kullanıldığını şeffaf bir şekilde açıklar.

---

## 1. YZ Tarafından Tasarlanan Bileşenler

### 1.1 YZ Karar Motoru (`AIDecisionEngine`)

Sistemin çekirdeğindeki akıllı bileşendir. İş akışı adımlarını değerlendirerek insana gerek kalmadan otomatik kararlar alır:

| Karar Türü | Açıklama | Örnek |
|---|---|---|
| **Otomatik Onay** | Tutar eşik altında ve geçmiş temiz ise onaylar | ₺3.000'lik standart talep → otomatik onay |
| **Öncelik Yönlendirme** | Acil durumları üst onaylayıcıya yönlendirir | Son tarih < 24 saat → müdüre ata |
| **Adım Atlama** | Hedef sistemde veri zaten varsa adımı atlar | CRM'de kayıt mevcut → CRM adımını atla |
| **Anomali Tespiti** | Normal kalıptan sapmaları işaretler | Alışılmadık tutar/departman → manuel inceleme |
| **Akıllı Atama** | En uygun onaylayıcıyı seçer | En az bekleyen görevi olan kişiye ata |

### 1.2 YZ Karar Modeli (`AIDecision`)

Her karar aşağıdaki verilerle kaydedilir:

```
AIDecision {
    Id              — Benzersiz tanımlayıcı
    WorkflowId      — İlgili iş akışı
    StepId          — İlgili adım
    Action          — Alınan eylem (OtomatikOnay, ManuelInceleme, AdımAtla, vb.)
    Confidence      — Güven skoru (0.0 – 1.0)
    Reasoning       — Kararın gerekçesi (Türkçe açıklama)
    Timestamp       — Karar zamanı
    WasOverridden   — Kullanıcı tarafından geçersiz kılındı mı?
}
```

---

## 2. YZ Sistemin Neresini Etkiliyor?

### 2.1 İş Akışı Yürütme Döngüsü

```
İş akışı adımı tetiklenir
        │
        ▼
 ┌──────────────────────┐
 │  AIDecisionEngine     │
 │  değerlendirme yapar  │
 └──────────┬───────────┘
            │
     ┌──────┴──────┐
     ▼             ▼
 [Otomatik]    [Manuel]
 İnsan yok     Görev oluştur
 Direkt ilerle  Kullanıcıya ata
```

### 2.2 Etki Alanları

| Alan | YZ Etkisi | Kazanım |
|---|---|---|
| **Onay Süreçleri** | Düşük riskli talepleri otomatik onaylar | Manuel onay bekleme süresi → sıfır |
| **Görev Atama** | İş yükü dengelemesi yapar | Darboğazları önler |
| **Entegrasyon Tetikleme** | CRM/ERP güncellemesini otomatik başlatır | Manuel veri girişi → sıfır |
| **Anomali Tespiti** | Olağandışı durumları işaretler | İnsan hataları erkenden yakalanır |
| **Adım Optimizasyonu** | Gereksiz adımları atlar | Süreç süresi kısalır |

---

## 3. YZ Kararlarının Kullanıldığı Noktalar

### 3.1 Gösterge Paneli (`Home/Index`)
- **YZ Otomasyonu Oranı**: Toplam iş akışlarının yüzde kaçı YZ tarafından otomatik tamamlandı
- **Kazanılan Zaman**: Manuel süreç vs. YZ otomasyonu karşılaştırması
- **YZ Karar Akışı**: Son YZ kararlarının canlı listesi

### 3.2 Görev Detayı (`Task/Details`)
- YZ'nin güven skoru ve gerekçesi kullanıcıya gösterilir
- Kullanıcı YZ kararını **geçersiz kılabilir** (override)
- Geçersiz kılma da denetim günlüğüne kaydedilir

### 3.3 Denetim Günlüğü (`Audit/Index`)
- Her YZ kararı tam izlenebilirlikle loglanır
- Kim, ne zaman, hangi güven skoruyla, hangi gerekçeyle karar aldı
- Geçersiz kılınan kararlar ayrıca işaretlenir

### 3.4 Entegrasyon Tetikleyicisi
- YZ otomatik onay verdiğinde `IntegrationService` direkt tetiklenir
- İnsan müdahalesi olmadan: CRM kaydı oluştur, ERP emri aç, Slack bildirimi gönder

---

## 4. YZ Şeffaflık İlkeleri

| İlke | Uygulama |
|---|---|
| **Açıklanabilirlik** | Her karar Türkçe gerekçeyle kaydedilir |
| **Denetlenebilirlik** | Tüm kararlar `AuditLog` + `AIDecision` tablolarında saklanır |
| **Geçersiz Kılınabilirlik** | Kullanıcı her zaman YZ kararını değiştirebilir |
| **Güven Skoru** | 0.0–1.0 arası skor ile kararın kesinliği gösterilir |
| **İzlenebilirlik** | Hangi iş akışında, hangi adımda, ne karar alındı izlenebilir |

---

## 5. Sunum İçin Özet

**FlowMind'ın değer teklifi: YZ Karar Kapısı**

```
ESKİ SÜREÇ (Manuel):
  Talep → E-posta → Bekle → Onay → Manuel CRM girişi → Manuel ERP girişi
  ⏱ Ortalama: 2-4 saat

YENİ SÜREÇ (FlowMind + YZ):
  Talep → YZ değerlendirir → Otomatik onay → CRM/ERP otomatik güncellenir
  ⏱ Ortalama: 30 saniye

  📉 %87 zaman tasarrufu
```

YZ, her adımda şeffaf gerekçe sunarak güven oluşturur. Kullanıcı istediği zaman YZ kararını geçersiz kılabilir — kontrol her zaman insandadır.
