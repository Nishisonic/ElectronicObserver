using System;
using ElectronicObserver.Common;
using ElectronicObserver.ViewModels.Translations;
using ElectronicObserver.Window.Dialog;
using Microsoft.Extensions.DependencyInjection;
namespace ElectronicObserver.Window.Dialog.ResourceChartWPF;
public class ResourceChartViewModel : WindowViewModelBase
{
	public ResourceChartViewModel()
	{
		DialogResourceChart = App.Current.Services.GetService<DialogResourceChartTranslationViewModel>()!;
	}

	public bool ShowFuel { get; set; } = true;
	public bool ShowAmmo { get; set; } = true;
	public bool ShowSteel { get; set; } = true;
	public bool ShowBaux { get; set; } = true;
	public bool ShowInstantRepair { get; set; } = true;
	public bool ShowInstantConstruction { get; set; } = true;
	public bool ShowDevelopmentMaterial { get; set; } = true;
	public bool ShowModdingMaterial { get; set; } = true;
	public bool ShowExperience { get; set; } = true;
	public DateTime DateBegin { get; set; }
	public DateTime DateEnd { get; set; }
	public DateTime MinDate { get; set; }
	public DateTime MaxDate { get; set; }
	public string Today => $"Today : {DateTime.Now:yyyy/MM/dd}";
	public DialogResourceChartTranslationViewModel DialogResourceChart { get; }
}
