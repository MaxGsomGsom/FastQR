using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FastQR
{
    public sealed class WidgetState
    {
        public WidgetState(string file)
        {
            File = file;
        }

        public string File { get; }
        public string TransformedFile => File + ".transformed";
        public float TranslateX { get; set; } = 0;
        public float TranslateY { get; set; } = 0;
        public float Zoom { get; set; } = 1;
    }
}
