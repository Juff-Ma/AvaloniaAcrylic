// SPDX-License-Identifier: MIT
//
// App.axaml.cs is part of AvaloniaAcrylic.
// Copyright (C) 2026 Julian Rossbach
//
// Licensed under the MIT License. See LICENSE.txt in the project root for details.

/*
 App.axaml.cs is part of AvaloniaAcrylic.
 Copyright (C) 2026 Julian Rossbach

 Licensed under the MIT License. See LICENSE.txt in the project root for details.
*/

/*
 App.axaml.cs is part of AvaloniaAcrylic.
 Copyright (C) 2026 Julian Rossbach

 Licensed under the MIT License. See LICENSE.txt in the project root for details.
*/

/*
 App.axaml.cs is part of AvaloniaAcrylic.
 Copyright (C) 2026 Julian Rossbach

 Licensed under the MIT License. See LICENSE.txt in the project root for details.
*/
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace AvaloniaAcrylicSample;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}