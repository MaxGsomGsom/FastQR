using System;
using Tizen.Applications;

namespace FastQR
{
    public static class BundleManager
    {
        private const string Key = "widgetState";

        public static string? Load(Bundle bundle)
        {
            try
            {
                return bundle.GetItem<string>(Key);
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
            bundle.AddItem(Key, file);
            setContent(bundle);
        }
    }
}
