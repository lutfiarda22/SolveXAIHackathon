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

            // İş akışı yönetim servisi
            builder.Services.AddScoped<IIsAkisiServisi, IsAkisiServisi>();

            // AI Karar Motoru servisi
            builder.Services.AddScoped<IKararMotoru, KararMotoru>();

            // TODO: Veritabanı bağlantısı ileride buraya eklenecek
            // builder.Services.AddDbContext<FlowMindDbContext>(...);

            // TODO: Entegrasyon servisleri (CRM, ERP, İletişim)
            // builder.Services.AddScoped<IEntegrasyonServisi, EntegrasyonServisi>();

            var app = builder.Build();

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
