using System.ComponentModel;

namespace CosmosQueryEditor.Settings
{
    public interface IGeneralSettings : INotifyPropertyChanged
    {
        int? MaxItemCount { get; set; }
        bool CrossPartition { get; set; }
    }
}