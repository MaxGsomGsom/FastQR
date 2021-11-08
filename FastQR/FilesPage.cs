using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ElmSharp;
using ElmSharp.Wearable;

namespace FastQR
{
    public sealed class FilesPage : IDisposable
    {
        private readonly Window window;
        private readonly Conformant conformant;
        private string currentDir = "/home/owner/media/Images";
        private readonly CircleGenList filesList;
        private readonly GenItemClass stringGenItemClass;

        public event EventHandler<string>? LoadImage;

        public FilesPage(Window window, Conformant conformant)
        {
            this.window = window;
            this.conformant = conformant;

            stringGenItemClass = new GenItemClass("default");
            stringGenItemClass.GetTextHandler = (data, _) => data is string str ? str : string.Empty;

            filesList = new CircleGenList(window, new CircleSurface(conformant));
            filesList.ItemSelected += OnFileSelected;

            conformant.SetContent(filesList);
            filesList.Show();
            FillFilesList();
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
                LoadImage?.Invoke(this, newFileOrDir);
            }
        }

        private void FillFilesList()
        {
            filesList.Clear();
            filesList.Append(stringGenItemClass, "Select image:");

            var files = Directory.EnumerateFileSystemEntries(currentDir)
                .Select(e => e.Remove(0, currentDir.Length))
                .Where(e => !e.Contains(Utility.Extension));
            foreach (var file in files)
                filesList.Append(stringGenItemClass, file, GenListItemType.Normal);

            filesList.Append(stringGenItemClass, string.Empty);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            filesList.Hide();
        }
    }
}
