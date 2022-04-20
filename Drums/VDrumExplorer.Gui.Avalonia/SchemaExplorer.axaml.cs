// Copyright 2020 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VDrumExplorer.ViewModel.LogicalSchema;

namespace VDrumExplorer.Gui
{
    /// <summary>
    /// Interaction logic for SchemaExplorer.xaml
    /// </summary>
    public partial class SchemaExplorer : Window
    {
        public SchemaExplorer()
        {
            InitializeComponent();
        }

        private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            ((ModuleSchemaViewModel) DataContext).SelectedNode = (TreeNodeViewModel) treeView.SelectedItem;
        }
    }
}
