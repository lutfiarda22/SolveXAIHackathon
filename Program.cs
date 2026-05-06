using FlowMind.Services;

namespace FlowMind
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ===== Servislerin Kaydı (Dependency Injection) =====
            // MVC servislerini ekle
            builder.Services.AddControllersWithViews();

            // Merkezi bellek içi veri deposu (Singleton — tüm uygulama boyunca tek örnek)
            builder.Services.AddSingleton<InMemoryDataStore>();

            // İş akışı yönetim servisi
            builder.Services.AddScoped<IIsAkisiServisi, IsAkisiServisi>();

            // AI Karar Motoru servisi
            builder.Services.AddScoped<IKararMotoru, KararMotoru>();

            // Denetim günlüğü servisi
            builder.Services.AddScoped<AuditService>();

            // Bildirim servisi
            builder.Services.AddScoped<NotificationService>();

            // Mock entegrasyon servisi (CRM, ERP, Slack vb.)
            builder.Services.AddScoped<MockIntegrationService>();

            // Gelişmiş YZ Karar Motoru
            builder.Services.AddScoped<AIDecisionEngine>();

            var app = builder.Build();

            // Başlangıç verilerini yükle
            var dataStore = app.Services.GetRequiredService<InMemoryDataStore>();
            dataStore.SeedData();

            // ===== HTTP İstek Pipeline Yapılandırması =====
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.MapStaticAssets();

            // Varsayılan rota yapılandırması
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
