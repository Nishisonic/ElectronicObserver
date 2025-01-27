﻿using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using ElectronicObserver.Common;
using ElectronicObserver.Data;

namespace ElectronicObserver.Window.Tools.AirControlSimulator;

public partial class AirControlSimulatorViewModel : WindowViewModelBase
{
	public AirControlSimulatorTranslationViewModel AirControlSimulator { get; }

	public bool Fleet1 { get; set; } = true;
	public bool Fleet2 { get; set; }
	public bool Fleet3 { get; set; }
	public bool Fleet4 { get; set; }
	public bool MaxAircraftLevelFleet { get; set; } = true;

	public ObservableCollection<AirBaseArea> AirBaseAreas { get; } = new();
	public AirBaseArea? AirBaseArea { get; set; }
	public bool MaxAircraftLevelAirBase { get; set; } = true;

	public bool ShipData { get; set; } = true;
	public bool EquipmentData { get; set; } = true;
	public bool LockedEquipment { get; set; } = true;
	public bool AllEquipment { get; set; }

	public bool ElectronicObserverBrowser { get; set; } = true;
	public bool SystemBrowser { get; set; }

	public bool FleetOnly { get; set; }
	public bool ShowDataGroup => !FleetOnly;

	public bool? DialogResult { get; set; }

	public AirControlSimulatorViewModel()
	{
		AirControlSimulator = Ioc.Default.GetService<AirControlSimulatorTranslationViewModel>()!;
		
		AirBaseAreas.Add(new(0, AirControlSimulator.None));

		var maps = KCDatabase.Instance.BaseAirCorps.Values
			.Select(b => b.MapAreaID)
			.Distinct()
			.OrderBy(i => i)
			.Select(i => KCDatabase.Instance.MapArea[i])
			.Where(m => m != null);

		foreach (var map in maps)
		{
			int mapAreaID = map.MapAreaID;
			string? name = map.NameEN;

			if (string.IsNullOrWhiteSpace(map.NameEN) || map.NameEN == "※")
			{
				name = Properties.Window.Dialog.DialogBaseAirCorpsSimulation.EventMap;
			}

			AirBaseAreas.Add(new(mapAreaID, name));
		}

		AirBaseArea = AirBaseAreas.MaxBy(b => b.AreaId);
	}

	[ICommand]
	private void Confirm() => DialogResult = true;

	[ICommand]
	private void Cancel() => DialogResult = false;
}
