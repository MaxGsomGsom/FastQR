using System.Linq;
using System.Threading.Tasks;
using ElmSharp;
using Tizen.System;

namespace FastQR
{
    public sealed class ImagePage
    {
        private readonly Window window;
        private readonly Conformant conformant;
        private readonly string file;
        private readonly Image background;
        readonly int screenWidth;

        public ImagePage(Window window, Conformant conformant, string file)
        {
            this.window = window;
            this.conformant = conformant;
            this.file = file;

            Information.TryGetValue(Utility.ScreenWidthFeature, out screenWidth);

            background = new Image(window)
            {
                BackgroundColor = Color.White,
                IsScaling = false,
                CanFillOutside = true
            };
            background.Show();
            conformant.SetContent(background);

            var labelToShow = file.Split('.', '/').FirstOrDefault(e => e.Contains("name:") && e.Length > 5);
            if (labelToShow != null)
            {
                var label = new Label(window)
                {
                    Geometry = new Rect(0, 0, screenWidth, screenWidth),
                    Text = $"<font_size=20 align=center>{labelToShow.Substring(5)}</font_size>",
                    Color = Color.Black,
                };

                label.Show();
                background.SetContent(label);
            }
        }

        public async Task Init()
        {
            await background.LoadAsync(Utility.GetTransformedFile(file));
        }
    }
}
