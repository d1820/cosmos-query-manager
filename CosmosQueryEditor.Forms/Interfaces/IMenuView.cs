using System;
using CosmosQueryEditor.Forms.EventArgs;
using CosmosQueryEditor.Forms.Presenters;

namespace CosmosQueryEditor.Forms.Interfaces {
    public interface IMenuView
    {
        void ShowFolderDialog();
        void ShowFileDialog();
        EventHandler<FolderChangeEventArgs> OnFolderChange {get;set;}
        EventHandler<FileChangeEventArgs> OnFileChange {get;set;}
    }
}