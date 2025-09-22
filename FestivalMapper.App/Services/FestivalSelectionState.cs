using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalMapper.App.Services
{
    public sealed class FestivalSelectionState : INotifyPropertyChanged, IDisposable
    {
        public Guid? _selectedFestivalId;
        public Guid? SelectedFestivalId
        {
            get => _selectedFestivalId;
            private set
            {
                if (_selectedFestivalId == value) return;

                _selectedFestivalId = value;
                OnPropertyChanged(nameof(SelectedFestivalId));
                OnPropertyChanged(nameof(HasSelection));
            }
        }

        public bool HasSelection => SelectedFestivalId.HasValue;

        public void Select(Guid festivalId) => SelectedFestivalId = festivalId;
        public void Clear() => SelectedFestivalId = null;

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Dispose()
        {
            // nothing yet
        }
    }
}
