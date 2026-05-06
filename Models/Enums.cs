namespace FlowMind.Models;

/// <summary>İş akışı örneğinin mevcut durumu</summary>
public enum WorkflowStatus
{
    Taslak,        // Draft
    Calisıyor,     // Running
    Beklemede,     // Waiting for approval
    Tamamlandı,    // Completed
    Reddedildi,    // Rejected
    İptalEdildi    // Cancelled
}

/// <summary>Bir iş akışı adımının eylem türü</summary>
public enum StepActionType
{
    Onay,          // Approval required
    VeriGirisi,    // Data entry
    Entegrasyon,   // External system integration
    Bildirim,      // Send notification
    YZKarar        // AI decision point
}

/// <summary>Bir adımın mevcut durumu</summary>
public enum StepStatus
{
    Bekliyor,      // Pending
    Calisıyor,     // In progress
    Tamamlandı,    // Completed
    Atlandı,       // Skipped by AI
    Reddedildi     // Rejected
}

/// <summary>Görev durumu</summary>
public enum TaskStatus
{
    Atandı,        // Assigned
    Görüldü,       // Viewed
    Tamamlandı,    // Completed
    Onaylandı,     // Approved
    Reddedildi     // Rejected
}

/// <summary>Kullanıcı rolü</summary>
public enum UserRole
{
    Calisan,       // Employee
    Yonetici,      // Manager
    Finans,        // Finance
    IT,            // IT Department
    Admin          // System Admin
}

/// <summary>Entegrasyon hedef sistemi</summary>
public enum IntegrationType
{
    CRM,
    ERP,
    EPosta,
    Slack,
    Drive
}

/// <summary>Entegrasyon durumu</summary>
public enum IntegrationStatus
{
    Bekliyor,
    Basarili,
    Basarisiz
}

/// <summary>YZ tarafından önerilen eylem türleri</summary>
public enum AIAction
{
    OtomatikOnayla,     // Auto-approve
    ManuelInceleme,     // Route to manual review
    OncelikliYonlendir, // Priority routing
    AdimAtla,           // Skip step
    Reddet              // Reject
}

/// <summary>Bildirim türü</summary>
public enum NotificationType
{
    GorevAtandi,
    OnayBekleniyor,
    IsAkisiTamamlandi,
    YZKarariVerildi,
    EntegrasyonSonucu
}
