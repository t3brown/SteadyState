using Newtonsoft.Json;
using SteadyState.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SteadyState.MainProject.WPF.Models
{
	public class Shn : IShn, INotifyPropertyChanged
	{
		private int _index;
		private string _title;

		[JsonProperty]
		public Guid Id { get; set; }

		[JsonProperty]
		public int Index
		{
			get => _index;
			set
			{
				if (value == _index) return;
				_index = value;
				OnPropertyChanged();
			}
		}

		[JsonProperty]
		public string Title
		{
			get => _title;
			set
			{
				if (value == _title) return;
				_title = value;
				OnPropertyChanged();
			}
		}

		[JsonProperty]
		public double A0 { get; set; }

		[JsonProperty]
		public double A1 { get; set; }

		[JsonProperty]
		public double A2 { get; set; }

		[JsonProperty]
		public double B0 { get; set; }

		[JsonProperty]
		public double B1 { get; set; }

		[JsonProperty]
		public double B2 { get; set; }

		public Shn()
		{
			Id = Guid.NewGuid();
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
