using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Resource.Record;
using ElectronicObserver.Utility;
using ElectronicObserver.ViewModels;
using ScottPlot;
using ScottPlot.Plottable;
using Translation = ElectronicObserver.Properties.Window.Dialog.DialogResourceChart;

namespace ElectronicObserver.Window.Dialog.ResourceChartWPF;
/// <summary>
/// Interaction logic for ResourceChartWPF.xaml
/// </summary>
public partial class ResourceChartWPF
{
	private enum ChartType
	{
		Resource,
		ResourceDiff,
		Material,
		MaterialDiff,
		Experience,
		ExperienceDiff,
	}

	private enum ChartSpan
	{
		Day,
		Week,
		Month,
		Season,
		Year,
		All,
		WeekFirst,
		MonthFirst,
		SeasonFirst,
		YearFirst,
	}
	private MarkerPlot HighlightedPoint;
	private int LastHighlightedIndex = -1;
	private ScatterPlot? FuelPlot;
	private ScatterPlot? AmmoPlot;
	private ScatterPlot? SteelPlot;
	private ScatterPlot? BauxPlot;
	private ScatterPlot? InstantRepairPlot;
	private ScatterPlot? InstantConstructionPlot;
	private ScatterPlot? ModdingMaterialPlot;
	private ScatterPlot? DevelopmentMaterialPlot;
	private ToolTip toolTip;
	private ChartType SelectedChartType => (ChartType)GetSelectedMenuStripIndex(ChartTypeMenu);
	private ChartSpan SelectedChartSpan => (ChartSpan)GetSelectedMenuStripIndex(ChartSpanMenu);
	public ResourceChartWPF()
	{
		InitializeComponent();
		Loaded += ChartArea_Loaded;
	}

