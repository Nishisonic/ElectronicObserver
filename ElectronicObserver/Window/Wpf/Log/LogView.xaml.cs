using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ElectronicObserver.Window.Wpf.Log;
/// <summary>
/// Interaction logic for LogView.xaml
/// </summary>
public partial class LogView : UserControl
{ 
	public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
"ViewModel", typeof(LogViewViewModel), typeof(LogView), new PropertyMetadata(default(LogViewViewModel)));

	public LogViewViewModel ViewModel
	{
		get => (LogViewViewModel)GetValue(ViewModelProperty);
		set => SetValue(ViewModelProperty, value);
	}

	public LogView()
	{
		InitializeComponent();
	}

	private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
	{
		if (e.ExtentHeightChange != 0)
		{
			ScrollViewer.ScrollToVerticalOffset(ScrollViewer.ExtentHeight);
		}
	}
}
