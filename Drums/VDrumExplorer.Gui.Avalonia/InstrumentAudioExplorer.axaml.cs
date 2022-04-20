// Copyright 2020 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using VDrumExplorer.ViewModel.Audio;

namespace VDrumExplorer.Gui
{
    /// <summary>
    /// Interaction logic for InstrumentAudioExplorer.xaml
    /// </summary>
    public partial class InstrumentAudioExplorer : Window
    {
        private InstrumentAudioExplorerViewModel ViewModel => (InstrumentAudioExplorerViewModel) DataContext;

        public InstrumentAudioExplorer()
        {
            InitializeComponent();
        }

        private async void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) =>
            await ViewModel.PlayAudio();

        private async void InputElement_OnDoubleTapped(object? sender, RoutedEventArgs e) =>
            await ViewModel.PlayAudio();
    }
}
