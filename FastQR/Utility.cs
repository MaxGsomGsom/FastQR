using System;
using System.IO;
using Tizen.Applications;

namespace FastQR
{
    public static class Utility
    {
        public const string StoragePrivilege = "http://tizen.org/privilege/mediastorage";
        public const string ScreenWidthFeature = "http://tizen.org/feature/screen.width";
        public const string LogTag = "FastQR";
        public const string Extension = ".fastqr";
        private static string DirToSave = "/home/owner/media/.FastQR";

        public static string GetTransformedFile(string file)
        {
            //TODO: Remove in next version
            var compatiblePath = file + Extension;
            if (File.Exists(compatiblePath))
                return compatiblePath;

            return Path.Combine(DirToSave, Path.GetFileName(file) + Extension);
        }

        private const string StorageKey = "widgetState";

        public static string? Load(Bundle bundle)
        {
            try
            {
                return bundle.GetItem<string>(StorageKey);
            }
            catch
            {
                return null;
            }
        }

        public static void Save(string? file, Action<Bundle> setContent)
        {
            if (file == null)
                return;

            var bundle = new Bundle();
            bundle.AddItem(StorageKey, file);
            setContent(bundle);
        }
    }
}
