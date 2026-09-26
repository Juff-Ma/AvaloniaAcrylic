// SPDX-License-Identifier: MIT
//
// AcrylicBlurRenderOperation.cs is part of AvaloniaAcrylic.
// Copyright (C) 2026 Julian Rossbach
//
// Licensed under the MIT License. See LICENSE.txt in the project root for details.

using Avalonia;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
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

    private static SKShader? acrylicNoiseShader;

    public bool HitTest(Point p) => _bounds.Contains(p);

    public void Render(ImmediateDrawingContext context)
    {
        throw new NotImplementedException();
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
