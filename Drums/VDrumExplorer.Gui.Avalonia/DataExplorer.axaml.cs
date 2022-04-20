// Copyright 2020 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using VDrumExplorer.ViewModel.Data;

namespace VDrumExplorer.Gui
{
    /// <summary>
    /// Interaction logic for ModuleExplorer.xaml
    /// </summary>
    public partial class DataExplorer : Window
    {
        private DataExplorerViewModel ViewModel => (DataExplorerViewModel) DataContext;

        public DataExplorer()
        {
            InitializeComponent();

            treeView.GetObservable(TreeView.SelectedItemProperty).Subscribe(x =>
            {
                if (x is DataTreeNodeViewModel node)
                {
                    ViewModel.SelectedNode = node;
                }
            });
        }
    }
}
