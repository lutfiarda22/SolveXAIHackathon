# YZ İzlenebilirlik (AI Traceability) Güncelleme Raporu

## 🤖 YZ Kuralı / Karar Özeti
- **Etkilenen Bileşen:** [Örn: AIDecisionEngine (Akış Kararı) / AIAgentBackgroundService (Zaman Aşımı/SLA)]
- **İşlem / Senaryo:** [Örn: Yüksek tutarlı fatura onayı / 24 saati geçen izin talebi]
- **Yeni YZ Aksiyonu:** [Örn: Otomatik Onay, Manuel İncelemeye Yönlendirme, Üst Yöneticiye Eskalasyon]
- **Güven Skoru (Varsa):** [%X]

## 🧠 Gerekçe (Kuralın Mantığı)
[YZ'nin bu kararı verirken kullandığı kural seti, eşik değerleri (threshold) ve dikkate aldığı bağlam parametreleri nelerdir? Hangi durumda bu aksiyon tetikleniyor?]

## 🛡️ İnsan Kontrolü & Güvenlik
- [ ] Yüksek riskli işlemler (örn: eskalasyonlar, hassas kararlar) için **İnsan Kontrolü (Human-in-the-loop)** eklendi.
- [ ] YZ tarafından alınan karar, gerekçesiyle birlikte `AuditLog` (Denetim Günlüğü) sistemine kaydediliyor.
- [ ] Kullanıcının YZ kararını **Geçersiz Kılabilmesi (Override)** için gerekli arayüz desteği sağlandı.

*(Bu şablon, FlowMind sistemindeki AI karar süreçlerinin ve arka plan otonom işlemlerinin şeffaf bir şekilde izlenebilir ve denetlenebilir olması için oluşturulmuştur.)*