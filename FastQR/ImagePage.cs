using ElmSharp;

namespace FastQR
{
    public sealed class ImagePage
    {
        private readonly Window window;
        private readonly Conformant conformant;
        private readonly WidgetState state;
        private readonly Background background;

        public ImagePage(Window window, Conformant conformant, WidgetState state)
        {
            this.window = window;
            this.conformant = conformant;
            this.state = state;

            background = new Background(window);
            background.Show();
            conformant.SetContent(background);
            background.File = state.TransformedFile;
        }
    }
}
