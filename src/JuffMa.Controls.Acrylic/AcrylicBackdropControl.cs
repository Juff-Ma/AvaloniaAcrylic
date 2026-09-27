// SPDX-License-Identifier: MIT
//
// AcrylicBackdropControl.cs is part of AvaloniaAcrylic.
// Copyright (C) 2026 Julian Rossbach
//
// Licensed under the MIT License. See LICENSE.txt in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using SkiaSharp;

namespace JuffMa.Controls.Acrylic;

/// <summary>
/// Control that provides an acrylic backdrop effect using the native <see cref="ExperimentalAcrylicMaterial"/>.  
/// Compared to the default <see cref="ExperimentalAcrylicBorder" />, this control doesn't use the OS provided backdrop, but instead the controls behind it.
/// </summary>
public class AcrylicBackdropControl : Control
{
    /// <summary>
    /// <see cref="ExperimentalAcrylicMaterial"/> used as default for the <see cref="Material"/> property.
    /// </summary>
    public static readonly ImmutableExperimentalAcrylicMaterial DefaultMaterial = 
        (ImmutableExperimentalAcrylicMaterial)new ExperimentalAcrylicMaterial()
        {
            MaterialOpacity = 0.1,
            TintColor = Colors.White,
            TintOpacity = 0.1,
            PlatformTransparencyCompensationLevel = 0
        }.ToImmutable();

    /// <summary>
    /// <see cref="ExperimentalAcrylicMaterial"/> property linked with <see cref="Material"/>.
    /// </summary>
    public static readonly StyledProperty<ExperimentalAcrylicMaterial?> MaterialProperty =
        AvaloniaProperty.Register<AcrylicBackdropControl, ExperimentalAcrylicMaterial?>(nameof(Material));

    /// <summary>
    /// <see cref="CornerRadius"/> property linked with <see cref="CornerRadius"/>.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<AcrylicBackdropControl, CornerRadius>(nameof(CornerRadius));

    /// <summary>
    /// <see cref="ExperimentalAcrylicMaterial"/> used for the acrylic effect.
    /// </summary>
    public ExperimentalAcrylicMaterial? Material
    {
        get => GetValue(MaterialProperty);
        set => SetValue(MaterialProperty, value);
    }

    /// <summary>
    /// <see cref="CornerRadius"/> used for the acrylic effect.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    static AcrylicBackdropControl()
    {
        AffectsRender<AcrylicBackdropControl>(MaterialProperty, CornerRadiusProperty);
    }

    /// <summary>
    /// Renders the acrylic effect using the <see cref="Material"/> and <see cref="CornerRadius"/> properties.
    /// </summary>
    /// <param name="context">The drawing context.</param>
    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var material = Material is not null
            ? (ImmutableExperimentalAcrylicMaterial)Material.ToImmutable()
            : DefaultMaterial;

        context.Custom(
            new AcrylicBlurRenderOperation(material, 
                new Rect(Bounds.Size), CornerRadius));
    }
}