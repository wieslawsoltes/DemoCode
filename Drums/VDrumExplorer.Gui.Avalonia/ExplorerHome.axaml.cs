// Copyright 2020 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VDrumExplorer.Gui
{
    /// <summary>
    /// Interaction logic for ExplorerHome.xaml
    /// </summary>
    public partial class ExplorerHome : Window
    {
        public ExplorerHome()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }
    }
}
