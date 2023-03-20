using System;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using UnitedSets.Classes.PreservedDataClasses;
using UnitedSets.Services;
using Windows.UI;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8602 // 
#pragma warning disable CS8601 // Possible null reference assignment.
namespace UnitedSets.Services {
	public static class PreservedHelpers {
		
		public static string? BrushToStr(Brush brush) {
			return (brush as SolidColorBrush)?.Color.ToString();
		}

		private static System.Drawing.ColorConverter ColorConvert = new();
		public static string ColorToStr(Color color) => ColorConvert.ConvertToString(color);

		public static Color ConvertToColor(String colorStr) {

			var dcolor = (System.Drawing.Color)ColorConvert.ConvertFromString(colorStr);
			return Color.FromArgb(dcolor.A, dcolor.R, dcolor.G, dcolor.B);
		}
		public static Brush ColorStrToBrush(String colorStr) => new SolidColorBrush(ConvertToColor(colorStr));

		public static Thickness RectToThick(OurRect rect) => RectToThick((OurRect?)rect);
		public static Thickness RectToThick(Nullable<OurRect> rect) => new Thickness(rect?.Left ?? 0, rect?.Top ?? 0, rect?.Right ?? 0, rect?.Bottom ?? 0);
		public static CornerRadius RectToCornerRadius(Nullable<OurRect> rect) => new CornerRadius(rect?.Left ?? 0, rect?.Top ?? 0, rect?.Right ?? 0, rect?.Bottom ?? 0);
		

		public static RET_TYPE DoOrThrow<SRC_TYPE, RET_TYPE>(SRC_TYPE? data, Func<SRC_TYPE, RET_TYPE?> dataFunc, Action<SRC_TYPE, RET_TYPE> OnSuccess, Func<Exception>? OnFail = null) where SRC_TYPE : class {
			if (data == null)
				return default;
			var ret = dataFunc(data);
			if (ret == null)
				throw OnFail?.Invoke() ?? new ArgumentException();
			OnSuccess(data, ret);
			return ret;
		}
		public static RET_TYPE DoOrThrow<SRC_TYPE, RET_TYPE>(SRC_TYPE? data, Func<SRC_TYPE, RET_TYPE?> dataFunc, Action<SRC_TYPE, RET_TYPE> OnSuccess, Func<Exception>? OnFail = null) where SRC_TYPE : struct {
			if (data == null)
				return default;

			var ret = dataFunc(data.Value);
			if (ret == null)
				throw OnFail?.Invoke() ?? new ArgumentException();
			OnSuccess(data.Value, ret);
			return ret;
		}


		public static SRC_TYPE DoOrThrow<SRC_TYPE>(SRC_TYPE? data, Action<SRC_TYPE> OnSuccess, bool MustBeTrue = true, Func<Exception>? OnFail = null) where SRC_TYPE : class {
			if (data == null)
				return default;
			if (!MustBeTrue)
				throw OnFail?.Invoke() ?? new ArgumentException();
			OnSuccess(data);
			return data;
		}
		public static SRC_TYPE DoOrThrow<SRC_TYPE>(SRC_TYPE? data, Action<SRC_TYPE> OnSuccess, bool MustBeTrue = true, Func<Exception>? OnFail = null) where SRC_TYPE : struct {
			if (data == null)
				return default;
			if (!MustBeTrue)
				throw OnFail?.Invoke() ?? new ArgumentException();
			OnSuccess(data.Value);
			return data.Value;
		}
		private static JsonSerializerOptions json_opts => new() { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull, Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() } };
		public static string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, options: json_opts);
		public static T? Deserialize<T>(string text) => JsonSerializer.Deserialize<T>(text, json_opts);

		public static SRC_TYPE DoOrThrow<SRC_TYPE>(SRC_TYPE? data, Action<SRC_TYPE> OnSuccess, Func<SRC_TYPE, bool> MustBeTrue, Func<Exception>? OnFail = null) where SRC_TYPE : class {
			if (data == null)
				return default;
			var ndata = data;
			if (!MustBeTrue(ndata))
				throw OnFail?.Invoke() ?? new ArgumentException();
			OnSuccess(ndata);
			return ndata;
		}
		public static SRC_TYPE DoOrThrow<SRC_TYPE>(SRC_TYPE? data, Action<SRC_TYPE> OnSuccess, Func<SRC_TYPE, bool> MustBeTrue, Func<Exception>? OnFail = null) where SRC_TYPE : struct {
			if (data == null)
				return default;
			var ndata = data.Value;
			if (!MustBeTrue(ndata))
				throw OnFail?.Invoke() ?? new ArgumentException();
			OnSuccess(ndata);
			return ndata;
		}
	}




}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS8603 //
#pragma warning restore CS8601 //
#pragma warning restore CS8604 //
#pragma warning restore CS8605
#pragma warning restore CS8602
