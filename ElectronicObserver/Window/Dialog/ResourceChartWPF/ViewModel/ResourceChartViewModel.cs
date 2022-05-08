using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ElectronicObserver.ViewModels.Translations;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicObserver.Window.Dialog.ResourceChartWPF;
public class ResourceChartViewModel : ObservableObject
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
	public DialogResourceChartTranslationViewModel DialogResourceChart { get; }
}
