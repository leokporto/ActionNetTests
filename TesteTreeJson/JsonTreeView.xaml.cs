using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SPIN.Wpf.Controls.OTSAux
{
	/// <summary>
	/// Interaction logic for JsonTreeView.xaml
	/// </summary>
	public partial class JsonTreeView : UserControl
	{
		private const GeneratorStatus Generated = GeneratorStatus.ContainersGenerated;
		private DispatcherTimer _timer;

		public JsonTreeView()
		{
			InitializeComponent();
		}

		public void Load(List<JToken> items)
		{
			if (items == null)
			{
				JsonTreeView1.ItemsSource = null;
				JsonTreeView1.Items.Clear();
				return;
			}

			JsonTreeView1.ItemsSource = items;

			ToggleItems(true);
		}


		private void JValue_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			//if (e.ClickCount != 2)
			//	return;

			//var tb = sender as TextBlock;
			//if (tb != null)
			//{
			//	Clipboard.SetText(tb.Text);
			//}
		}

		private void ExpandAll(object sender, RoutedEventArgs e)
		{
			ToggleItems(true);
		}

		private void CollapseAll(object sender, RoutedEventArgs e)
		{
			ToggleItems(false);
		}

		private void ToggleItems(bool isExpanded)
		{
			if (JsonTreeView1.Items.IsEmpty)
				return;

			var prevCursor = Cursor;
			//System.Windows.Controls.DockPanel.Opacity = 0.2;
			//System.Windows.Controls.DockPanel.IsEnabled = false;
			Cursor = Cursors.Wait;
			_timer = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Normal, delegate
			{
				ToggleItems(JsonTreeView1, JsonTreeView1.Items, isExpanded);
				//System.Windows.Controls.DockPanel.Opacity = 1.0;
				//System.Windows.Controls.DockPanel.IsEnabled = true;
				_timer.Stop();
				Cursor = prevCursor;
			}, Application.Current.Dispatcher);
			_timer.Start();
		}

		private void ToggleItems(ItemsControl parentContainer, ItemCollection items, bool isExpanded)
		{
			var itemGen = parentContainer.ItemContainerGenerator;
			if (itemGen.Status == Generated)
			{
				Recurse(items, isExpanded, itemGen);
			}
			else
			{
				itemGen.StatusChanged += delegate
				{
					Recurse(items, isExpanded, itemGen);
				};
			}
		}

		private void Recurse(ItemCollection items, bool isExpanded, ItemContainerGenerator itemGen)
		{
			if (itemGen.Status != Generated)
				return;

			foreach (var item in items)
			{
				var tvi = itemGen.ContainerFromItem(item) as TreeViewItem;
				tvi.IsExpanded = isExpanded;
				ToggleItems(tvi, tvi.Items, isExpanded);
			}
		}
	}
}
