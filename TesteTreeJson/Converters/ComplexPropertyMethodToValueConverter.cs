﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SPIN.Wpf.Controls.OTSAux.Converters
{
	public class ComplexPropertyMethodToValueConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var methodName = parameter as string;
			if (value == null || methodName == null)
				return null;
			var methodInfo = value.GetType().GetMethod(methodName, new Type[0]);
			if (methodInfo == null)
				return null;
			var invocationResult = methodInfo.Invoke(value, new object[0]);
			var jTokens = (IEnumerable<JToken>)invocationResult;
			return jTokens.First().Children();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException(GetType().Name + " can only be used for one way conversion.");
		}

	}
}
