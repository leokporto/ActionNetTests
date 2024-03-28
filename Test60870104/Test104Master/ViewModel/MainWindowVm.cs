using lib60870;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Test104Master.Base;

namespace Test104Master.ViewModel
{
	public class MainWindowVm : INPCBase
	{
		private string _statusMsg;

        private Protocol _protocol = new Protocol();

        public MainWindowVm()
		{
			_protocol.ConnectionStatusChanged += ConnectionStatusChanged;
			_protocol.PointsReceived += Protocol_PointsReceived;

			StatusMsg = $"Using lib60870.NET version {LibraryCommon.GetLibraryVersionString()}.\n";
		}

		~MainWindowVm()
		{
			_protocol.ConnectionStatusChanged -= ConnectionStatusChanged;
			_protocol.PointsReceived -= Protocol_PointsReceived;
		}

		public string StatusMsg
		{
			get => _statusMsg;
			set
			{
				_statusMsg = value;
				NotifyPropertyChanged(nameof(StatusMsg));
			}
		}

		public ObservableCollection<Model.Point> Points { get; set; } = new ObservableCollection<Model.Point>();

		private void Protocol_PointsReceived(List<Model.Point> points)
		{
			if(points == null || points.Count == 0)
			{				
				return;
			}

			Application.Current.Dispatcher.Invoke(() =>
			{
				foreach (var point in points)
				{
					if (Points.Any(p => string.Equals(p.Key, point.Key, StringComparison.OrdinalIgnoreCase)))
					{
						var existingPoint = Points.First(p => string.Equals(p.Key, point.Key, StringComparison.OrdinalIgnoreCase));
						existingPoint.Quality = point.Quality;
						existingPoint.Value = point.Value;
						existingPoint.Timestamp = point.Timestamp;
					}
					else
					{
						Points.Add(point);
					}
				}
			});
		}

		private void ConnectionStatusChanged(string status)
		{
			StatusMsg += $"{status}\n";
		}

		

        public void Open104Connection()
		{
			
			_protocol.OpenConnection();			
		}

		public void Close104Connection()
		{
			_protocol.CloseConnection();
		}		

    }
}
