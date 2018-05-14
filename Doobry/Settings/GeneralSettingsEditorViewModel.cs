using System;
using System.ComponentModel;

namespace Doobry.Settings
{
    public class GeneralSettingsEditorViewModel : INotifyPropertyChanged
    {
        private int? _maxItemCount;
        private bool _crossPartition = true;

        public int? MaxItemCount
        {
            get { return _maxItemCount; }
            set { this.MutateVerbose(ref _maxItemCount, value, RaisePropertyChanged()); }
        }

        public bool CrossPartition
        {
            get => _crossPartition;
            set => this.MutateVerbose(ref _crossPartition, value, RaisePropertyChanged());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
