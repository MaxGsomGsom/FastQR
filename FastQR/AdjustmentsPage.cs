using System;
using System.IO;
using System.Numerics;
using ElmSharp;
using ElmSharp.Wearable;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Tizen.System;
using Color = SixLabors.ImageSharp.Color;
using ElmColor = ElmSharp.Color;
using ElmImage = ElmSharp.Image;
using SharpImage = SixLabors.ImageSharp.Image;

namespace FastQR
{
    public sealed class AdjustmentsPage : IDisposable
    {
        private readonly RotarySelectorItem bigger;
        private RotarySelectorItem smaller;
        private readonly RotarySelectorItem moveRight;
        private readonly RotarySelectorItem moveLeft;
        private readonly RotarySelectorItem moveBottom;
        private readonly RotarySelectorItem moveTop;
        private readonly RotarySelectorItem okButton;
        private readonly int screenWidth;
        private const int DeltaTransform = 20;
        private const float DeltaZoom = 0.05f;
        private readonly ElmColor GrayTransparent = ElmColor.FromRgba(0, 0, 0, 64);
        private readonly ElmColor WhiteTransparent = ElmColor.FromRgba(255, 255, 255, 190);
        private readonly Window window;
        private readonly Conformant conformant;
        private readonly string file;
        private readonly SharpImage originalImage;
        private readonly Background background;
        private readonly RotarySelector rotarySelector;

        private string resPath => Tizen.Applications.Application.Current.DirectoryInfo.SharedResource;

        public event EventHandler? Finished;

        public float translateX = 0;
        public float translateY = 0;
        public float zoom = 1;

        public AdjustmentsPage(Window window, Conformant conformant, string file)
        {
            this.window = window;
            this.conformant = conformant;
            this.file = file;

            Information.TryGetValue(Utility.ScreenWidthFeature, out screenWidth);

            background = new Background(window);
            background.BackgroundOption = BackgroundOptions.Tile;
            background.Color = ElmColor.White;
            background.Show();

            rotarySelector = new RotarySelector(window);
            smaller = CreateButton("Minus.png");
            bigger = CreateButton("Plus.png");
            moveRight = CreateButton("Right.png");
            moveLeft = CreateButton("Left.png");
            moveBottom = CreateButton("Bottom.png");
            moveTop = CreateButton("Top.png");
            okButton = CreateButton("Ok.png");
            rotarySelector.Items.Add(smaller);
            rotarySelector.Items.Add(bigger);
            rotarySelector.Items.Add(moveRight);
            rotarySelector.Items.Add(moveLeft);
            rotarySelector.Items.Add(moveBottom);
            rotarySelector.Items.Add(moveTop);
            rotarySelector.Items.Add(okButton);

            rotarySelector.BackgroundColor = GrayTransparent;
            rotarySelector.Show();
            conformant.SetContent(background);

            rotarySelector.Clicked += OnSelectorOnClicked;

            originalImage = SharpImage.Load(file);
            var padding = (int)(screenWidth * 1.5);
            originalImage.Mutate(img => img.Pad(padding, padding, Color.White));
            originalImage.SaveAsPng(Utility.GetTransformedFile(file));
            background.File = Utility.GetTransformedFile(file);
        }

        private RotarySelectorItem CreateButton(string icon)
        {
            var result = new RotarySelectorItem() { NormalIconImage = new ElmImage(window) };
            result.NormalIconImage.Load(Path.Combine(resPath, icon));
            result.NormalBackgroundColor = WhiteTransparent;
            result.SelectedBackgroundColor = WhiteTransparent;
            result.PressedBackgroundColor = WhiteTransparent;
            return result;
        }

        private void OnSelectorOnClicked(object s, RotarySelectorItemEventArgs e)
        {
            using var sharpImage = originalImage.Clone(_ => { });

            if (e.Item == moveBottom)
                translateY -= DeltaTransform;
            else if (e.Item == moveTop)
                translateY += DeltaTransform;
            else if (e.Item == moveLeft)
                translateX += DeltaTransform;
            else if (e.Item == moveRight)
                translateX -= DeltaTransform;
            else if (e.Item == smaller)
                zoom -= DeltaZoom;
            else if (e.Item == bigger)
                zoom += DeltaZoom;
            else if (e.Item == okButton)
            {
                Finished?.Invoke(this, EventArgs.Empty);
                return;
            }

            TransformImage(sharpImage);
            sharpImage.SaveAsPng(Utility.GetTransformedFile(file));
            background.File = Utility.GetTransformedFile(file);
        }

        private void TransformImage(SharpImage sharpImage)
        {
            if (translateX != 0 || translateY != 0)
            {
                var transformBuilder = new AffineTransformBuilder();
                transformBuilder.AppendTranslation(new Vector2(translateX, translateY));
                sharpImage.Mutate(img => img.Transform(transformBuilder));
            }

            if (Math.Abs(zoom - 1f) > 0.001f)
            {
                var x = sharpImage.Width * zoom;
                var y = sharpImage.Height * zoom;
                sharpImage.Mutate(img => img.Resize((int)x, (int)y));
            }

            try
            {
                sharpImage.Mutate(img => img.Crop(screenWidth, screenWidth));
            } catch {}
        }

        /// <inheritdoc />
        public void Dispose()
        {
            background.Hide();
            rotarySelector.Hide();
        }
    }
}
