using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Doobry.Infrastructure;

namespace Doobry.Features.QueryDeveloper
{
    public class ResultSetExplorerViewModel : INotifyPropertyChanged
    {
        private int _selectedRow = -1;
        private ResultSet _resultSet;
        private bool _isError;
        private string _error;
        private string _totalCost;

        public ResultSetExplorerViewModel(ICommand fetchMoreCommand, ICommand editDocumentCommand,
            ICommand deleteDocumentCommand)
        {
            FetchMoreCommand = fetchMoreCommand ?? throw new ArgumentNullException(nameof(fetchMoreCommand));
            EditDocumentCommand = editDocumentCommand;
            DeleteDocumentCommand = deleteDocumentCommand;
            SaveDocumentCommand = new Command(o => SaveDocument((Result) o), o => o is Result);
        }

        /// <summary>
        /// Gets or sets the result set.
        /// </summary>
        /// <value>
        /// The result set.
        /// </value>
        public ResultSet ResultSet
        {
            get => _resultSet;
            set
            {
                this.MutateVerbose(ref _resultSet, value, RaisePropertyChanged());


                if (_resultSet != null)
                {
                    TotalCost = $"{_resultSet.Cost} RU's";
                    if (!string.IsNullOrEmpty(_resultSet.Error))
                    {
                        IsError = true;
                        Error = _resultSet.Error;
                        SelectedRow = -1;
                    }
                    else
                    {
                        IsError = false;
                        Error = null;
                        if (_resultSet.Results.Count > 0)
                            SelectedRow = 0;
                        else
                            SelectedRow = -1;
                    }
                }
                else
                {
                    TotalCost = "";
                    SelectedRow = -1;
                }

            }
        }

        public ICommand FetchMoreCommand { get; }

        public ICommand DeleteDocumentCommand { get; }

        public ICommand EditDocumentCommand { get; }

        public ICommand SaveDocumentCommand { get; }

        public bool IsError
        {
            get => _isError;
            private set => this.MutateVerbose(ref _isError, value, RaisePropertyChanged());
        }

        public string Error
        {
            get => _error;
            private set { this.MutateVerbose(ref _error, value, RaisePropertyChanged()); }
        }

        public int SelectedRow
        {
            get => _selectedRow;
            set => this.MutateVerbose(ref _selectedRow, value, RaisePropertyChanged());
        }

        public string TotalCost
        {
            get => _totalCost;
            private set => this.MutateVerbose(ref _totalCost, value, RaisePropertyChanged());
        }

        private void SaveDocument(Result result)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Document",
                DefaultExt = ".json",
                Filter = "JSON documents (.json)|*.json|Text documents (.txt)|*.txt"
            };

            var showDialogResult = saveFileDialog.ShowDialog();
            if (!showDialogResult.HasValue || !showDialogResult.Value) return;
            try
            {
                File.WriteAllText(saveFileDialog.FileName, result.Data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An Error Occurred");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
