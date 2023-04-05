using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using UnitedSets.Classes.PreservedDataClasses;
using UnitedSets.Classes.Tabs;
using Windows.UI;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8602 // 
#pragma warning disable CS8601 // Possible null reference assignment.
namespace UnitedSets.Helpers
{
	public static class PreservedHelpers
	{

		public static string? BrushToStr(Brush brush)
		{
			return (brush as SolidColorBrush)?.Color.ToString();
		}

		private static System.Drawing.ColorConverter ColorConvert = new();
		public static string ColorToStr(Color color) => ColorConvert.ConvertToString(color);

		public static Color ConvertToColor(string colorStr)
		{

			var dcolor = (System.Drawing.Color)ColorConvert.ConvertFromString(colorStr);
			return Color.FromArgb(dcolor.A, dcolor.R, dcolor.G, dcolor.B);
		}
		public static Brush ColorStrToBrush(string colorStr) => new SolidColorBrush(ConvertToColor(colorStr));

		public static Thickness RectToThick(OurRect rect) => RectToThick((OurRect?)rect);
		public static Thickness RectToThick(OurRect? rect) => new Thickness(rect?.Left ?? 0, rect?.Top ?? 0, rect?.Right ?? 0, rect?.Bottom ?? 0);
		public static CornerRadius RectToCornerRadius(OurRect? rect) => new CornerRadius(rect?.Left ?? 0, rect?.Top ?? 0, rect?.Right ?? 0, rect?.Bottom ?? 0);
		public static string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, options: json_opts);
		public static T? Deserialize<T>(string text) => JsonSerializer.Deserialize<T>(text, json_opts);
		private static JsonSerializerOptions json_opts => new() { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull, Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() } };

	}




}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS8603 //
#pragma warning restore CS8601 //
#pragma warning restore CS8604 //
#pragma warning restore CS8605
#pragma warning restore CS8602
