// Copyright 2020 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text;
using Microsoft.Win32;
using System.Threading.Tasks;
using VDrumExplorer.Gui.Dialogs;
using VDrumExplorer.ViewModel;
using VDrumExplorer.ViewModel.Audio;
using VDrumExplorer.ViewModel.Data;
using VDrumExplorer.ViewModel.Dialogs;
using VDrumExplorer.ViewModel.LogicalSchema;

namespace VDrumExplorer.Gui
{
    internal class ViewServices : IViewServices
    {
        internal static ViewServices Instance { get; } = new ViewServices();

        private ViewServices()
        {
        }

        private string ToFilter(FileFilter[] filters)
        {
            var sb = new StringBuilder();
  
            for (var i = 0; i < filters.Length; i++)
            {
                var filter = filters[i];
                var extensions = filter.Extensions;
 
                sb.Append(filter.Name);
                sb.Append('|');

                for (var j = 0; j < extensions.Length; j++)
                {
                    sb.Append("*.");
                    sb.Append(extensions[j]);
                    if (extensions.Length > 1 && j < extensions.Length - 1)
                    {
                        sb.Append(';');
                    }
                }

                if (filters.Length > 1 && i < filters.Length - 1)
                {
                    sb.Append('|');
                }
            }

            return sb.ToString();
        }

        public Task<string?> ShowOpenFileDialog(FileFilter[] filter)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = ToFilter(filter)
            };
            return Task.FromResult(dialog.ShowDialog() == true ? dialog.FileName : null);
        }

        public Task<string?> ShowSaveFileDialog(FileFilter[] filter)
        {
            SaveFileDialog dialog = new SaveFileDialog { Filter = ToFilter(filter) };
            return Task.FromResult(dialog.ShowDialog() == true ? dialog.FileName : null);
        }

        public Task<int?> ChooseCopyKitTarget(CopyKitViewModel viewModel)
        {
            var dialog = new CopyKitTargetDialog { DataContext = viewModel };
            var result = dialog.ShowDialog();
            return Task.FromResult(result == true ? viewModel.DestinationKitNumber : default(int?));
        }

        public void ShowKitExplorer(KitExplorerViewModel viewModel) =>
            new DataExplorer { DataContext = viewModel }.Show();

        public void ShowModuleExplorer(ModuleExplorerViewModel viewModel) =>
            new DataExplorer { DataContext = viewModel }.Show();

        public void ShowSchemaExplorer(ModuleSchemaViewModel viewModel) =>
            new SchemaExplorer { DataContext = viewModel }.Show();

        public void ShowInstrumentRecorderDialog(InstrumentAudioRecorderViewModel viewModel)
        {
            var recorder = new InstrumentAudioRecorderDialog { DataContext = viewModel };
            // Ugly hack: we can't bind DialogResult to the ViewModel in XAML, so let's just do it here.
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.RecordedAudio))
                {
                    recorder.DialogResult = true;
                }
            };
            recorder.ShowDialog();
        }

        public void ShowInstrumentAudioExplorer(InstrumentAudioExplorerViewModel viewModel) =>
            new InstrumentAudioExplorer { DataContext = viewModel }.Show();

        public async Task<T?> ShowDataTransferDialog<T>(DataTransferViewModel<T> viewModel)
            where T : class
        {
            var dialog = new DataTransferDialog { DataContext = viewModel };
            Task<T>? task = null;
            // Ugly hack: we can't bind DialogResult to the ViewModel in XAML, so let's just do it here.
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.DialogResult))
                {
                    dialog.DialogResult = viewModel.DialogResult;
                }
            };
            // Second ugly hack: only start the transfer when the dialog is shown. Without this,
            // there's a race condition: if the transfer fails immediately, we try to set DialogResult
            // before the dialog is shown, and that's ignored.
            dialog.Activated += (sender, args) =>
            {
                if (task is null)
                {
                    task = viewModel.TransferAsync();
                }
            };
            // TODO: We don't close the dialog if this has already failed. Check what's going on.
            var result = dialog.ShowDialog();
            return result == true ? await task! : null;
        }
    }
}
