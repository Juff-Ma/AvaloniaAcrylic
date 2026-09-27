// SPDX-License-Identifier: MIT
//
// Program.cs is part of AvaloniaAcrylic.
// Copyright (C) 2026 Julian Rossbach
//
// Licensed under the MIT License. See LICENSE.txt in the project root for details.

/*
 Program.cs is part of AvaloniaAcrylic.
 Copyright (C) 2026 Julian Rossbach

 Licensed under the MIT License. See LICENSE.txt in the project root for details.
*/
using Avalonia;
using System;

namespace AvaloniaAcrylicSample;

static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseWaylandWithFallback()
            .With(new X11PlatformOptions { RenderingMode = 
            [X11RenderingMode.Vulkan, X11RenderingMode.Egl, 
                X11RenderingMode.Glx, X11RenderingMode.Software] })
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}
