using ElmSharp;
using Tizen.Applications;

namespace FastQRDummy
{
    class App : CoreUIApplication
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Initialize();
        }

        void Initialize()
        {
            Window window = new Window("FastQR");
            window.BackButtonPressed += (s, e) => Exit();
            window.Show();

            var box = new Box(window)
            {
                AlignmentX = -1,
                AlignmentY = -1,
                WeightX = 1,
                WeightY = 1,
            };
            box.Show();

            var conformant = new Conformant(window);
            conformant.Show();
            conformant.SetContent(box);

            var label = new Label(window)
            {
                Text = "<font_size=28 align=center>Go to widgets list and<br>select FastQR widget there." +
                       "<br>For more info visit<br>Galaxy Store.</font_size>",
                Color = Color.White
            };
            label.Show();
            box.PackEnd(label);
        }

        static void Main(string[] args)
        {
            Elementary.Initialize();
            Elementary.ThemeOverlay();
            App app = new App();
            app.Run(args);
        }
    }
}
