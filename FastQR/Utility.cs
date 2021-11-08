using System;
using Tizen.Applications;

namespace FastQR
{
    public static class Utility
    {
        public const string StoragePrivilege = "http://tizen.org/privilege/mediastorage";
        public const string ScreenWidthFeature = "http://tizen.org/feature/screen.width";
        public const string LogTag = "FastQR";
        public const string Extension = ".fastqr";

        public static string GetTransformedFile(string file) => file + Extension;

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
