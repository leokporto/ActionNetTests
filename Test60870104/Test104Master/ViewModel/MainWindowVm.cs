using lib60870;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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

			StatusMsg = $"Using lib60870.NET version {LibraryCommon.GetLibraryVersionString()}.\n";
		}

        ~MainWindowVm()
		{
			_protocol.ConnectionStatusChanged -= ConnectionStatusChanged;
		}

		private void ConnectionStatusChanged(string status)
		{
			StatusMsg += $"{status}\n";
		}

		public string StatusMsg
		{
			get => _statusMsg;
            set { 
                _statusMsg = value;
                NotifyPropertyChanged(nameof(StatusMsg));
            }
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
