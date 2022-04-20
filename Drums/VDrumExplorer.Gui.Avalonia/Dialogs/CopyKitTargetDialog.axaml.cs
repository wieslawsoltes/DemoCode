// Copyright 2020 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace VDrumExplorer.Gui.Dialogs
{
    /// <summary>
    /// Interaction logic for CopyKitTargetDialog.xaml
    /// </summary>
    public partial class CopyKitTargetDialog : Window
    {
        public bool? DialogResult { get; set; }

        public CopyKitTargetDialog()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
