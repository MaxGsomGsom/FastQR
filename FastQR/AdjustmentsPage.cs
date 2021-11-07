using System;
using System.Collections.Generic;
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
using SharpImage = SixLabors.ImageSharp.Image;

namespace FastQR
{
    public sealed class AdjustmentsPage
    {
        private readonly RotarySelectorItem zoomIn = new() { MainText = "zoomIn" };
        private readonly RotarySelectorItem zoomOut = new() { MainText = "zoomOut" };
        private readonly RotarySelectorItem moveRight = new() { MainText = "moveRight" };
        private readonly RotarySelectorItem moveLeft = new() { MainText = "moveLeft" };
        private readonly RotarySelectorItem moveBottom = new() { MainText = "moveBottom" };
        private readonly RotarySelectorItem moveTop = new() { MainText = "moveTop" };
        private readonly RotarySelectorItem okButton = new() { MainText = "okButton" };

        public event EventHandler<WidgetState>? Finished;

        private readonly Window window;
        private readonly Conformant conformant;
        private readonly WidgetState state;
        private readonly Background background;
        private readonly RotarySelector rotarySelector;

        public AdjustmentsPage(Window window, Conformant conformant, WidgetState state)
        {
            this.window = window;
            this.conformant = conformant;
            this.state = state;

            background = new Background(window);
            background.Show();

            rotarySelector = new RotarySelector(window);
            rotarySelector.Items.Add(zoomIn);
            rotarySelector.Items.Add(zoomOut);
            rotarySelector.Items.Add(moveRight);
            rotarySelector.Items.Add(moveLeft);
            rotarySelector.Items.Add(moveBottom);
            rotarySelector.Items.Add(moveTop);
            rotarySelector.Items.Add(okButton);

            rotarySelector.BackgroundColor = ElmColor.FromRgba(0, 0, 0, 64);
            background.SetContent(rotarySelector);
            conformant.SetContent(background);

            rotarySelector.Clicked += OnSelectorOnClicked;

            using var sharpImage = SharpImage.Load(state.File);
            sharpImage.SaveAsPng(state.TransformedFile);
            background.File = state.TransformedFile;
        }

        private void OnSelectorOnClicked(object s, RotarySelectorItemEventArgs e)
        {
            using var sharpImage = SharpImage.Load(state.File);

            if (e.Item == moveBottom)
                state.TranslateY += sharpImage.Height * 0.1f;
            else if (e.Item == moveTop)
                state.TranslateY -= sharpImage.Height * 0.1f;
            else if (e.Item == moveLeft)
                state.TranslateX -= sharpImage.Width * 0.1f;
            else if (e.Item == moveRight)
                state.TranslateX += sharpImage.Width * 0.1f;
            else if (e.Item == zoomIn)
                state.Zoom *= 0.1f;
            else if (e.Item == zoomOut)
                state.Zoom *= 0.1f;
            else if (e.Item == okButton)
                Finished?.Invoke(this, state);

            var transformBuilder = new AffineTransformBuilder();
            if (state.Zoom > 1)
                transformBuilder.AppendScale(state.Zoom);
            transformBuilder.AppendTranslation(new Vector2(state.TranslateX, state.TranslateY));
            sharpImage.Mutate(x => x.Transform(transformBuilder));
            if (state.Zoom < 1)
            {
                var x = sharpImage.Width * state.Zoom;
                var y = sharpImage.Height * state.Zoom;
                sharpImage.Mutate(e => e.Pad((int)x, (int)y, SharpColor.White));
            }

            sharpImage.SaveAsPng(state.TransformedFile);
            background.File = state.TransformedFile;
        }
    }
}
