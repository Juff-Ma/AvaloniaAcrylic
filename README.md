![AvaloniaAcrylic Logo](https://raw.githubusercontent.com/Juff-Ma/AvaloniaAcrylic/refs/heads/main/assets/AvaloniaAcrylic-Mini.png)
# AvaloniaAcrylic
[release-version]: https://img.shields.io/github/v/release/Juff-Ma/AvaloniaAcrylic?sort=date&display_name=tag&style=flat-square&logo=github
[download]: https://img.shields.io/badge/nuget-download-004880?style=flat-square&logo=nuget
[actions]: https://img.shields.io/github/actions/workflow/status/Juff-Ma/AvaloniaAcrylic/push.yml?branch=main&style=flat-square&logo=githubactions&logoColor=FFFFFF
[license]: https://img.shields.io/github/license/Juff-Ma/AvaloniaAcrylic?style=flat-square&logo=opensourceinitiative&logoColor=FFFFFF

[![][release-version]](https://github.com/Juff-Ma/AvaloniaAcrylic/releases/latest)
[![][download]](https://www.nuget.org/packages/JuffMa.Controls.Acrylic/)
[![][actions]](https://github.com/Juff-Ma/AvaloniaAcrylic/actions)
[![][license]](https://github.com/Juff-Ma/AvaloniaAcrylic/blob/main/LICENSE.txt)

## Introduction
AvaloniaAcrylic, or `JuffMa.Controls.Acrylic` as the package is named, is a small library containing a single control: `AcrylicBackdropControl`

What it does is draw an acrylic effect over whatever elements are behind it, optionally with rounded corners. This allows you to create such an effect in whatever way you want.

It uses the same `ExperimentalAcrylicMaterial` as its basis, just like Avalonia's OS based acrylic support.

![Sample App](https://raw.githubusercontent.com/Juff-Ma/AvaloniaAcrylic/refs/heads/main/assets/SampleApp.png)

## Quick Start
First of all you need to install the nuget package:

`dotnet add package JuffMa.Controls.Acrylic`

After that, add the following namespace to the elements you want to use the control in:

`xmlns:acrylic="clr-namespace:JuffMa.Controls.Acrylic;assembly=JuffMa.Controls.Acrylic"`

Then just add the control where you like:

```xaml
<acrylic:AcrylicBackdropControl Width="50" Height="50"/>
```

Or, if you want something more fancy, you can specify a corner radius, effect and a material:

```xaml
<acrylic:AcrylicBackdropControl
    CornerRadius="10">
    <acrylic:AcrylicBackdropControl.Material>
        <ExperimentalAcrylicMaterial 
            TintColor="Black"
            TintOpacity="0.75"
            MaterialOpacity="0.3"/>
    </acrylic:AcrylicBackdropControl.Material>
    <acrylic:AcrylicBackdropControl.Effect>
        <DropShadowEffect BlurRadius="10" 
                          Color="Black"
                          Opacity="0.3"
                          OffsetX="3"
                          OffsetY="5"/>
    </acrylic:AcrylicBackdropControl.Effect>
</acrylic:AcrylicBackdropControl>
```

Note that the control cannot take children. The easiest way to work around this is to just wrap it in a panel.

## More Info

Since `AcrylicBackdropControl` inherits from `Control` you can use most basic properties like `Effect`. Additionally, as mentioned above, `CornerRadius` is built in.

`Material` takes an `ExperimentalAcrylicMaterial` but only `TintColor`, `TintOpacity` and `MaterialOpacity` are used. The first one is self describing.
The difference between `TintOpacity` and `MaterialOpacity` is that the latter is far more powerful and the former is used for fine adjustments.
Even the difference between `TintOpacity` at `1.0` and `0.0` is only a difference in saturation.

If you want to know more, you can find the example app (see image at the top) in the `src` directory.

## Technical properties

In essence, it is just multiple Skia shaders overlayed with a paint that uses a blurred screenshot of the background as its base. This sounds heavy, and it kinda is, but it's not too bad since Skia is GPU accellerated.
And let's be real, for UI like this you should never worry about performance too much.

This is 90% based on [this control by Dani John](https://github.com/rocksdanister/weather/blob/200d11ac9599ae10887d95175d5cd37a4046ea11/src/Drizzle.UI.Avalonia/UserControls/BackdropBlurControl.cs)
and [this gist by Nikita Tsukanov](https://gist.github.com/kekekeks/ac06098a74fe87d49a9ff9ea37fa67bc).

The core change is porting to Avalonia 12. It also cleans up the code a lot.

But AvaloniaAcrylic also introduces two new features: the rounded corners and a fallback software based renderer.
The latter is especially useful in the preview, since it doesn't support GPU accelleration and would either display nothing or crash.

Still you very much want GPU accelleration. Make sure you include the right Skia natives and that your app is configured to enable GPU support. 
(On Linux this might require explicitely setting the renderer priority, see `Program.cs` of example app)
