using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SPIN.Wpf.Controls.OTSAux.TemplateSelectors
{
	public sealed class JPropertyDataTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if(item == null)
				return null;

			FrameworkElement frameworkElement = container as FrameworkElement;

			if(frameworkElement == null)
				return null;

			if (item is JProperty jProp) 
			{
				switch (jProp.Value.Type) 
				{
					case JTokenType.Object:
						return frameworkElement.FindResource("ObjectPropertyTemplate") as DataTemplate;
					case JTokenType.Array:
						return frameworkElement.FindResource("ArrayPropertyTemplate") as DataTemplate;						
					default:
						return frameworkElement.FindResource("PrimitivePropertyTemplate") as DataTemplate;
				}
			}

			return null;

			/*
			 var key = new DataTemplateKey(type);
            return frameworkElement.FindResource(key) as DataTemplate;
			 */
		}
	}
}
