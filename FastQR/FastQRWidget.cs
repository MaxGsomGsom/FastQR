using System;
using ElmSharp;
using Tizen.Applications;

namespace FastQR
{
    public class FastQRWidget : WidgetBase
    {
        private Conformant? conformant;
        private readonly UserPermission userPermission = new();
        private string? file;
        private FilesPage? filesPage;
        private ImagePage? imagePage;
        private AdjustmentsPage? adjustmentsPage;

        public override async void OnCreate(Bundle content, int w, int h)
        {
            base.OnCreate(content, w, h);

            if (!await userPermission.CheckAndRequestPermission(Utility.StoragePrivilege))
                Exit();
            
            conformant = new Conformant(Window);
            conformant.Show();
            file = BundleManager.Load(content);
            if (file != null)
            {
                imagePage = new ImagePage(Window, conformant, file);
                return;
            }
            
            filesPage = new FilesPage(Window, conformant);
            filesPage.LoadImage += OpenAdjustmentPage;
        }

        private void OpenAdjustmentPage(object _, string newFile)
        {
            filesPage?.Dispose();
            file = newFile;
            adjustmentsPage = new AdjustmentsPage(Window, conformant!, file);
            adjustmentsPage.Finished += OpenImagePage;
        }

        private void OpenImagePage(object _, EventArgs e)
        {
            adjustmentsPage?.Dispose();
            BundleManager.Save(file, SetContent);
            imagePage = new ImagePage(Window, conformant!, file);
        }
    }
}
