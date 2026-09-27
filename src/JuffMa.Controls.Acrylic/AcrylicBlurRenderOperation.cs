// SPDX-License-Identifier: MIT
//
// AcrylicBlurRenderOperation.cs is part of AvaloniaAcrylic.
// Copyright (C) 2026 Julian Rossbach
//
// Licensed under the MIT License. See LICENSE.txt in the project root for details.

using Avalonia;
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

    public Rect Bounds => _bounds.Inflate(4.0d);

    public AcrylicBlurRenderOperation(ImmutableExperimentalAcrylicMaterial material, Rect bounds, CornerRadius cornerRadius)
    {
        _material = material;
        _bounds = bounds;
        _cornerRadius = cornerRadius;
    }

    private static SKShader? _acrylicNoiseShader;

    public bool HitTest(Point p) => _bounds.Contains(p);

    public void Render(ImmediateDrawingContext context)
    {
        var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
        if (leaseFeature is null)
        {
            return;
        }

        using var skia = leaseFeature.Lease();

        if (!skia.SkCanvas.TotalMatrix.TryInvert(out var currentInverse))
        {
            return;
        }

        using var background = skia.SkSurface?.Snapshot();
        using var backgroundShader = SKShader.CreateImage(background,
            SKShaderTileMode.Clamp,
            SKShaderTileMode.Clamp,
            currentInverse);

        var shape = Utils.CreateRoundedRect(new SKRect(
            0, 0, (float)_bounds.Width, (float)_bounds.Height), 
            _cornerRadius);

        // Fix rendering in preview
        // This also fixes rendering in other limited contexts, even though it defeats the purpose of the blur effect
        if (skia.GrContext is null)
        {
            using var tmpFilter = SKImageFilter.CreateBlur(3, 3, SKShaderTileMode.Clamp);
            using var tmpPaint = new SKPaint();
            tmpPaint.Shader = backgroundShader;
            tmpPaint.ImageFilter = tmpFilter;

            skia.SkCanvas.DrawRoundRect(shape, tmpPaint);

            return;
        }

        using var blurred = SKSurface.Create(skia.GrContext, false,
            new SKImageInfo(
                (int)Math.Ceiling(_bounds.Width),
                (int)Math.Ceiling(_bounds.Height),
                SKImageInfo.PlatformColorType, SKAlphaType.Premul));

        using var filter = SKImageFilter.CreateBlur(10, 10, SKShaderTileMode.Clamp);
        using var blurPaint = new SKPaint();
        blurPaint.Shader = backgroundShader;
        blurPaint.ImageFilter = filter;

        blurred.Canvas.DrawRoundRect(shape, blurPaint);

        using var blurredImage = blurred.Snapshot();
        using var blurredShader = SKShader.CreateImage(blurredImage);
        using var blurredPaint = new SKPaint();
        blurredPaint.Shader = blurredShader;
        blurredPaint.IsAntialias = true;

        skia.SkCanvas.DrawRoundRect(shape, blurredPaint);

        using var acrylicPaint = new SKPaint();
        acrylicPaint.IsAntialias = true;

        const double noiseOpacity = 0.0225;

        if (_acrylicNoiseShader is null)
        {
            const string resourceName = "Avalonia.Skia.Assets.NoiseAsset_256X256_PNG.png";
            using var stream = typeof(SkiaPlatform).Assembly
                .GetManifestResourceStream(resourceName);

            using var bitmap = SKBitmap.Decode(stream);
            _acrylicNoiseShader = SKShader.CreateBitmap(bitmap, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat)
                .WithColorFilter(Utils.CreateAlphaColorFilter(noiseOpacity));
        }

        var materialColor = _material.MaterialColor.ToSKColor();
        var tintColor = _material.TintColor.ToSKColor();

        using var backdropShader = SKShader.CreateColor(materialColor);
        using var tintShader = SKShader.CreateColor(tintColor);

        using var effectiveTintShader = SKShader.CreateCompose(backdropShader, tintShader);
        using var effectiveShader = SKShader.CreateCompose(effectiveTintShader, _acrylicNoiseShader);

        acrylicPaint.Shader = effectiveShader;
        acrylicPaint.IsAntialias = true;

        skia.SkCanvas.DrawRoundRect(shape, acrylicPaint);
    }

    public bool Equals(ICustomDrawOperation? other)
    {
        return other is AcrylicBlurRenderOperation op &&
               op._bounds.Equals(_bounds) && 
               op._cornerRadius.Equals(_cornerRadius) &&
               op._material.Equals(_material);
    }

    public void Dispose()
    {
        // No unmanaged resources to dispose
    }
}
