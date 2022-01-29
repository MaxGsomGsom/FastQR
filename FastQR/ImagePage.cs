using System.IO;
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
                CanFillOutside = true,
                CanScaleDown = false,
                CanScaleUp = false,
                IsMirroredMode = false,
                IsScaling = false,
                IsFixedAspect = false,
            };
            background.Show();
            conformant.SetContent(background);

            //TODO: "name=" replaced for back compatibility. Remove in next version
            var labelToShow = Path.GetFileNameWithoutExtension(file).Replace("name=", "");
            //Do not show if starts from "_"
            if (labelToShow.Length > 0 && labelToShow[0] != '_')
            {
                var label = new Label(window)
                {
                    Geometry = new Rect(0, 10, screenWidth, screenWidth),
                    Text = $"<font_size=24 align=center>{labelToShow}</font_size>",
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
