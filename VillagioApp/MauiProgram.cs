using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using VillagioApp.Data;

namespace VillagioApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Simulação da leitura do appsettings.json
            var connectionString = "Server=JUN0675605W11-1\\BDSENAC;Initial Catalog=DBAPPVILLAGIOMICHELIN;User Id=senaclivre;Password='senaclivre';TrustServerCertificate=yes";

            // Registro do DBContext com a string de conexão
            builder.Services.AddDbContext<DBContext>(options =>
                options.UseSqlServer(connectionString));

            // Registrar a MainPage com injeção de dependência
            builder.Services.AddTransient<MainPage>();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("fonnts.com-Canvas_Inline_Reg.otf", "CanvasIn");
                    fonts.AddFont("SourceSans3-Italic-VariableFont_wght.ttf", "Sans3");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}