using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.ViewModels.Translations;
public class DialogResourceChartTranslationViewModel : TranslationBaseViewModel
{
	public string Menu_File => Properties.Window.Dialog.DialogResourceChart.Menu_File.Replace("_", "__").Replace("&", "_");
	public string Menu_File_SaveImage => Properties.Window.Dialog.DialogResourceChart.Menu_File_SaveImage.Replace("_", "__").Replace("&", "_");
	public string Menu_Graph => Properties.Window.Dialog.DialogResourceChart.Menu_Graph.Replace("_", "__").Replace("&", "_");
	public string Menu_Graph_Resource => Properties.Window.Dialog.DialogResourceChart.Menu_Graph_Resource.Replace("_", "__").Replace("&", "_");
	public string Menu_Graph_ResourceDiff => Properties.Window.Dialog.DialogResourceChart.Menu_Graph_ResourceDiff.Replace("_", "__").Replace("&", "_");
	public string Menu_Graph_Material => Properties.Window.Dialog.DialogResourceChart.Menu_Graph_Material.Replace("_", "__").Replace("&", "_");
	public string Menu_Graph_MaterialDiff => Properties.Window.Dialog.DialogResourceChart.Menu_Graph_MaterialDiff.Replace("_", "__").Replace("&", "_");
	public string Menu_Graph_Experience => Properties.Window.Dialog.DialogResourceChart.Menu_Graph_Experience.Replace("_", "__").Replace("&", "_");
	public string Menu_Graph_ExperienceDiff => Properties.Window.Dialog.DialogResourceChart.Menu_Graph_ExperienceDiff.Replace("_", "__").Replace("&", "_");
	public string Menu_Option_DivideByDay => Properties.Window.Dialog.DialogResourceChart.Menu_Option_DivideByDay.Replace("_", "__").Replace("&", "_");
	public string Menu_Option_ShowAllData => Properties.Window.Dialog.DialogResourceChart.Menu_Option_ShowAllData.Replace("_", "__").Replace("&", "_");
	public string Menu_Span => Properties.Window.Dialog.DialogResourceChart.Menu_Span.Replace("_", "__").Replace("&", "_");
	//For Translation Below
	//public string Menu_Span_WeekFirst => Properties.Window.Dialog.DialogResourceChart.Menu_Span_WeekFirst.Replace("_", "__").Replace("&", "_");
	//public string Menu_Span_MonthFirst => Properties.Window.Dialog.DialogResourceChart.Menu_Span_MonthFirst.Replace("_", "__").Replace("&", "_");
	//public string Menu_Span_SeasonFirst => Properties.Window.Dialog.DialogResourceChart.Menu_Span_SeasonFirst.Replace("_", "__").Replace("&", "_");
	//public string Menu_Span_YearFirst => Properties.Window.Dialog.DialogResourceChart.Menu_Span_WeekFirst.Replace("_", "__").Replace("&", "_");
}
