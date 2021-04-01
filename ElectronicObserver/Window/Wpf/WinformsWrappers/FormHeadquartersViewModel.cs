﻿using ElectronicObserver.Window.Wpf.WinformsHost;

namespace ElectronicObserver.Window.Wpf.WinformsWrappers
{
	public class FormHeadquartersViewModel : WinformsHostViewModel
	{
		public FormHeadquartersViewModel() : base("HQ", "FormHeadquarters")
		{
			// todo remove parameter cause it's never used
			WinformsControl = new FormHeadquarters(null!) { TopLevel = false };

			WindowsFormsHost.Child = WinformsControl;
		}
	}
}