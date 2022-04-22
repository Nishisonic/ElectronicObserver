using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ElectronicObserver.Data;
using ElectronicObserver.Resource.Record;
using Microsoft.Win32;
using ScottPlot;
using ScottPlot.Plottable;
using Translation = ElectronicObserver.Properties.Window.Dialog.DialogResourceChart;

namespace ElectronicObserver.Window.Dialog.ResourceChartWPF;

/// <summary>
/// Interaction logic for ResourceChartWPF.xaml
/// </summary>
public partial class ResourceChartWPF
{
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

	private enum ChartType
	{
		Resource,
		ResourceDiff,
		Material,
		MaterialDiff,
		Experience,
		ExperienceDiff,
	}

	private ScatterPlot? FuelPlot;
	private ScatterPlot? AmmoPlot;
	private ScatterPlot? SteelPlot;
	private ScatterPlot? BauxPlot;
	private SignalPlotXY? FuelSignalPlot;
	private SignalPlotXY? AmmoSignalPlot;
	private SignalPlotXY? SteelSignalPlot;
	private SignalPlotXY? BauxSignalPlot;
	private ScatterPlot? InstantRepairPlot;
	private ScatterPlot? InstantConstructionPlot;
	private ScatterPlot? ModdingMaterialPlot;
	private ScatterPlot? DevelopmentMaterialPlot;
	private SignalPlotXY? InstantRepairSignalPlot;
	private SignalPlotXY? InstantConstructionSignalPlot;
	private SignalPlotXY? ModdingMaterialSignalPlot;
	private SignalPlotXY? DevelopmentMaterialSignalPlot;
	private ToolTip? toolTip;
	private ScatterPlot? ExperiencePlot;
	private SignalPlotXY ExperienceSignalPlot;
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


