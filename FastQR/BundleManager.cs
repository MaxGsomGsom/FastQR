using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Tizen.Applications;
using Tizen.NUI;

namespace FastQR
{
    public static class BundleManager
    {
        private const string Key = "widgetState";

        public static WidgetState? Load(Bundle bundle)
        {
            try
            {
                var json = bundle.GetItem<string>(Key);
                return JsonSerializer.Deserialize<WidgetState>(json);
            }
            catch
            {
                return null;
            }
        }

        public static void Save(WidgetState? state, Action<Bundle> setContent)
        {
            if (state == null)
                return;

            var bundle = new Bundle();
            bundle.AddItem(Key, JsonSerializer.Serialize(state));
            setContent(bundle);
        }
    }
}
