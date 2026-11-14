// SPDX-License-Identifier: MIT
//
// MainWindow.axaml.cs is part of AvaloniaAcrylic.
// Copyright (C) 2026 Julian Rossbach
//
// Licensed under the MIT License. See LICENSE.txt in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using System;

namespace AvaloniaAcrylicSample;

public partial class MainWindow : Window
{
    // used for animating the carousel
    private DispatcherTimer? _autoSlideTimer;

    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        _autoSlideTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(4)
        };

        _autoSlideTimer.Tick += (s, e) =>
        {
            if (carousel.ItemCount == 0) return;

            // Loop back to 0 when reaching the last item
            carousel.SelectedIndex = (carousel.SelectedIndex + 1) % carousel.ItemCount;
        };

        _autoSlideTimer.Start();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _autoSlideTimer?.Stop();
        _autoSlideTimer = null;
    }
}