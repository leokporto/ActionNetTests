using lib60870.CS104;
using lib60870;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using Test104Master.ViewModel;

namespace Test104Master
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		
		private MainWindowVm _mainwindowVm = null;
		

		public MainWindow()
		{
			InitializeComponent();

			_mainwindowVm = this.DataContext as MainWindowVm;
			
		}

		

		private void Connect_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				_mainwindowVm.Open104Connection();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: " + ex.Message);
			}
			
			
			
		}

		private void CloseConnection_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				_mainwindowVm.Close104Connection();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: " + ex.Message);
			}
		}



	}
}
