using ElmSharp;
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using ElmSharp.Wearable;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Tizen.Applications;
using SharpColor = SixLabors.ImageSharp.Color;
using ElmColor = ElmSharp.Color;
using ElmImage = ElmSharp.Image;
using SharpImage = SixLabors.ImageSharp.Image;

namespace FastQR
{
    public class FastQRWidget : WidgetBase
    {
        private Conformant? conformant;
        private readonly UserPermission userPermission = new();
        private WidgetState? state;
        private FilesPage? filesPage;
        private ImagePage? imagePage;
        private AdjustmentsPage? adjustmentsPage;

        public override async void OnCreate(Bundle content, int w, int h)
        {
            base.OnCreate(content, w, h);

            conformant = new Conformant(Window);
            conformant.Show();

            state = BundleManager.Load(content);

            if (!await userPermission.CheckAndRequestPermission(Utility.StoragePrivilege))
                Exit();
            else if (state != null)
                imagePage = new ImagePage(Window, conformant, state);
            else
            {
                filesPage = new FilesPage(Window, conformant);
                filesPage.LoadImage += (_, file) =>
                {
                    state = new WidgetState(file);
                    adjustmentsPage = new AdjustmentsPage(Window, conformant, state);
                    adjustmentsPage.Finished += (_, newState) =>
                    {
                        state = newState;
                        BundleManager.Save(state, SetContent);
                        imagePage = new ImagePage(Window, conformant, state);
                    };
                };
            }
                
        }
    }
}
