using ElmSharp;

namespace FastQR
{
    public sealed class ImagePage
    {
        private readonly Window window;
        private readonly Conformant conformant;
        private readonly string file;
        private readonly Background background;

        public ImagePage(Window window, Conformant conformant, string file)
        {
            this.window = window;
            this.conformant = conformant;
            this.file = file;

            background = new Background(window);
            background.BackgroundOption = BackgroundOptions.Tile;
            background.Color = Color.White;
            background.Show();
            conformant.SetContent(background);
            background.File = Utility.GetTransformedFile(file);
        }
    }
}
