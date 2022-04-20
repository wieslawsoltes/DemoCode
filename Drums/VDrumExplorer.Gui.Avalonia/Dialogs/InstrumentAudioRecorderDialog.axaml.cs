// Copyright 2020 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VDrumExplorer.ViewModel.Dialogs;

namespace VDrumExplorer.Gui.Dialogs
{
    /// <summary>
    /// Interaction logic for InstrumentAudioRecorderDialog.xaml
    /// </summary>
    public partial class InstrumentAudioRecorderDialog : Window
    {
        public bool? DialogResult { get; set; }

        private InstrumentAudioRecorderViewModel ViewModel => (InstrumentAudioRecorderViewModel) DataContext;

        public InstrumentAudioRecorderDialog()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void Cancel(object sender, CancelEventArgs e) =>
            ViewModel.Cancel();
    }
}
