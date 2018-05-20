using System;
using System.ComponentModel;

namespace CosmosQueryEditor.Settings
{
    public class GeneralSettings : IGeneralSettings
    {
        private int? _maxItemCount;
        private bool _crossPartition = true;

        public GeneralSettings(int? maxItemCount, bool crossPartition)
        {
            MaxItemCount = maxItemCount;
            CrossPartition = crossPartition;
        }

        public int? MaxItemCount
        {
            get => _maxItemCount;
            set => this.MutateVerbose(ref _maxItemCount, value, RaisePropertyChanged());
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