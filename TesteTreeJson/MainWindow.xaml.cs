using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPIN.Wpf.Controls.OTSAux
{
	/*
	 {"coord":{"lon":10.99,"lat":44.34},"weather":[{"id":501,"main":"Rain","description":"moderate rain","icon":"10d"}],"base":"stations","main":{"temp":298.48,"feels_like":298.74,"temp_min":297.56,"temp_max":300.05,"pressure":1015,"humidity":64,"sea_level":1015,"grnd_level":933},"visibility":10000,"wind":{"speed":0.62,"deg":349,"gust":1.18},"rain":{"1h":3.16},"clouds":{"all":100},"dt":1661870592,"sys":{"type":2,"id":2075663,"country":"IT","sunrise":1661834187,"sunset":1661882248},"timezone":7200,"id":3163858,"name":"Zocca","cod":200}
	 */
	//https://bitbucket.org/rasmuszimmer/wpf-jsonviewer-usercontrol/src/master/JsonViewerDemo/JsonViewer/JsonViewer.xaml

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private string jsonString;
		private object treeDataSource;

		public MainWindow()
		{
			InitializeComponent();
			this.DataContext = this;
		}


		public string JsonString { get => jsonString;
			set { 
				jsonString = value;
				NotifyPropertyChanged(nameof(JsonString));	
			}
		}

		public object TreeDataSource { get => treeDataSource; 
			set  { 
				treeDataSource = value;
				NotifyPropertyChanged(nameof(TreeDataSource));
			} 
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

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			//dynamic ola = JsonSerializer.Deserialize(JsonString);

			//var ola2 = JsonConvert.DeserializeObject(JsonString);
			//TreeDataSource = JsonConvert.DeserializeObject(JsonString);


			var children = new List<JToken>();

			try
			{
				var token = JToken.Parse(JsonString);

				if (token != null)
				{
					children.Add(token);
				}

				TreeView1.Load(children);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Could not open the JSON string:\r\n" + ex.Message);
			}
		}
	}
}
