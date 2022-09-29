using Newtonsoft.Json;
using SteadyState.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SteadyState.MainProject.WPF.Models
{
	public class Rpn : IRpn, INotifyPropertyChanged
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
		public byte StepMax { get; set; }

		[JsonProperty]
		public double StepRpn { get; set; }

		[JsonProperty]
		public sbyte Step { get; set; }

		public Rpn()
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
