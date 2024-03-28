using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test104Master.Base;

namespace Test104Master.Model
{
	public class Point : INPCBase
	{
		private byte _quality;
		private object _value;
		private DateTimeOffset _timestamp;

		public string TypeName { get; set; }
		public int Address { get; set; }
		
		public byte Quality { 
			get => _quality; 
			set {
				_quality = value; 
				NotifyPropertyChanged(nameof(Quality));
			}
		}

		public object Value { 
			get => _value; 
			set { 
				_value = value; 
				NotifyPropertyChanged(nameof(Value));
			}
		}

		public DateTimeOffset Timestamp { 
			get => _timestamp; 
			set { 
				_timestamp = value; 
				NotifyPropertyChanged(nameof(Timestamp));
			}
		}
	}
}
