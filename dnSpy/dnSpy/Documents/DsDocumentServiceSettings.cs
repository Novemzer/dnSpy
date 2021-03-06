﻿/*
    Copyright (C) 2014-2016 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using dnSpy.Contracts.MVVM;
using dnSpy.Contracts.Settings;

namespace dnSpy.Documents {
	interface IDsDocumentServiceSettings : INotifyPropertyChanged {
		bool UseMemoryMappedIO { get; set; }
	}

	class DsDocumentServiceSettings : ViewModelBase, IDsDocumentServiceSettings {
		protected virtual void OnModified() { }

		public bool UseMemoryMappedIO {
			get { return useMemoryMappedIO; }
			set {
				if (useMemoryMappedIO != value) {
					useMemoryMappedIO = value;
					OnPropertyChanged(nameof(UseMemoryMappedIO));
					OnModified();
				}
			}
		}
		bool useMemoryMappedIO = true;
	}

	[Export, Export(typeof(IDsDocumentServiceSettings))]
	sealed class DsDocumentServiceSettingsImpl : DsDocumentServiceSettings {
		static readonly Guid SETTINGS_GUID = new Guid("3643CE93-84D5-455A-9183-94B58BC80942");

		readonly ISettingsService settingsService;

		[ImportingConstructor]
		DsDocumentServiceSettingsImpl(ISettingsService settingsService) {
			this.settingsService = settingsService;

			this.disableSave = true;
			var sect = settingsService.GetOrCreateSection(SETTINGS_GUID);
			this.UseMemoryMappedIO = sect.Attribute<bool?>(nameof(UseMemoryMappedIO)) ?? this.UseMemoryMappedIO;
			this.disableSave = false;
		}
		readonly bool disableSave;

		protected override void OnModified() {
			if (disableSave)
				return;
			var sect = settingsService.RecreateSection(SETTINGS_GUID);
			sect.Attribute(nameof(UseMemoryMappedIO), UseMemoryMappedIO);
		}
	}
}
