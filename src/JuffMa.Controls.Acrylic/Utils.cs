// SPDX-License-Identifier: MIT
//
// Utils.cs is part of AvaloniaAcrylic.
// Copyright (C) 2026 Julian Rossbach
//
// Licensed under the MIT License. See LICENSE.txt in the project root for details.

using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.Intrinsics;
using Avalonia;
using SkiaSharp;

namespace JuffMa.Controls.Acrylic;

internal static class Utils
{
    private const int FilterLen = 256;

    private static readonly ImmutableArray<byte> IndexTable =
    [
        .. Enumerable.Range(0, FilterLen - 1)
            .Select(i => (byte)i)
    ];

    /// <summary>
    /// Creates a color filter that applies an alpha value to the colors of an image.
    /// </summary>
    /// <param name="opacity">The opacity to apply to the image.</param>
    /// <returns>The created color filter.</returns>
    public static SKColorFilter CreateAlphaColorFilter(double opacity)
    {
        opacity = Math.Clamp(opacity, 0, 1);

        var c = IndexTable.AsSpan();
        Span<byte> a = stackalloc byte[FilterLen];

        for (var i = 0; i < FilterLen; i++)
        {
            a[i] = (byte)(i * opacity);
        }

        return SKColorFilter.CreateTable(a, c, c, c);
    }

    /// <summary>
    /// Creates a rounded rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to round.</param>
    /// <param name="cornerRadius">The radius of the corners.</param>
    /// <returns>The created rounded rectangle.</returns>
    public static SKRoundRect CreateRoundedRect(SKRect rect, CornerRadius cornerRadius)
    {
        var topLeft = (float)cornerRadius.TopLeft;
        var topRight = (float)cornerRadius.TopRight;
        var bottomRight = (float)cornerRadius.BottomRight;
        var bottomLeft = (float)cornerRadius.BottomLeft;

        Span<SKPoint> radii = stackalloc SKPoint[4];

        radii[0] = new(topLeft, topLeft);
        radii[1] = new(topRight, topRight);
        radii[2] = new(bottomRight, bottomRight);
        radii[3] = new(bottomLeft, bottomLeft);

        SKRoundRect roundRect = new();

        roundRect.SetRectRadii(rect, radii);

        return roundRect;
    }
}
