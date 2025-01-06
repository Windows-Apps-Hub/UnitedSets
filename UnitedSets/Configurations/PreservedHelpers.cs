using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8605 // Unboxing a possibly null value.
namespace UnitedSets.Configurations;

public static class PreservedHelpers
{

    public static string? BrushToStr(Brush brush)
    {
        return (brush as SolidColorBrush)?.Color.ToString();
    }

    private static System.Drawing.ColorConverter ColorConvert = new();
    public static string ColorToStr(Color color) => ColorConvert.ConvertToString(color);

    public static Color ConvertToColor(string? colorStr)
    {
        if (colorStr is null) return default;
        var dcolor = (System.Drawing.Color)ColorConvert.ConvertFromString(colorStr);
        return Color.FromArgb(dcolor.A, dcolor.R, dcolor.G, dcolor.B);
    }
    public static Brush ColorStrToBrush(string colorStr) => new SolidColorBrush(ConvertToColor(colorStr));

    public static Thickness RectToThick(OurRect rect) => RectToThick((OurRect?)rect);
    public static Thickness RectToThick(OurRect? rect) => new Thickness(rect?.Left ?? 0, rect?.Top ?? 0, rect?.Right ?? 0, rect?.Bottom ?? 0);
    public static OurRect ThickToRect(Thickness rect) => new OurRect((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom);
    public static CornerRadius RectToCornerRadius(OurRect? rect) => new CornerRadius(rect?.Left ?? 0, rect?.Top ?? 0, rect?.Right ?? 0, rect?.Bottom ?? 0);
    public static OurRect RectToCornerRadius(CornerRadius rect) => new((int)rect.TopLeft, (int)rect.TopRight, (int)rect.BottomLeft, (int)rect.BottomRight);
    public static string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, options: json_opts);
    public static T? Deserialize<T>(string text) => JsonSerializer.Deserialize<T>(text, json_opts);
    private static JsonSerializerOptions json_opts => new() { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull, Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() } };

}

#pragma warning restore CS8603 //
#pragma warning restore CS8605
