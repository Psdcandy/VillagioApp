using Microsoft.Extensions.Logging;

namespace VillagioApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
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
