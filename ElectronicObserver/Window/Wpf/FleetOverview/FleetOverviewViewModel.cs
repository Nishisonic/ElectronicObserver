﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using ElectronicObserver.Data;
using ElectronicObserver.Observer;
using ElectronicObserver.Resource;
using ElectronicObserver.Utility.Data;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.ViewModels;
using ElectronicObserver.ViewModels.Translations;
using ElectronicObserver.Window.Wpf.Fleet;
using ElectronicObserverTypes;

namespace ElectronicObserver.Window.Wpf.FleetOverview;

public class FleetOverviewItemViewModel : ObservableObject
{
	public string? Text { get; set; }
	public string? ToolTip { get; set; }
	public ImageSource? Icon { get; set; }
	public bool Visible { get; set; }
	public DateTime? Tag { get; set; }

	public Visibility Visibility => Visible.ToVisibility();
}

public class FleetOverviewViewModel : AnchorableViewModel
{
	public FormFleetOverviewTranslationViewModel FormFleetOverview { get; }

	public List<FleetViewModel> Fleets { get; }
	public FleetOverviewItemViewModel AnchorageRepairingTimer { get; }
	public FleetOverviewItemViewModel CombinedTag { get; }

	public FleetOverviewViewModel(List<FleetViewModel> fleets) : base("Fleets", "Fleets",
		ImageSourceIcons.GetIcon(IconContent.FormFleet))
	{
		FormFleetOverview = Ioc.Default.GetService<FormFleetOverviewTranslationViewModel>()!;

		Title = FormFleetOverview.Title;
		FormFleetOverview.PropertyChanged += (_, _) => Title = FormFleetOverview.Title;

		Fleets = fleets;

		//api register
		APIObserver o = APIObserver.Instance;

		o.ApiReqNyukyo_Start.RequestReceived += Updated;
		o.ApiReqNyukyo_SpeedChange.RequestReceived += Updated;
		o.ApiReqHensei_Change.RequestReceived += Updated;
		o.ApiReqKousyou_DestroyShip.RequestReceived += Updated;
		o.ApiReqMember_UpdateDeckName.RequestReceived += Updated;
		o.ApiReqMap_Start.RequestReceived += Updated;
		o.ApiReqHensei_Combined.RequestReceived += Updated;
		o.ApiReqKaisou_OpenExSlot.RequestReceived += Updated;

		o.ApiPort_Port.ResponseReceived += Updated;
		o.ApiGetMember_Ship2.ResponseReceived += Updated;
		o.ApiGetMember_NDock.ResponseReceived += Updated;
		o.ApiReqKousyou_GetShip.ResponseReceived += Updated;
		o.ApiReqHokyu_Charge.ResponseReceived += Updated;
		o.ApiReqKousyou_DestroyShip.ResponseReceived += Updated;
		o.ApiGetMember_Ship3.ResponseReceived += Updated;
		o.ApiReqKaisou_PowerUp.ResponseReceived += Updated; //requestのほうは面倒なのでこちらでまとめてやる
		o.ApiGetMember_Deck.ResponseReceived += Updated;
		o.ApiReqMap_Start.ResponseReceived += Updated;
		o.ApiReqMap_Next.ResponseReceived += Updated;
		o.ApiGetMember_ShipDeck.ResponseReceived += Updated;
		o.ApiReqHensei_PresetSelect.ResponseReceived += Updated;
		o.ApiReqKaisou_SlotExchangeIndex.ResponseReceived += Updated;
		o.ApiGetMember_RequireInfo.ResponseReceived += Updated;
		o.ApiReqKaisou_SlotDeprive.ResponseReceived += Updated;
		o.ApiReqKaisou_Marriage.ResponseReceived += Updated;
		o.ApiReqMap_AnchorageRepair.ResponseReceived += Updated;


		AnchorageRepairingTimer = new()
		{
			// Anchor = AnchorStyles.Left,
			// Padding = new Padding(0, 1, 0, 1),
			// Margin = new Padding(2, 1, 2, 1),
			// ImageList = ResourceManager.Instance.Icons,
			// ImageIndex = (int)ResourceManager.IconContent.FleetAnchorageRepairing,
			Text = "-",
			Icon = ImageSourceIcons.GetIcon(IconContent.FleetAnchorageRepairing),
			// AutoSize = true
			Visible = false
		};

		// TableFleet.Controls.Add(AnchorageRepairingTimer, 1, 4);

		CombinedTag = new()
		{
			// Anchor = AnchorStyles.Left,
			// Padding = new Padding(0, 1, 0, 1),
			// Margin = new Padding(2, 1, 2, 1),
			// ImageList = ResourceManager.Instance.Icons,
			// ImageIndex = (int)ResourceManager.IconContent.FleetCombined,
			Text = "-",
			Icon = ImageSourceIcons.GetIcon(IconContent.FleetCombined),

			// AutoSize = true,
			Visible = false
		};

		// TableFleet.Controls.Add(CombinedTag, 1, 5);

		ConfigurationChanged();

		Utility.SystemEvents.UpdateTimerTick += UpdateTimerTick;
		Utility.Configuration.Instance.ConfigurationChanged += ConfigurationChanged;
	}