		toolTip = new ToolTip
		{
			Content = "ToolTip"
		};
		ChartArea.ToolTip = toolTip;
		SwitchMenuStrip(ChartSpanMenu, "5");
		SwitchMenuStrip(ChartTypeMenu, "0");
		ChartArea.Plot.Style(ScottPlot.Style.Black);
		ChartArea.Configuration.Zoom = false;
		ChartArea.Configuration.Pan = false;
		ChartArea.RightClicked -= ChartArea.DefaultRightClickEvent;
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
		(double mouseCoordX, double mouseCoordY) = ChartArea.GetMouseCoordinates();
		double xyRatio = ChartArea.Plot.XAxis.Dims.PxPerUnit / ChartArea.Plot.YAxis.Dims.PxPerUnit;
		if (SelectedChartType == ChartType.Resource)
		{
			(double fuelpointX, double fuelpointY, int fuelpointIndex) = FuelPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
			(double ammopointX, double ammopointY, int ammopointIndex) = AmmoPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
			(double steelpointX, double steelpointY, int steelpointIndex) = SteelPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
			(double bauxpointX, double bauxpointY, int bauxpointIndex) = BauxPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
			(double instantrepairpointX, double instantrepairpointY, int instantrepairpointIndex) = InstantRepairPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
			DateTime date = DateTime.FromOADate(fuelpointX);
			toolTip.Content = string.Format("{0}\nFuel: {1}\nAmmo:{2}\nSteel: {3}\nBaux: {4}\nInstant Repair: {5}", date, fuelpointY, ammopointY, steelpointY, bauxpointY, instantrepairpointY);
		}
		else if(SelectedChartType == ChartType.ResourceDiff)
		{
			(double fuelpointX, double fuelpointY, int fuelpointIndex) = FuelSignalPlot.GetPointNearestX(mouseCoordX);
			(double ammopointX, double ammopointY, int ammopointIndex) = AmmoSignalPlot.GetPointNearestX(mouseCoordX);
			(double steelpointX, double steelpointY, int steelpointIndex) = SteelSignalPlot.GetPointNearestX(mouseCoordX);
			(double bauxpointX, double bauxpointY, int bauxpointIndex) = BauxSignalPlot.GetPointNearestX(mouseCoordX);
			(double instantrepairpointX, double instantrepairpointY, int instantrepairpointIndex) = InstantRepairSignalPlot.GetPointNearestX(mouseCoordX);
			DateTime date = DateTime.FromOADate(fuelpointX);
			if (Menu_Option_DivideByDay.IsChecked)
			{
				toolTip.Content = string.Format("{0}\nFuel: {1:+0;-0;±0} /day \nAmmo:{2:+0;-0;±0} /day \nSteel: {3:+0;-0;±0}/day \nBaux: {4:+0;-0;±0} /day\nInstant Repair: {5:+0;-0;±0} /day", date, fuelpointY, ammopointY, steelpointY, bauxpointY, instantrepairpointY);
			}
			else
			{
				toolTip.Content = string.Format("{0}\nFuel: {1}\nAmmo:{2}\nSteel: {3}\nBaux: {4}\nInstant Repair: {5}", date, fuelpointY, ammopointY, steelpointY, bauxpointY, instantrepairpointY);
			}
		}
		else if (SelectedChartType == ChartType.Material)
		{
			(double instantconstructionpointX, double instantconstructionpointY, int instantconstructionpointpointIndex) = InstantConstructionPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
			(double moddingmaterialpointX, double moddingmaterialpointY, int moddingmaterialpointIndex) = ModdingMaterialPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
			(double developmentmaterialpointX, double developmentmaterialpointY, int developmentmaterialpointIndex) = DevelopmentMaterialPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
			(double instantrepairpointX, double instantrepairpointY, int instantrepairpointIndex) = InstantRepairPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
			DateTime date = DateTime.FromOADate(instantconstructionpointX);
			toolTip.Content = string.Format("{0}\nDevelopment Material: {1}\nModding Material:{2}\nInstant Construction: {3}\nInstant Repair: {4}", date, developmentmaterialpointY, moddingmaterialpointY, instantconstructionpointY, instantrepairpointY);
		}
		else if (SelectedChartType == ChartType.MaterialDiff)
		{
			(double instantconstructionpointX, double instantconstructionpointY, int instantconstructionpointpointIndex) = InstantConstructionSignalPlot.GetPointNearestX(mouseCoordX);
			(double moddingmaterialpointX, double moddingmaterialpointY, int moddingmaterialpointIndex) = ModdingMaterialSignalPlot.GetPointNearestX(mouseCoordX);
			(double developmentmaterialpointX, double developmentmaterialpointY, int developmentmaterialpointIndex) = DevelopmentMaterialSignalPlot.GetPointNearestX(mouseCoordX);
			(double instantrepairpointX, double instantrepairpointY, int instantrepairpointIndex) = InstantRepairSignalPlot.GetPointNearestX(mouseCoordX);
			DateTime date = DateTime.FromOADate(instantconstructionpointX);
			if (Menu_Option_DivideByDay.IsChecked)
			{
				toolTip.Content = string.Format("{0}\nDevelopment Material: {1:+0;-0;±0} /day\nModding Material:{2:+0;-0;±0} /day\nInstant Construction: {3:+0;-0;±0} /day\nInstant Repair: {4:+0;-0;±0} /day", date, developmentmaterialpointY, moddingmaterialpointY, instantconstructionpointY, instantrepairpointY);
			}
			else
			{
				toolTip.Content = string.Format("{0}\nDevelopment Material: {1}\nModding Material:{2}\nInstant Construction: {3}\nInstant Repair: {4}", date, developmentmaterialpointY, moddingmaterialpointY, instantconstructionpointY, instantrepairpointY);
			}
		}else if (SelectedChartType == ChartType.Experience)
		{
			(double experiencepointX, double experiencepointY, int experiencepointIndex) = ExperiencePlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
			DateTime date = DateTime.FromOADate(experiencepointX);
			toolTip.Content = $"{date}\nHQ Experience: {experiencepointY}";
		}
		else if (SelectedChartType == ChartType.ExperienceDiff)
		{
			(double experiencepointX, double experiencepointY, int experiencepointIndex) = ExperienceSignalPlot.GetPointNearestX(mouseCoordX);
			DateTime date = DateTime.FromOADate(experiencepointX);
			if (Menu_Option_DivideByDay.IsChecked)
			{
				toolTip.Content = string.Format("{0}\nHQ Experience: {1:+0;-0;±0}", date, experiencepointY);
			}
			else
			{
				toolTip.Content = string.Format("{0}\nHQ Experience: {1:+0;-0;±0} /day", date, experiencepointY);
			}
		}
		else
		{
			toolTip = null;
		}
		// update the GUI to describe the highlighted point
		double mouseX = e.GetPosition(this).X;
		double mouseY = e.GetPosition(this).Y;

