using lib60870;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Test104Master.ViewModel
{
	public class MainWindowVm : INotifyPropertyChanged
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

		public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
