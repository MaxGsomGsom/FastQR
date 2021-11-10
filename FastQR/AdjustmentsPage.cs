using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using ElmSharp;
using ElmSharp.Wearable;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Tizen.System;
using ElmColor = ElmSharp.Color;
using ElmImage = ElmSharp.Image;
using SharpImage = SixLabors.ImageSharp.Image;

namespace FastQR
{
    public sealed class AdjustmentsPage : IDisposable
    {
        private readonly RotarySelectorItem bigger;
        private readonly RotarySelectorItem smaller;
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
        private SharpImage? originalImage;
        private readonly MemoryStream modifiedImage = new();
        private readonly ElmImage background;
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

            background = new ElmImage(window)
            {
                BackgroundColor = ElmColor.White,
                IsScaling = false,
                CanFillOutside = true
            };
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
        }

        public async Task Init()
        {
            originalImage = await SharpImage.LoadAsync(file);
            FixSize(originalImage, originalImage.Width);
            FixSize(originalImage, originalImage.Height);

            await OnSelectorOnClicked(this, new RotarySelectorItemEventArgs());

            async void OnRotarySelectorOnClicked(object s, RotarySelectorItemEventArgs e) => await OnSelectorOnClicked(s, e);
            rotarySelector.Clicked += OnRotarySelectorOnClicked;
        }

        private void FixSize(SharpImage image, int size)
        {
            var maxSize = screenWidth * 2;
            if (size > maxSize)
            {
                var scale = size / (float)maxSize;
                originalImage.Mutate(img =>img.Resize(
                    (int)(image.Width / scale),
                    (int)(image.Height / scale)));
            }
        }

        private RotarySelectorItem CreateButton(string icon)
        {
            var result = new RotarySelectorItem { NormalIconImage = new ElmImage(window) };
            result.NormalIconImage.LoadAsync(Path.Combine(resPath, icon));
            result.NormalBackgroundColor = WhiteTransparent;
            result.SelectedBackgroundColor = WhiteTransparent;
            result.PressedBackgroundColor = WhiteTransparent;
            return result;
        }

        private async Task OnSelectorOnClicked(object s, RotarySelectorItemEventArgs e)
        {
            using var sharpImage = originalImage.Clone(_ => { });

            if (e.Item == moveBottom)
                translateY += DeltaTransform;
            else if (e.Item == moveTop)
                translateY -= DeltaTransform;
            else if (e.Item == moveLeft)
                translateX -= DeltaTransform;
            else if (e.Item == moveRight)
                translateX += DeltaTransform;
            else if (e.Item == smaller)
                zoom -= DeltaZoom;
            else if (e.Item == bigger)
                zoom += DeltaZoom;
            else if (e.Item == okButton)
            {
                TransformImage(sharpImage);
                await sharpImage.SaveAsPngAsync(Utility.GetTransformedFile(file));
                Finished?.Invoke(this, EventArgs.Empty);
                return;
            }

            TransformImage(sharpImage);
            modifiedImage.Position = 0;
            modifiedImage.SetLength(0);
            await sharpImage.SaveAsPngAsync(modifiedImage);
            modifiedImage.Position = 0;
            await background.LoadAsync(modifiedImage);
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
        }

        /// <inheritdoc />
        public void Dispose()
        {
            background.Hide();
            rotarySelector.Hide();
        }
    }
}
