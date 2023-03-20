using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
using System.Linq;
namespace UnitedSets.Controls {
	
	public class NullableBoolConverter : IValueConverter {
		

		public object Convert(object value, Type targetType, object parameter, string language) {
			if (targetType != typeof(Boolean))
				throw new Exception("Wrong target type");
			if (!value.Equals(true))
				return false;
			return true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			return (bool)value;
		}
	}
	public class EnumToStringConverter : IValueConverter {
		public Type EnumType { get; set; }
		public object Convert(object value, Type targetType, object parameter, string language) {
			if (value is Enum)
				return value.ToString();
			else if (value is string)
				return value;
			else
				throw new NotImplementedException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			if (value is Enum)
				return value;
			else if (value is string s)
				return Enum.Parse(EnumType, s);
			else if (value == null)
				return Enum.ToObject(EnumType, 0);
			else
				throw new NotImplementedException();
		}
	}
}
