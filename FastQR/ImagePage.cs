using System.Threading.Tasks;
using ElmSharp;

namespace FastQR
{
    public sealed class ImagePage
    {
        private readonly Window window;
        private readonly Conformant conformant;
        private readonly string file;
        private readonly Image background;

        public ImagePage(Window window, Conformant conformant, string file)
        {
            this.window = window;
            this.conformant = conformant;
            this.file = file;

            background = new Image(window)
            {
                BackgroundColor = Color.White,
                IsScaling = false,
                CanFillOutside = true
            };
            background.Show();
            conformant.SetContent(background);
        }

        public async Task Init()
        {
            await background.LoadAsync(Utility.GetTransformedFile(file));
        }
    }
}
