using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ElectronicObserver.Window.Dialog.ResourceChartWPF;
public class ResourceChartViewModel : ObservableObject
{
	public bool ShowFuel { get; set; } = true;
	public bool ShowAmmo { get; set; } = true;
	public bool ShowSteel { get; set; } = true;
	public bool ShowBaux { get; set; } = true;
	public bool ShowInstantRepair { get; set; } = true;
	public bool ShowInstantConstruction { get; set; } = true;
	public bool ShowDevelopmentMaterial { get; set; } = true;
	public bool ShowModdingMaterial { get; set; } = true;
	public bool ShowExperience { get; set; } = true;
}