	private void ChartArea_Loaded(object sender, RoutedEventArgs e)
	{
		if (!RecordManager.Instance.Resource.Record.Any())
		{
			System.Windows.Forms.MessageBox.Show(Translation.RecordDataDoesNotExist, Translation.Error, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			Close();
			return;
		}
		//toolTip = new ToolTip();
		//toolTip.Content = "ToolTip";

		//ChartArea.ToolTip = toolTip;
		SwitchMenuStrip(ChartSpanMenu, "5");
		SwitchMenuStrip(ChartTypeMenu, "0");
		ChartArea.Plot.Style(ScottPlot.Style.Black);
		ChartArea.Configuration.Zoom = false;
		ChartArea.Configuration.Pan = false;
		ChartArea.Configuration.DoubleClickBenchmark = false;
		UpdateChart();
	}
	/// <summary>
	/// Chart onhover handler
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void ChartArea_MouseMove(object sender, MouseEventArgs e)
	{
		// determine point nearest the cursor
		//(double mouseCoordX, double mouseCoordY) = ChartArea.GetMouseCoordinates();
		//double xyRatio = ChartArea.Plot.XAxis.Dims.PxPerUnit / ChartArea.Plot.YAxis.Dims.PxPerUnit;
		//(double pointX, double pointY, int pointIndex) = FuelPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
		// place the highlight over the point of interest
		//HighlightedPoint.X = pointX;
		//HighlightedPoint.Y = pointY;
		//HighlightedPoint.IsVisible = true;
		// render if the highlighted point chnaged
		//if (LastHighlightedIndex != pointIndex)
		//{
		//	LastHighlightedIndex = pointIndex;
		//	ChartArea.Refresh();
		//}

		// update the GUI to describe the highlighted point
		double mouseX = e.GetPosition(this).X;
		double mouseY = e.GetPosition(this).Y;

		//toolTip.HorizontalOffset = mouseX;
		//toolTip.VerticalOffset = mouseY-20;
		//toolTip.Content = "Fuel: " + pointY;
		//HighlightedPoint.Text = "Fuel: " + pointY;
	}
	private void SetResourceChart()
	{
		ChartArea.Plot.Clear();

		ChartArea.Plot.XAxis.Label("Date");
		ChartArea.Plot.YAxis.Label("Resource");
		ChartArea.Plot.XAxis.DateTimeFormat(true);
		ChartArea.Plot.Legend(true, Alignment.LowerLeft);
		AxisXIntervals(SelectedChartSpan);
		FuelCheck.IsChecked = true;
		AmmoCheck.IsChecked = true;
		BauxCheck.IsChecked = true;
		ResourcesPanel.Visibility = Visibility.Visible;
		MaterialPanel.Visibility = Visibility.Collapsed;
		InstantRepairCheck.IsChecked = true;
		List<double>? fuel_list = Array.Empty<double>().ToList();

		List<double>? ammo_list = Array.Empty<double>().ToList();

		List<double>? baux_list = Array.Empty<double>().ToList();

		List<double>? steel_list = Array.Empty<double>().ToList();

		List<double>? instant_repair_list = Array.Empty<double>().ToList();
		ChartArea.Plot.YAxis2.Ticks(true);
		ChartArea.Plot.YAxis2.MajorGrid(true);
		List<double>? date_list = Array.Empty<double>().ToList();

		{
			var record = GetRecords();

			ResourceRecord.ResourceElement prev = null;
			if (record.Any())
			{
				prev = record.First();
				foreach (var r in record)
				{
					if (ShouldSkipRecord(r.Date - prev.Date))
						continue;

					date_list.Add(r.Date.ToOADate());
					fuel_list.Add(r.Fuel);
					ammo_list.Add(r.Ammo);
					baux_list.Add(r.Bauxite);
					steel_list.Add(r.Steel);
					instant_repair_list.Add(r.InstantRepair);

					prev = r;
				}
			}
		}
		FuelPlot = ChartArea.Plot.AddScatterLines(date_list.ToArray(), fuel_list.ToArray(), System.Drawing.Color.DarkGreen, label: "Fuel");
		AmmoPlot = ChartArea.Plot.AddScatterLines(date_list.ToArray(), ammo_list.ToArray(), System.Drawing.Color.Brown, label: "Ammo");
		BauxPlot = ChartArea.Plot.AddScatterLines(date_list.ToArray(), baux_list.ToArray(), System.Drawing.Color.Orange, label: "Bauxite");
		SteelPlot = ChartArea.Plot.AddScatterLines(date_list.ToArray(), steel_list.ToArray(), System.Drawing.Color.AliceBlue, label: "Steel");
		InstantRepairPlot = ChartArea.Plot.AddScatterLines(date_list.ToArray(), instant_repair_list.ToArray(), System.Drawing.Color.Green, label: "Instant Repair");
		InstantRepairPlot.YAxisIndex = 1;

		// Add a red circle we can move around later as a highlighted point indicator
		//HighlightedPoint = ChartArea.Plot.AddPoint(0, 0);
		//HighlightedPoint.Color = System.Drawing.Color.Red;
		//HighlightedPoint.MarkerSize = 10;
		//HighlightedPoint.MarkerShape = MarkerShape.none;
		//HighlightedPoint.IsVisible = false;
		ChartArea.Refresh();
	}

	private bool ShouldSkipRecord(TimeSpan span)
	{
		//if (Menu_Option_ShowAllData.Checked)
		//return false;

		if (span.Ticks == 0)        //初回のデータ( prev == First )は無視しない
			return false;

		switch (SelectedChartSpan)
		{
			case ChartSpan.Day:
			case ChartSpan.Week:
			case ChartSpan.WeekFirst:
			default:
				return false;

			case ChartSpan.Month:
			case ChartSpan.MonthFirst:
				return span.TotalHours < 12.0;

			case ChartSpan.Season:
			case ChartSpan.SeasonFirst:
			case ChartSpan.Year:
			case ChartSpan.YearFirst:
			case ChartSpan.All:
				return span.TotalDays < 1.0;
		}
	}

	private void SetYBounds(double min, double max)
	{

		int order = (int)Math.Log10(Math.Max(max - min, 1));
		double powered = Math.Pow(10, order);
		double unitbase = Math.Round((max - min) / powered);
		double unit = powered * (
			unitbase < 2 ? 0.2 :
			unitbase < 5 ? 0.5 :
			unitbase < 7 ? 1.0 : 2.0);

		//ResourceChart.ChartAreas[0].AxisY.Minimum = Math.Floor(min / unit) * unit;
		//ResourceChart.ChartAreas[0].AxisY.Maximum = Math.Ceiling(max / unit) * unit;

		//ResourceChart.ChartAreas[0].AxisY.Interval = unit;
		//ResourceChart.ChartAreas[0].AxisY.MinorGrid.Interval = unit / 2;

		//if (ResourceChart.Series.Where(s => s.Enabled).Any(s => s.YAxisType == AxisType.Secondary))
		//{
		//	ResourceChart.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
		//	if (ResourceChart.Series.Count(s => s.Enabled) == 1)
		//	{
		//		ResourceChart.ChartAreas[0].AxisY2.MajorGrid.Enabled = true;
		//		ResourceChart.ChartAreas[0].AxisY2.MinorGrid.Enabled = true;
		//	}
		//	else
		//	{
		//		ResourceChart.ChartAreas[0].AxisY2.MajorGrid.Enabled = false;
		//		ResourceChart.ChartAreas[0].AxisY2.MinorGrid.Enabled = false;
		//	}
		//	ResourceChart.ChartAreas[0].AxisY2.Minimum = ResourceChart.ChartAreas[0].AxisY.Minimum / 100;
		//	ResourceChart.ChartAreas[0].AxisY2.Maximum = ResourceChart.ChartAreas[0].AxisY.Maximum / 100;
		//	ResourceChart.ChartAreas[0].AxisY2.Interval = unit / 100;
		//	ResourceChart.ChartAreas[0].AxisY2.MinorGrid.Interval = unit / 200;
		//}
		//else
		//{
		//	//ResourceChart.ChartAreas[0].AxisY2.Enabled = AxisEnabled.False;
		//}

	}
	private void SetMaterialChart()
	{
		ChartArea.Plot.Clear();
		ResourcesPanel.Visibility = Visibility.Collapsed;
		MaterialPanel.Visibility = Visibility.Visible;
		ChartArea.Plot.YAxis2.IsVisible = false;
		ChartArea.Plot.XAxis.Label("Date");
		ChartArea.Plot.YAxis.Label("Material");
		ChartArea.Plot.XAxis.DateTimeFormat(true);
		AxisXIntervals(SelectedChartSpan);
		ChartArea.Plot.Legend(true, Alignment.LowerLeft);
		ModdingMaterialCheck.IsChecked = true;
		InstantRepairMatCheck.IsChecked = true;
		InstantConstructionMatCheck.IsChecked = true;
		List<double>? instant_repair_list = Array.Empty<double>().ToList();
		List<double>? development_material_list = Array.Empty<double>().ToList();
		List<double>? modding_material_list = Array.Empty<double>().ToList();
		List<double>? instant_contruction_list = Array.Empty<double>().ToList();

		List<double>? date_list = Array.Empty<double>().ToList();
		{
			var record = GetRecords();

			ResourceRecord.ResourceElement prev = null;
			if (record.Any())
			{
				prev = record.First();
				foreach (var r in record)
				{
					if (ShouldSkipRecord(r.Date - prev.Date))
						continue;

					date_list.Add(r.Date.ToOADate());
					instant_repair_list.Add(r.InstantRepair);
					development_material_list.Add(r.DevelopmentMaterial);
					modding_material_list.Add(r.ModdingMaterial);
					instant_contruction_list.Add(r.InstantConstruction);
					prev = r;
				}
			}
		}
		InstantRepairPlot = ChartArea.Plot.AddScatterLines(date_list.ToArray(), instant_repair_list.ToArray(), System.Drawing.Color.Green, label: "Instant Repair");
		DevelopmentMaterialPlot = ChartArea.Plot.AddScatterLines(date_list.ToArray(), development_material_list.ToArray(), System.Drawing.Color.Beige, label: "Development Material");
		ModdingMaterialPlot = ChartArea.Plot.AddScatterLines(date_list.ToArray(), modding_material_list.ToArray(), System.Drawing.Color.Blue, label: "Modding Material");
		InstantConstructionPlot = ChartArea.Plot.AddScatterLines(date_list.ToArray(), instant_contruction_list.ToArray(), System.Drawing.Color.Orange, label: "Instant Construction");
		ChartArea.Refresh();
	}

	private void SetYBounds()
	{
		//SetYBounds(
		//	!ResourceChart.Series.Any(s => s.Enabled) || SelectedChartType == ChartType.ExperienceDiff ? 0 : ResourceChart.Series.Where(s => s.Enabled).Select(s => s.YAxisType == AxisType.Secondary ? s.Points.Min(p => p.YValues[0] * 100) : s.Points.Min(p => p.YValues[0])).Min(),
		//	!ResourceChart.Series.Any(s => s.Enabled) ? 0 : ResourceChart.Series.Where(s => s.Enabled).Select(s => s.YAxisType == AxisType.Secondary ? s.Points.Max(p => p.YValues[0] * 100) : s.Points.Max(p => p.YValues[0])).Max()
		//);
	}

	private void AxisXIntervals(ChartSpan span)
	{
		var axis = ChartArea.Plot.XAxis;
		switch (span)
		{
			case ChartSpan.Day:
				axis.TickLabelFormat("MM/dd HH:mm", true);
				axis.ManualTickSpacing(2, ScottPlot.Ticks.DateTimeUnit.Hour);
				axis.TickLabelStyle(rotation: 90);
				break;
			case ChartSpan.Week:
			case ChartSpan.WeekFirst:
				axis.TickLabelFormat("MM/dd HH:mm", true);
				axis.ManualTickSpacing(12, ScottPlot.Ticks.DateTimeUnit.Hour);
				axis.TickLabelStyle(rotation: 90);
				break;
			case ChartSpan.Month:
			case ChartSpan.MonthFirst:
				axis.TickLabelFormat("yyyy/MM/dd", true);
				axis.ManualTickSpacing(2, ScottPlot.Ticks.DateTimeUnit.Day);
				axis.TickLabelStyle(rotation: 90);
				break;
			case ChartSpan.Season:
			case ChartSpan.SeasonFirst:
				axis.TickLabelFormat("yyyy/MM/dd", true);
				axis.ManualTickSpacing(7, ScottPlot.Ticks.DateTimeUnit.Day);
				axis.TickLabelStyle(rotation: 90);
				break;
			case ChartSpan.Year:
			case ChartSpan.YearFirst:
			case ChartSpan.All:
				axis.TickLabelFormat("yyyy/MM/dd", true);
				axis.TickLabelStyle(rotation: 90);
				axis.ManualTickSpacing(1, ScottPlot.Ticks.DateTimeUnit.Month);
				break;
		}
	}
	private void FuelShow(object sender, RoutedEventArgs e)
	{
		if (FuelPlot is not null)
		{
			FuelPlot.IsVisible = true;
			ChartArea.Refresh();
		}
	}
	private void FuelHide(object sender, RoutedEventArgs e)
	{
		if (FuelPlot is not null)
		{
			FuelPlot.IsVisible = false;
			ChartArea.Refresh();
		}
	}
	private void AmmoShow(object sender, RoutedEventArgs e)
	{
		if (AmmoPlot is not null)
		{
			AmmoPlot.IsVisible = true;
			ChartArea.Refresh();
		}
	}
	private void AmmoHide(object sender, RoutedEventArgs e)
	{
		if (AmmoPlot is not null)
		{
			AmmoPlot.IsVisible = false;
			ChartArea.Refresh();
		}
	}
	private void SteelShow(object sender, RoutedEventArgs e)
	{
		if (SteelPlot is not null)
		{
			SteelPlot.IsVisible = true;
			ChartArea.Refresh();
		}
	}
	private void SteelHide(object sender, RoutedEventArgs e)
	{
		if (SteelPlot is not null)
		{
			SteelPlot.IsVisible = false;
			ChartArea.Refresh();
		}
	}
	private void BauxShow(object sender, RoutedEventArgs e)
	{
		if (BauxPlot is not null)
		{
			BauxPlot.IsVisible = true;
			ChartArea.Refresh();
		}
	}
	private void BauxHide(object sender, RoutedEventArgs e)
	{
		if (BauxPlot is not null)
		{
			BauxPlot.IsVisible = false;
			ChartArea.Refresh();
		}
	}
	private void InstantRepairShow(object sender, RoutedEventArgs e)
	{
		if (InstantRepairPlot is not null)
		{
			InstantRepairPlot.IsVisible = true;
			ChartArea.Refresh();
		}
	}
	private void InstantRepairHide(object sender, RoutedEventArgs e)
	{
		if (InstantRepairPlot is not null)
		{
			InstantRepairPlot.IsVisible = false;
			ChartArea.Refresh();
		}
	}
	private void ChartSpan_Click(object sender, RoutedEventArgs e)
	{
		SwitchMenuStrip(ChartSpanMenu, ((MenuItem)sender).Tag);
		UpdateChart();
	}

	private void UpdateChart()
	{
		switch (SelectedChartType)
		{
			case ChartType.Resource:
				SetResourceChart();
				break;
			case ChartType.Material:
				SetMaterialChart();
				break;
		}

	}


	private void SwitchMenuStrip(MenuItem parent, object index)
	{
		int intindex = int.Parse((string)index);
		var items = parent.Items.OfType<MenuItem>();
		int c = 0;

		foreach (var item in items)
		{
			item.IsChecked = intindex == c;
			c++;
		}
		parent.Tag = intindex;
	}
	private int GetSelectedMenuStripIndex(MenuItem parent)
	{
		return parent.Tag as int? ?? -1;
	}
	private IEnumerable<ResourceRecord.ResourceElement> GetRecords()
	{
		var border = DateTime.MinValue;
		var now = DateTime.Now;

		switch (SelectedChartSpan)
		{
			case ChartSpan.Day:
				border = now.AddDays(-1);
				break;
			case ChartSpan.Week:
				border = now.AddDays(-7);
				break;
			case ChartSpan.Month:
				border = now.AddMonths(-1);
				break;
			case ChartSpan.Season:
				border = now.AddMonths(-3);
				break;
			case ChartSpan.Year:
				border = now.AddYears(-1);
				break;
			case ChartSpan.WeekFirst:
				border = now.AddDays(now.DayOfWeek == DayOfWeek.Sunday ? -6 : (1 - (int)now.DayOfWeek));
				break;
			case ChartSpan.MonthFirst:
				border = new DateTime(now.Year, now.Month, 1);
				break;
			case ChartSpan.SeasonFirst:
			{
				int m = now.Month / 3 * 3;
				if (m == 0)
					m = 12;
				border = new DateTime(now.Year - (now.Month < 3 ? 1 : 0), m, 1);
			}
			break;
			case ChartSpan.YearFirst:
				border = new DateTime(now.Year, 1, 1);
				break;
		}

		foreach (var r in RecordManager.Instance.Resource.Record)
		{
			if (r.Date >= border)
				yield return r;
		}

		var material = KCDatabase.Instance.Material;
		var admiral = KCDatabase.Instance.Admiral;
		if (material.IsAvailable && admiral.IsAvailable)
		{
			yield return new ResourceRecord.ResourceElement(
				material.Fuel, material.Ammo, material.Steel, material.Bauxite,
				material.InstantConstruction, material.InstantRepair, material.DevelopmentMaterial, material.ModdingMaterial,
				admiral.Level, admiral.Exp);
		}
	}

	private void MaterialMenu_Click(object sender, RoutedEventArgs e)
	{
		SwitchMenuStrip(ChartTypeMenu, "2");
		UpdateChart();
	}

	private void ResourceMenu_Click(object sender, RoutedEventArgs e)
	{
		SwitchMenuStrip(ChartTypeMenu, "0");
		UpdateChart();
	}

	private void InstantConstructionShow(object sender, RoutedEventArgs e)
	{
		if (InstantConstructionPlot is not null)
		{
			InstantConstructionPlot.IsVisible = true;
			ChartArea.Refresh();
		}
	}

	private void InstantConstructionHide(object sender, RoutedEventArgs e)
	{
		if (InstantConstructionPlot is not null)
		{
			InstantConstructionPlot.IsVisible = false;
			ChartArea.Refresh();
		}
	}

	private void ModdingMaterialShow(object sender, RoutedEventArgs e)
	{
		if (ModdingMaterialPlot is not null)
		{
			ModdingMaterialPlot.IsVisible = true;
			ChartArea.Refresh();
		}
	}

	private void ModdingMaterialHide(object sender, RoutedEventArgs e)
	{
		if (ModdingMaterialPlot is not null)
		{
			ModdingMaterialPlot.IsVisible = false;
			ChartArea.Refresh();
		}
	}

	private void DevelopmentMaterialShow(object sender, RoutedEventArgs e)
	{
		if (DevelopmentMaterialPlot is not null)
		{
			DevelopmentMaterialPlot.IsVisible = true;
			ChartArea.Refresh();
		}
	}

	private void DevelopmentMaterialHide(object sender, RoutedEventArgs e)
	{
		if (DevelopmentMaterialPlot is not null)
		{
			DevelopmentMaterialPlot.IsVisible = false;
			ChartArea.Refresh();
		}
	}
}
