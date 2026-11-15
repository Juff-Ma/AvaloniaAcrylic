// SPDX-License-Identifier: MIT
//
// AcrylicBlurRenderOperation.cs is part of AvaloniaAcrylic.
// Copyright (C) 2026 Julian Rossbach
//
// Licensed under the MIT License. See LICENSE.txt in the project root for details.

using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace JuffMa.Controls.Acrylic;

internal sealed class AcrylicBlurRenderOperation : ICustomDrawOperation
{
    private readonly ImmutableExperimentalAcrylicMaterial _material;
    private readonly Rect _bounds;
    private readonly CornerRadius _cornerRadius;
    private readonly bool _cacheRenderResult;

    public Rect Bounds => _bounds.Inflate(4.0d);

    public AcrylicBlurRenderOperation(
        ImmutableExperimentalAcrylicMaterial material,
        Rect bounds,
        CornerRadius cornerRadius,
        bool cacheRenderResult
        )
    {
        _material = material;
        _bounds = bounds;
        _cornerRadius = cornerRadius;
        _cacheRenderResult = cacheRenderResult;
    }

    private static readonly SKShader AcrylicNoiseShader = Utils.CreateAcrylicNoiseShader(0.0225);

    public bool HitTest(Point p) => _bounds.Contains(p);

    public void Render(ImmediateDrawingContext context)
    {
        var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
        if (leaseFeature is null)
        {
            return;
        }

        using var skia = leaseFeature.Lease();

        var shape = Utils.CreateRoundedRect(new SKRect(
                0, 0, (float)_bounds.Width, (float)_bounds.Height),
            _cornerRadius);

        bool result = true;
        if (!_cacheRenderResult || 
            _acrylicPaint is null || 
            _blurredPaint is null)
        {
            result = PrepareShaders(skia, shape);
        }

        if (!result)
        {
            return;
        }

        skia.SkCanvas.DrawRoundRect(shape, _blurredPaint);
        skia.SkCanvas.DrawRoundRect(shape, _acrylicPaint);
    }

    private bool PrepareShaders(ISkiaSharpApiLease skia, SKRoundRect shape)
    {
        DisposeSkiaResources();

        if (!skia.SkCanvas.TotalMatrix.TryInvert(out var currentInverse))
        {
            return false;
        }

        _background = skia.SkSurface?.Snapshot();
        _backgroundShader = SKShader.CreateImage(_background,
            SKShaderTileMode.Clamp,
            SKShaderTileMode.Clamp,
            currentInverse);

        var surfaceInfo = new SKImageInfo(
            (int)Math.Ceiling(_bounds.Width),
            (int)Math.Ceiling(_bounds.Height),
            SKImageInfo.PlatformColorType, SKAlphaType.Premul);

        // Use GPU accel if available, otherwise fallback to CPU rendering
        _blurred = skia.GrContext is null ?
            SKSurface.Create(surfaceInfo) :
            SKSurface.Create(skia.GrContext, false, surfaceInfo);

        // This happens if the size of the control is <1x1, which can happen in some cases (e.g. when the control is not visible)
        // In that case we just return since if it's not visible, we don't need to render anything.
        if (_blurred is null)
        {
            return false;
        }

        _filter = SKImageFilter.CreateBlur(10, 10, SKShaderTileMode.Clamp);
        _blurPaint = new SKPaint();
        _blurPaint.Shader = _backgroundShader;
        _blurPaint.ImageFilter = _filter;

        _blurred.Canvas.DrawRoundRect(shape, _blurPaint);

        _blurredImage = _blurred.Snapshot();
        _blurredShader = SKShader.CreateImage(_blurredImage);
        _blurredPaint = new SKPaint();
        _blurredPaint.Shader = _blurredShader;
        _blurredPaint.IsAntialias = true;

        _acrylicPaint = new SKPaint();
        _acrylicPaint.IsAntialias = true;

        var materialColor = _material.MaterialColor.ToSKColor();
        var tintColor = _material.TintColor.ToSKColor();

        _backdropShader = SKShader.CreateColor(materialColor);
        _tintShader = SKShader.CreateColor(tintColor);

        _effectiveTintShader = SKShader.CreateCompose(_backdropShader, _tintShader);
        _effectiveShader = SKShader.CreateCompose(_effectiveTintShader, AcrylicNoiseShader);

        _acrylicPaint.Shader = _effectiveShader;
        _acrylicPaint.IsAntialias = true;

        return true;
    }

    public bool Equals(ICustomDrawOperation? other)
    {
        return other is AcrylicBlurRenderOperation op &&
               op._bounds.Equals(_bounds) && 
               op._cornerRadius.Equals(_cornerRadius) &&
               op._material.Equals(_material) &&
               op._cacheRenderResult == _cacheRenderResult;
    }

    private SKImage? _background;
    private SKShader? _backgroundShader;
    private SKSurface? _blurred;
    private SKImageFilter? _filter;
    private SKPaint? _blurPaint;
    private SKImage? _blurredImage;
    private SKShader? _blurredShader;
    private SKPaint? _blurredPaint;
    private SKPaint? _acrylicPaint;
    private SKShader? _backdropShader;
    private SKShader? _tintShader;
    private SKShader? _effectiveTintShader;
    private SKShader? _effectiveShader;
    
    private void DisposeSkiaResources()
    {
        _background?.Dispose();
        _background = null;

        _backgroundShader?.Dispose();
        _backgroundShader = null;

        _blurred?.Dispose();
        _blurred = null;

        _filter?.Dispose();
        _filter = null;

        _blurPaint?.Dispose();
        _blurPaint = null;

        _blurredImage?.Dispose();
        _blurredImage = null;

        _blurredShader?.Dispose();
        _blurredShader = null;

        _blurredPaint?.Dispose();
        _blurredPaint = null;

        _acrylicPaint?.Dispose();
        _acrylicPaint = null;

        _backdropShader?.Dispose();
        _backdropShader = null;

        _tintShader?.Dispose();
        _tintShader = null;

        _effectiveTintShader?.Dispose();
        _effectiveTintShader = null;

        _effectiveShader?.Dispose();
        _effectiveShader = null;
    }
    public void Dispose()
    {
        DisposeSkiaResources();
    }
}