	void ConfigurationChanged()
	{
		// TableFleet.SuspendLayout();

		// AutoScroll = Utility.Configuration.Config.FormFleet.IsScrollable;

		/*
		foreach (var c in ControlFleet)
			c.ConfigurationChanged(this);
		*/

		// CombinedTag.Font = Font;
		// AnchorageRepairingTimer.Font = Font;
		AnchorageRepairingTimer.Visible = Utility.Configuration.Config.FormFleet.ShowAnchorageRepairingTimer;

		// LayoutSubInformation();

		// ControlHelper.SetTableRowStyles(TableFleet, ControlHelper.GetDefaultRowStyle());

		// TableFleet.ResumeLayout();
	}


	private void Updated(string apiname, dynamic data)
	{

		// TableFleet.SuspendLayout();

		// TableFleet.RowCount = KCDatabase.Instance.Fleet.Fleets.Values.Count(f => f.IsAvailable);
		// for (int i = 0; i < ControlFleet.Count; i++)
		// {
		// 	ControlFleet[i].Update();
		// }

		if (KCDatabase.Instance.Fleet.CombinedFlag > 0)
		{
			CombinedTag.Text = Constants.GetCombinedFleet(KCDatabase.Instance.Fleet.CombinedFlag);

			var fleet1 = KCDatabase.Instance.Fleet[1];
			var fleet2 = KCDatabase.Instance.Fleet[2];

			int tp = Calculator.GetTPDamage(fleet1) + Calculator.GetTPDamage(fleet2);

			var members = fleet1.MembersWithoutEscaped.Concat(fleet2.MembersWithoutEscaped).Where(s => s != null);

			// 各艦ごとの ドラム缶 or 大発系 を搭載している個数
			var transport = members.Select(s => s.AllSlotInstanceMaster.Count(eq => eq?.CategoryType == EquipmentTypes.TransportContainer));
			var landing = members.Select(s => s.AllSlotInstanceMaster.Count(eq => eq?.CategoryType == EquipmentTypes.LandingCraft || eq?.CategoryType == EquipmentTypes.SpecialAmphibiousTank));
			var radar = members.Select(s => s.AllSlotInstanceMaster.Count(eq => eq?.IsSurfaceRadar == true));

			CombinedTag.ToolTip = string.Format(FormFleetOverview.CombinedFleetToolTip,
				transport.Sum(),
				landing.Sum(),
				tp,
				(int)Math.Floor(tp * 0.7),
				Calculator.GetAirSuperiority(fleet1) + Calculator.GetAirSuperiority(fleet2),
				Math.Floor(fleet1.GetSearchingAbility() * 100) / 100 + Math.Floor(fleet2.GetSearchingAbility() * 100) / 100,
				Math.Floor(Calculator.GetSearchingAbility_New33(fleet1, 1) * 100) / 100 + Math.Floor(Calculator.GetSearchingAbility_New33(fleet2, 1) * 100) / 100,
				Math.Floor(Calculator.GetSearchingAbility_New33(fleet1, 2) * 100) / 100 + Math.Floor(Calculator.GetSearchingAbility_New33(fleet2, 2) * 100) / 100,
				Math.Floor(Calculator.GetSearchingAbility_New33(fleet1, 3) * 100) / 100 + Math.Floor(Calculator.GetSearchingAbility_New33(fleet2, 3) * 100) / 100,
				Math.Floor(Calculator.GetSearchingAbility_New33(fleet1, 4) * 100) / 100 + Math.Floor(Calculator.GetSearchingAbility_New33(fleet2, 4) * 100) / 100,
				radar.Sum(),
				radar.Count(i => i > 0),
				transport.Count(i => i> 0),
				landing.Count(i => i > 0)

			);


			CombinedTag.Visible = true;
		}
		else
		{
			CombinedTag.Visible = false;
		}

		if (KCDatabase.Instance.Fleet.AnchorageRepairingTimer > DateTime.MinValue)
		{
			AnchorageRepairingTimer.Text = DateTimeHelper.ToTimeElapsedString(KCDatabase.Instance.Fleet.AnchorageRepairingTimer);
			AnchorageRepairingTimer.Tag = KCDatabase.Instance.Fleet.AnchorageRepairingTimer;
			AnchorageRepairingTimer.ToolTip =
				FormFleetOverview.AnchorageRepairToolTip +
				DateTimeHelper.TimeToCSVString(KCDatabase.Instance.Fleet.AnchorageRepairingTimer) +
				$"\r\n{FormFleetOverview.Recovery}: " +
				DateTimeHelper.TimeToCSVString(KCDatabase.Instance.Fleet.AnchorageRepairingTimer.AddMinutes(20));
		}

	}


	void UpdateTimerTick()
	{
		// for (int i = 0; i < ControlFleet.Count; i++)
		// {
		// 	ControlFleet[i].Refresh();
		// }

		if (AnchorageRepairingTimer.Visible && AnchorageRepairingTimer.Tag != null)
			AnchorageRepairingTimer.Text = DateTimeHelper.ToTimeElapsedString((DateTime)AnchorageRepairingTimer.Tag);
	}
}
