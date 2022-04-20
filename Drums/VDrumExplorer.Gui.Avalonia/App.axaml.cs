using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Logging;
using VDrumExplorer.Model.Audio;
using VDrumExplorer.Model.Midi;
using VDrumExplorer.NAudio;
using VDrumExplorer.ViewModel;
using VDrumExplorer.ViewModel.Logging;

namespace VDrumExplorer.Gui
{
    public partial class App : Application
    {
        private readonly IAudioDeviceManager audioDeviceManager;
        private DeviceViewModel deviceViewModel;
        private LogViewModel logViewModel;

        public App()
        {
            MidiDevices.Manager = new Midi.ManagedMidi.MidiManager();
            audioDeviceManager = new NAudioDeviceManager();
            deviceViewModel = new DeviceViewModel();
            logViewModel = new LogViewModel();
            /* TODO:
            DispatcherUnhandledException += (sender, args) =>
            {
                logViewModel.Logger.LogError(args.Exception, "Unhandled exception");
                args.Handled = true;
            };
            */
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var viewModel = new ExplorerHomeViewModel(ViewServices.Instance, logViewModel, deviceViewModel, audioDeviceManager);
                desktop.MainWindow = new ExplorerHome { DataContext = viewModel };
                
                logViewModel.LogVersion(GetType());
                // TODO: await ?
                await deviceViewModel.DetectModule(logViewModel.Logger);

                desktop.Exit += OnExit;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            deviceViewModel?.ConnectedDevice?.Dispose();
        }
    }
}