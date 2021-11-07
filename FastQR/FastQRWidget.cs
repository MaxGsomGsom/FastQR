using ElmSharp;
using System;
using System.IO;
using System.Linq;
using ElmSharp.Wearable;
using Tizen.Applications;

namespace FastQR
{
    public class FastQRWidget : WidgetBase
    {
        private Conformant? conformant;
        private CircleGenList? filesList;
        private Image? qrCodeView;
        private GenItemClass? stringGenItemClass;
        private readonly UserPermission userPermission = new();
        private string currentDir = "/home/owner/media/Images";
        private string? selectedFile;
        private string selectedFileKey = "selectedFile";

        public override async void OnCreate(Bundle content, int w, int h)
        {
            base.OnCreate(content, w, h);

            try
            {
                selectedFile = content.GetItem<string>(selectedFileKey);
            }
            catch { }

            if (!await userPermission.CheckAndRequestPermission(Utility.StoragePrivilege))
                Exit();
            else if (!string.IsNullOrWhiteSpace(selectedFile) && File.Exists(selectedFile))
                InitQrLayout();
            else
                InitFilesLayout();
        }

        private void InitFilesLayout()
        {
            if (filesList != null)
                return;

            InitBasicLayout();

            stringGenItemClass = new GenItemClass("default");
            stringGenItemClass.GetTextHandler = (data, _) => data is string str ? str : string.Empty;

            filesList = new CircleGenList(Window, new CircleSurface(conformant));
            filesList.ItemSelected += OnFileSelected;

            conformant?.SetContent(filesList);
            filesList.Show();
            FillFilesList();
        }

        private void InitBasicLayout()
        {
            filesList?.Hide();
            filesList = null;

            qrCodeView?.Hide();
            qrCodeView = null;

            if (conformant != null)
                return;

            conformant = new Conformant(Window);
            conformant.Show();
        }

        private void OnFileSelected(object sender, GenListItemEventArgs e)
        {
            if (!(e.Item.Data is string fileOrDir))
                return;

            var newFileOrDir = currentDir + fileOrDir;
            if (!Uri.IsWellFormedUriString(newFileOrDir, UriKind.RelativeOrAbsolute))
                return;

            if (Directory.Exists(newFileOrDir))
            {
                currentDir = newFileOrDir;
                FillFilesList();
            }
            else if (File.Exists(newFileOrDir))
            {
                selectedFile = newFileOrDir;
                var bundle = new Bundle();
                bundle.AddItem(selectedFileKey, selectedFile);
                SetContent(bundle);
                InitQrLayout();
            }
        }

        private void FillFilesList()
        {
            if (filesList == null)
                return;

            filesList.Clear();
            filesList.Append(stringGenItemClass, "Select image:");

            var files = Directory.EnumerateFileSystemEntries(currentDir)
                .Select(e => e.Remove(0, currentDir.Length));
            foreach (var file in files)
                filesList.Append(stringGenItemClass, file, GenListItemType.Normal);

            filesList.Append(stringGenItemClass, string.Empty);
        }

        private void InitQrLayout()
        {
            if (string.IsNullOrWhiteSpace(selectedFile))
                return;

            InitBasicLayout();

            filesList?.Hide();
            filesList = null;

            qrCodeView = new Image(Window);
            qrCodeView.Load(selectedFile);
            qrCodeView.Show();
            conformant?.SetContent(qrCodeView);
        }
    }
}
