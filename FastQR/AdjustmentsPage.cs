using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using ElmSharp;
using ElmSharp.Wearable;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SharpColor = SixLabors.ImageSharp.Color;
using ElmColor = ElmSharp.Color;
using ElmImage = ElmSharp.Image;
using Rectangle = SixLabors.ImageSharp.Rectangle;
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

        private string resPath => Tizen.Applications.Application.Current.DirectoryInfo.SharedResource;

        public event EventHandler? Finished;

        private readonly Window window;
        private readonly Conformant conformant;
        private readonly string file;
        public float translateX = 0;
        public float translateY = 0;
        public float zoom = 1;
        private SharpImage originalImage;
        private readonly Background background;
        private readonly RotarySelector rotarySelector;

        public AdjustmentsPage(Window window, Conformant conformant, string file)
        {
            this.window = window;
            this.conformant = conformant;
            this.file = file;

            background = new Background(window);
            background.BackgroundOption = BackgroundOptions.Tile;
            background.Show();

            rotarySelector = new RotarySelector(window);
            smaller = new() { NormalIconImage = new ElmImage(window) };
            smaller.NormalIconImage.Load(Path.Combine(resPath, "Minus.png"));
            bigger = new() { NormalIconImage = new ElmImage(window) };
            bigger.NormalIconImage.Load(Path.Combine(resPath, "Plus.png"));
            moveRight = new() { NormalIconImage = new ElmImage(window) };
            moveRight.NormalIconImage.Load(Path.Combine(resPath, "Right.png"));
            moveLeft = new() { NormalIconImage = new ElmImage(window) };
            moveLeft.NormalIconImage.Load(Path.Combine(resPath, "Left.png"));
            moveBottom = new() { NormalIconImage = new ElmImage(window) };
            moveBottom.NormalIconImage.Load(Path.Combine(resPath, "Bottom.png"));
            moveTop = new() { NormalIconImage = new ElmImage(window) };
            moveTop.NormalIconImage.Load(Path.Combine(resPath, "Top.png"));
            okButton = new() { NormalIconImage = new ElmImage(window) };
            okButton.NormalIconImage.Load(Path.Combine(resPath, "Ok.png"));
            rotarySelector.Items.Add(smaller);
            rotarySelector.Items.Add(bigger);
            rotarySelector.Items.Add(moveRight);
            rotarySelector.Items.Add(moveLeft);
            rotarySelector.Items.Add(moveBottom);
            rotarySelector.Items.Add(moveTop);
            rotarySelector.Items.Add(okButton);

            rotarySelector.BackgroundColor = ElmColor.FromRgba(0, 0, 0, 64);
            rotarySelector.Show();
            conformant.SetContent(background);

            rotarySelector.Clicked += OnSelectorOnClicked;

            originalImage = SharpImage.Load(file);
            originalImage.SaveAsPng(Utility.GetTransformedFile(file));
            background.File = Utility.GetTransformedFile(file);
        }

        private void OnSelectorOnClicked(object s, RotarySelectorItemEventArgs e)
        {
            using var sharpImage = originalImage.Clone(e => { });

            if (e.Item == moveBottom)
                translateY -= 20;
            else if (e.Item == moveTop)
                translateY += 20;
            else if (e.Item == moveLeft)
                translateX += 20;
            else if (e.Item == moveRight)
                translateX -= 20;
            else if (e.Item == smaller)
                zoom -= 0.05f;
            else if (e.Item == bigger)
                zoom += 0.05f;
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
        }

        /// <inheritdoc />
        public void Dispose()
        {
            background.Hide();
            rotarySelector.Hide();
        }
    }
}
