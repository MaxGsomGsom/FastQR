// Copyright 2021 Russian Post
// This source code is Russian Post Confidential Proprietary.
// This software is protected by copyright. All rights and titles are reserved.
// You shall not use, copy, distribute, modify, decompile, disassemble or reverse engineer the software.
// Otherwise this violation would be treated by law and would be subject to legal prosecution.
// Legal use of the software provides receipt of a license from the right holder only.

using Tizen.Applications;
using ElmSharp;

namespace FastQR
{
    class Program
    {
        static void Main(string[] args)
        {
            Elementary.Initialize();
            Elementary.ThemeOverlay();
            WidgetApplication app = new(typeof(FastQRWidget));
            app.Run(args);
        }
    }
}
