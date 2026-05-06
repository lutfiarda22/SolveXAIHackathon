using FlowMind.Services;
using FlowMind.Data;
using Microsoft.EntityFrameworkCore;

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

            // Veritabanı bağlamı (EF Core)
            builder.Services.AddDbContext<FlowMindDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // İş akışı yönetim servisi
            builder.Services.AddScoped<IIsAkisiServisi, IsAkisiServisi>();
            builder.Services.AddScoped<IEmailService, FakeEmailService>();

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

            // Veritabanını oluştur ve seed dataları ekle
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<FlowMindDbContext>();
                db.Database.EnsureCreated();
            }

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
