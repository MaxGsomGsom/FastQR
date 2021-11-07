using Tizen.Applications;
using ElmSharp;

namespace FastQR
{
    class Program
    {
        static void Main(string[] args)
        {
            Elementary.Initialize();
            Elementary.ThemeOverlay();
            WidgetApplication app = new(typeof(FastQRWidget));
            app.Run(args);
        }
    }
}