		toolTip.HorizontalOffset = mouseX;
		toolTip.VerticalOffset = mouseY - 20;
	}

	private void SetResourceChart()
	{
		ChartArea.Plot.Clear();

		ChartArea.Plot.XAxis.Label("Date");
		ChartArea.Plot.YAxis.Label("Resource");
		ChartArea.Plot.XAxis.DateTimeFormat(true);
		AxisXIntervals(SelectedChartSpan);
		FuelCheck.IsChecked = true;
		AmmoCheck.IsChecked = true;
		BauxCheck.IsChecked = true;
		ResourcesPanel.Visibility = Visibility.Visible;
		MaterialPanel.Visibility = Visibility.Collapsed;
		ExperiencePanel.Visibility = Visibility.Collapsed;
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
		ChartArea.Refresh();
	}
	private void SetResourceDiffChart()
	{
		ChartArea.Plot.Clear();

		ChartArea.Plot.XAxis.Label("Date");
		ChartArea.Plot.YAxis.Label("Resource");
		ChartArea.Plot.XAxis.DateTimeFormat(true);
		AxisXIntervals(SelectedChartSpan);
		FuelCheck.IsChecked = true;
		AmmoCheck.IsChecked = true;
		BauxCheck.IsChecked = true;
		ResourcesPanel.Visibility = Visibility.Visible;
		MaterialPanel.Visibility = Visibility.Collapsed;
		ExperiencePanel.Visibility = Visibility.Collapsed;
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

					double[] ys = new double[] {
						r.Fuel - prev.Fuel,
						r.Ammo - prev.Ammo,
						r.Steel - prev.Steel,
						r.Bauxite - prev.Bauxite,
						r.InstantRepair - prev.InstantRepair };
					if (Menu_Option_DivideByDay.IsChecked)
					{
						for (int i = 0; i < 4; i++)
							ys[i] /= Math.Max((r.Date - prev.Date).TotalDays, 1.0 / 1440.0);
					}

					date_list.Add(r.Date.ToOADate());

					fuel_list.Add(ys[0]);
					ammo_list.Add(ys[1]);
					steel_list.Add(ys[2]);
					baux_list.Add(ys[3]);
					instant_repair_list.Add(ys[4]);

					prev = r;
				}
			}
		}
		InstantRepairSignalPlot = ChartArea.Plot.AddSignalXY(date_list.ToArray(), instant_repair_list.ToArray());
		InstantRepairSignalPlot.StepDisplay = true;
		InstantRepairSignalPlot.FillAboveAndBelow(System.Drawing.Color.Green, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, System.Drawing.Color.Green, 1);
		InstantRepairSignalPlot.Label = "Instant Repair";
		InstantRepairSignalPlot.MarkerSize = 0;
		InstantRepairSignalPlot.YAxisIndex = 1;

		FuelSignalPlot = ChartArea.Plot.AddSignalXY(date_list.ToArray(), fuel_list.ToArray());
		FuelSignalPlot.StepDisplay = true;
		FuelSignalPlot.FillAboveAndBelow(System.Drawing.Color.LimeGreen, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, System.Drawing.Color.LimeGreen, 1);
		FuelSignalPlot.Label = "Fuel";
		FuelSignalPlot.MarkerSize = 0;

		AmmoSignalPlot = ChartArea.Plot.AddSignalXY(date_list.ToArray(), ammo_list.ToArray());
		AmmoSignalPlot.StepDisplay = true;
		AmmoSignalPlot.FillAboveAndBelow(System.Drawing.Color.Brown, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, System.Drawing.Color.Brown, 1);
		AmmoSignalPlot.Label = "Ammo";
		AmmoSignalPlot.MarkerSize = 0;

		SteelSignalPlot = ChartArea.Plot.AddSignalXY(date_list.ToArray(), steel_list.ToArray());
		SteelSignalPlot.StepDisplay = true;
		SteelSignalPlot.FillAboveAndBelow(System.Drawing.Color.AliceBlue, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, System.Drawing.Color.AliceBlue, 1);
		SteelSignalPlot.Label = "Steel";
		SteelSignalPlot.MarkerSize = 0;

		BauxSignalPlot = ChartArea.Plot.AddSignalXY(date_list.ToArray(), baux_list.ToArray());
		BauxSignalPlot.StepDisplay = true;
		BauxSignalPlot.FillAboveAndBelow(System.Drawing.Color.Brown, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, System.Drawing.Color.Brown, 1);
		BauxSignalPlot.Label = "Bauxite";
		BauxSignalPlot.MarkerSize = 0;

		ChartArea.Refresh();
	}
	private void SetMaterialDiffChart()
	{
		ChartArea.Plot.Clear();
		ResourcesPanel.Visibility = Visibility.Collapsed;
		MaterialPanel.Visibility = Visibility.Visible;
		ExperiencePanel.Visibility = Visibility.Collapsed;
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
					double[] ys = new double[] {
						r.InstantConstruction - prev.InstantConstruction ,
						r.InstantRepair - prev.InstantRepair,
						r.DevelopmentMaterial - prev.DevelopmentMaterial ,
						r.ModdingMaterial - prev.ModdingMaterial };

					if (Menu_Option_DivideByDay.IsChecked)
					{
						for (int i = 0; i < 4; i++)
							ys[i] /= Math.Max((r.Date - prev.Date).TotalDays, 1.0 / 1440.0);
					}
					date_list.Add(r.Date.ToOADate());
					instant_contruction_list.Add(ys[0]);
					instant_repair_list.Add(ys[1]);
					development_material_list.Add(ys[2]);
					modding_material_list.Add(ys[3]);

					prev = r;
				}
			}
		}
		InstantRepairSignalPlot = ChartArea.Plot.AddSignalXY(date_list.ToArray(), instant_repair_list.ToArray());
		InstantRepairSignalPlot.StepDisplay = true;
		InstantRepairSignalPlot.FillAboveAndBelow(System.Drawing.Color.Green, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, System.Drawing.Color.Green, 1);
		InstantRepairSignalPlot.Label = "Instant Repair";
		InstantRepairSignalPlot.MarkerSize = 0;

		ModdingMaterialSignalPlot = ChartArea.Plot.AddSignalXY(date_list.ToArray(), modding_material_list.ToArray());
		ModdingMaterialSignalPlot.StepDisplay = true;
		ModdingMaterialSignalPlot.Label = "Modding Material";
		ModdingMaterialSignalPlot.FillAboveAndBelow(System.Drawing.Color.Gray, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, System.Drawing.Color.Gray, 1);
		ModdingMaterialSignalPlot.MarkerSize = 0;

		DevelopmentMaterialSignalPlot = ChartArea.Plot.AddSignalXY(date_list.ToArray(), development_material_list.ToArray());
		DevelopmentMaterialSignalPlot.StepDisplay = true;
		DevelopmentMaterialSignalPlot.Label = "Development Material";
		DevelopmentMaterialSignalPlot.FillAboveAndBelow(System.Drawing.Color.SlateBlue, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, System.Drawing.Color.SlateBlue, 1);
		DevelopmentMaterialSignalPlot.MarkerSize = 0;

		InstantConstructionSignalPlot = ChartArea.Plot.AddSignalXY(date_list.ToArray(), instant_contruction_list.ToArray());
		InstantConstructionSignalPlot.StepDisplay = true;
		InstantConstructionSignalPlot.Label = "Instant Construction";
		InstantConstructionSignalPlot.FillAboveAndBelow(System.Drawing.Color.Orange, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, System.Drawing.Color.Orange, 1);
		InstantConstructionSignalPlot.MarkerSize = 0;

		ChartArea.Refresh();
	}
	private bool ShouldSkipRecord(TimeSpan span)
	{
		if (Menu_Option_ShowAllData.IsChecked)
			return false;

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
		ExperiencePanel.Visibility = Visibility.Collapsed;
		ChartArea.Plot.YAxis2.IsVisible = false;
		ChartArea.Plot.XAxis.Label("Date");
		ChartArea.Plot.YAxis.Label("Material");
		ChartArea.Plot.XAxis.DateTimeFormat(true);
		AxisXIntervals(SelectedChartSpan);
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

	private void SetExperienceChart()
	{
		ChartArea.Plot.Clear();
		ResourcesPanel.Visibility = Visibility.Collapsed;
		MaterialPanel.Visibility = Visibility.Collapsed;
		ExperiencePanel.Visibility = Visibility.Visible;
		AxisXIntervals(SelectedChartSpan);
		ChartArea.Plot.YAxis2.IsVisible = false;
		ChartArea.Plot.XAxis.Label("Date");
		ChartArea.Plot.YAxis.Label("Experience");
		ChartArea.Plot.XAxis.DateTimeFormat(true);
		List<double>? experience_list = Array.Empty<double>().ToList();

		List<double>? date_list = Array.Empty<double>().ToList();

		{
			var record = GetRecords();

			if (record.Any())
			{
				var prev = record.First();
				foreach (var r in record)
				{
					if (ShouldSkipRecord(r.Date - prev.Date))
						continue;

					experience_list.Add(r.HQExp);
					date_list.Add(r.Date.ToOADate());
					prev = r;
				}
			}
		}
		ExperiencePlot = ChartArea.Plot.AddScatterLines(date_list.ToArray(), experience_list.ToArray(), System.Drawing.Color.Orange, label: "HQ Experience");
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

	#region Chart Toggles

	private void FuelShow(object sender, RoutedEventArgs e)
	{
		if (FuelPlot is not null)
		{
			FuelPlot.IsVisible = true;
			ChartArea.Refresh();
		}
		if (FuelSignalPlot is not null)
		{
			FuelSignalPlot.IsVisible = true;
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
		if (FuelSignalPlot is not null)
		{
			FuelSignalPlot.IsVisible = false;
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
		if (AmmoSignalPlot is not null)
		{
			AmmoSignalPlot.IsVisible = true;
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
		if (AmmoSignalPlot is not null)
		{
			AmmoSignalPlot.IsVisible = false;
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
		if (SteelSignalPlot is not null)
		{
			SteelSignalPlot.IsVisible = true;
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
		if (SteelSignalPlot is not null)
		{
			SteelSignalPlot.IsVisible = false;
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
		if (BauxSignalPlot is not null)
		{
			BauxSignalPlot.IsVisible = true;
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
		if (BauxSignalPlot is not null)
		{
			BauxSignalPlot.IsVisible = false;
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
		if (InstantRepairSignalPlot is not null)
		{
			InstantRepairSignalPlot.IsVisible = true;
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
		if (InstantRepairSignalPlot is not null)
		{
			InstantRepairSignalPlot.IsVisible = false;
			ChartArea.Refresh();
		}
	}

	private void InstantConstructionShow(object sender, RoutedEventArgs e)
	{
		if (InstantConstructionPlot is not null)
		{
			InstantConstructionPlot.IsVisible = true;
			ChartArea.Refresh();
		}
		if (InstantConstructionSignalPlot is not null)
		{
			InstantConstructionSignalPlot.IsVisible = true;
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
		if (InstantConstructionSignalPlot is not null)
		{
			InstantConstructionSignalPlot.IsVisible = false;
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
		if (ModdingMaterialSignalPlot is not null)
		{
			ModdingMaterialSignalPlot.IsVisible = true;
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
		if (ModdingMaterialSignalPlot is not null)
		{
			ModdingMaterialSignalPlot.IsVisible = false;
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
		if (DevelopmentMaterialSignalPlot is not null)
		{
			DevelopmentMaterialSignalPlot.IsVisible = true;
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
		if (DevelopmentMaterialSignalPlot is not null)
		{
			DevelopmentMaterialSignalPlot.IsVisible = false;
			ChartArea.Refresh();
		}
	}

	private void ExperienceShow(object sender, RoutedEventArgs e)
	{
		if (ExperiencePlot is not null)
		{
			ExperiencePlot.IsVisible = true;
			ChartArea.Refresh();
		}
		if (ExperienceSignalPlot is not null)
		{
			ExperienceSignalPlot.IsVisible = true;
			ChartArea.Refresh();
		}
	}

	private void ExperienceHide(object sender, RoutedEventArgs e)
	{
		if (ExperiencePlot is not null)
		{
			ExperiencePlot.IsVisible = false;
			ChartArea.Refresh();
		}
		if (ExperienceSignalPlot is not null)
		{
			ExperienceSignalPlot.IsVisible = false;
			ChartArea.Refresh();
		}
	}

	#endregion Chart Toggles

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
			case ChartType.ResourceDiff:
				SetResourceDiffChart();
				break;
			case ChartType.Material:
				SetMaterialChart();
				break;
			case ChartType.MaterialDiff:
				SetMaterialDiffChart();
				break;
			case ChartType.Experience:
				SetExperienceChart();
				break;
			case ChartType.ExperienceDiff:
				SetExperienceDiffChart();
				break;
		}
	}

	private void SetExperienceDiffChart()
	{
		ChartArea.Plot.Clear();
		ResourcesPanel.Visibility = Visibility.Collapsed;
		MaterialPanel.Visibility = Visibility.Collapsed;
		ExperiencePanel.Visibility = Visibility.Visible;
		AxisXIntervals(SelectedChartSpan);
		ChartArea.Plot.YAxis2.IsVisible = false;
		ChartArea.Plot.XAxis.Label("Date");
		ChartArea.Plot.YAxis.Label("Experience");
		ChartArea.Plot.XAxis.DateTimeFormat(true);
		List<double>? experience_list = Array.Empty<double>().ToList();

		List<double>? date_list = Array.Empty<double>().ToList();

		{
			var record = GetRecords();

			if (record.Any())
			{
				var prev = record.First();
				foreach (var r in record)
				{
					if (ShouldSkipRecord(r.Date - prev.Date))
						continue;
					double ys = r.HQExp - prev.HQExp;
					if (Menu_Option_DivideByDay.IsChecked)
						ys /= Math.Max((r.Date - prev.Date).TotalDays, 1.0 / 1440.0);

					experience_list.Add(ys);
					date_list.Add(r.Date.ToOADate());
					prev = r;
				}
			}
		}
		ExperienceSignalPlot = ChartArea.Plot.AddSignalXY(date_list.ToArray(), experience_list.ToArray());
		ExperienceSignalPlot.StepDisplay = true;
		ExperienceSignalPlot.FillAboveAndBelow(System.Drawing.Color.Gold, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, System.Drawing.Color.Gold, 1);
		ExperienceSignalPlot.Label = "HQ Experience";
		ExperienceSignalPlot.MarkerSize = 0;
		ChartArea.Refresh();
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

	private void MaterialDiffMenu_Click(object sender, RoutedEventArgs e)
	{
		SwitchMenuStrip(ChartTypeMenu, "3");
		UpdateChart();
	}

	private void ExperienceMenu_Click(object sender, RoutedEventArgs e)
	{
		SwitchMenuStrip(ChartTypeMenu, "4");
		UpdateChart();
	}

	private void ExperienceDiffMenu_Click(object sender, RoutedEventArgs e)
	{
		SwitchMenuStrip(ChartTypeMenu, "5");
		UpdateChart();
	}

	private void ResourceDiffMenu_Click(object sender, RoutedEventArgs e)
	{
		SwitchMenuStrip(ChartTypeMenu, "1");
		UpdateChart();
	}

	private void Menu_Option_ShowAllData_Click(object sender, RoutedEventArgs e)
	{
		UpdateChart();
	}

	private void Menu_Option_DivideByDay_Click(object sender, RoutedEventArgs e)
	{
		UpdateChart();
	}

	private void FileSaveImage_Click(object sender, RoutedEventArgs e)
	{
		var sfd = new SaveFileDialog
		{
			FileName = "ResourceChart.png",
			Filter = "PNG Files (*.png)|*.png;*.png" +
			 "|JPG Files (*.jpg, *.jpeg)|*.jpg;*.jpeg" +
			 "|BMP Files (*.bmp)|*.bmp;*.bmp" +
			 "|All files (*.*)|*.*"
		};

		if (sfd.ShowDialog() is true)
			ChartArea.Plot.SaveFig(sfd.FileName);
	}
}
