// SPDX-License-Identifier: MIT
//
// AcrylicBackdropControl.cs is part of AvaloniaAcrylic.
// Copyright (C) 2026 Julian Rossbach
//
// Licensed under the MIT License. See LICENSE.txt in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
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
    public static readonly ExperimentalAcrylicMaterial DefaultMaterial =
        new()
        {
            MaterialOpacity = 0.1,
            TintColor = Colors.White,
            TintOpacity = 0.1,
            PlatformTransparencyCompensationLevel = 0
        };

    /// <summary>
    /// <see cref="ExperimentalAcrylicMaterial"/> property linked with <see cref="Material"/>.
    /// </summary>
    public static readonly StyledProperty<ExperimentalAcrylicMaterial> MaterialProperty =
        AvaloniaProperty.Register<AcrylicBackdropControl, ExperimentalAcrylicMaterial>
            (nameof(Material), defaultValue: DefaultMaterial);

    /// <summary>
    /// <see cref="Avalonia.CornerRadius"/> property linked with <see cref="CornerRadius"/>.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<AcrylicBackdropControl, CornerRadius>
            (nameof(CornerRadius));

    /// <summary>
    /// <see cref="bool"/> property linked with <see cref="ForcePermanentRender"/>.
    /// </summary>
    public static readonly StyledProperty<bool> ForcePermanentRenderProperty =
        AvaloniaProperty.Register<AcrylicBackdropControl, bool>
            (nameof(ForcePermanentRender), defaultValue: false);

    /// <summary>
    /// <see cref="bool"/> property linked with <see cref="ListenToPropertyChanges"/>
    /// </summary>
    public static readonly StyledProperty<bool> ListenToPropertyChangesProperty =
        AvaloniaProperty.Register<AcrylicBackdropControl, bool>
            (nameof(ListenToPropertyChanges), defaultValue: false);


    /// <summary>
    /// <see cref="bool"/> property linked with <see cref="CacheRenderResult"/>
    /// </summary>
    public static readonly StyledProperty<bool> CacheRenderResultProperty =
        AvaloniaProperty.Register<AcrylicBackdropControl, bool>
            (nameof(CacheRenderResult), defaultValue: false);

    /// <summary>
    /// <see cref="ExperimentalAcrylicMaterial"/> used for the acrylic effect.
    /// </summary>
    public ExperimentalAcrylicMaterial Material
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

    /// <summary>
    /// Force the object to be permanently rendered even if no property change occured
    /// </summary>
    public bool ForcePermanentRender
    {
        get => GetValue(ForcePermanentRenderProperty);
        set => SetValue(ForcePermanentRenderProperty, value);
    }

    /// <summary>
    /// Listen to parent and child property changes and force rerender if one occurs.
    /// This does not work with animations but with manual moves
    /// </summary>
    public bool ListenToPropertyChanges
    {
        get => GetValue(ListenToPropertyChangesProperty);
        set => SetValue(ListenToPropertyChangesProperty, value);
    }

    /// <summary>
    /// Cache the shader to be used for rendering. This is not only a performance enhancement.
    /// It can also help to create an effect similar to <see cref="ForcePermanentRender"/> as long as
    /// the background is not animated. You should enable <see cref="ListenToPropertyChanges"/>
    /// since without it, the cache will almost never update.
    /// </summary>
    public bool CacheRenderResult
    {
        get => GetValue(CacheRenderResultProperty);
        set => SetValue(CacheRenderResultProperty, value);
    }

    static AcrylicBackdropControl()
    {
        AffectsRender<AcrylicBackdropControl>(
            MaterialProperty, 
            CornerRadiusProperty,
            ForcePermanentRenderProperty,
            ListenToPropertyChangesProperty,
            CacheRenderResultProperty);
    }

    private TopLevel? _topLevel;
    private bool _enableConstantRendering;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _topLevel = TopLevel.GetTopLevel(this);

        if (ForcePermanentRender)
        {
            OnForcePermanentRenderChanged(true);
        }

        if (ListenToPropertyChanges)
        {
            OnListenToPropertyChangesChanged(true);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _topLevel = null;
        
        OnForcePermanentRenderChanged(false);
        OnListenToPropertyChangesChanged(false);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ForcePermanentRenderProperty)
        {
            OnForcePermanentRenderChanged(change.GetNewValue<bool>());
            return;
        }
        if (change.Property == ListenToPropertyChangesProperty)
        {
            OnListenToPropertyChangesChanged(change.GetNewValue<bool>());
            return;
        }
    }

    void OnForcePermanentRenderChanged(bool value)
    {
        if (value)
        {
            _enableConstantRendering = true;
            _topLevel?.RequestAnimationFrame(OnAnimationFrame);
            return;
        }
        _enableConstantRendering = false;
    }

    void OnListenToPropertyChangesChanged(bool value)
    {
        if (value)
        {
            Utils.AttachListenerToParents(this, OnWatchedPropertyChanged);
            Utils.AttachListenerToChildren(this, OnWatchedPropertyChanged);
            return;
        }
        Utils.DetachListenerFromParents(this, OnWatchedPropertyChanged);
        Utils.DetachListenerFromChildren(this, OnWatchedPropertyChanged);
    }

    private void OnWatchedPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        InvalidateVisual();
    }

    private void OnAnimationFrame(TimeSpan timestamp)
    {
        if (!_enableConstantRendering || _topLevel is null)
            return;

        InvalidateVisual();

        _topLevel.RequestAnimationFrame(OnAnimationFrame);
    }

    /// <summary>
    /// Renders the acrylic effect using the <see cref="Material"/> and <see cref="CornerRadius"/> properties.
    /// </summary>
    /// <param name="context">The drawing context.</param>
    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var material = (ImmutableExperimentalAcrylicMaterial)Material.ToImmutable();

        context.Custom(
            new AcrylicBlurRenderOperation(material, 
                new Rect(Bounds.Size), CornerRadius,
                CacheRenderResult));
    }
}