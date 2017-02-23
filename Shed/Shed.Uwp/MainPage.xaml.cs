// Copyright 2016 Jon Skeet.
// Licensed under the Apache License Version 2.0.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;
using Windows.Media.SpeechRecognition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using static Shed.Controllers.Factory;

namespace Shed.Uwp
{
    // This just makes the initialization of MainPage.handlers simpler.
    internal static class DictionaryExtensions
    {
        internal static Dictionary<string, TValue> WithKeyPrefix<TValue>(
            this Dictionary<string, TValue> source,
            string prefix)
            => source.ToDictionary(pair => prefix + pair.Key, pair => pair.Value);
    }

    public sealed partial class MainPage : Page
    {
        private const double ConfidenceThreshold = 0.6;
        private const string Prefix = "shed ";

        private static readonly Dictionary<string, Action> handlers = new Dictionary<string, Action>
        {
            { "lights on", Lighting.On },
            { "lights off", Lighting.Off },
            { "music play", Sonos.Play },
            { "music pause", Sonos.Pause },
            { "music mute", () => Sonos.SetVolume(0) },
            { "music quiet", () => Sonos.SetVolume(30) },
            { "music medium", () => Sonos.SetVolume(60) },
            { "music loud", () => Sonos.SetVolume(90) },
            { "music next", Sonos.Next },
            { "music previous", Sonos.Previous },
            { "music restart", Sonos.Restart },
            { "amplifier on", Amplifier.On },
            { "amplifier off", Amplifier.Off },
            { "amplifier mute", () => Amplifier.SetVolume(0) },
            { "amplifier quiet", () => Amplifier.SetVolume(30) },
            { "amplifier medium", () => Amplifier.SetVolume(50) },
            { "amplifier loud", () => Amplifier.SetVolume(60) },
            { "amplifier input dock", () => Amplifier.Source("dock") },
            { "amplifier input pie", () => Amplifier.Source("pi") },
            { "amplifier input sonos", () => Amplifier.Source("sonos") },
            { "amplifier input playstation", () => Amplifier.Source("ps4") }
        }.WithKeyPrefix(Prefix);

        // Unclear whether we really need an instance variable here. If we just
        // used a local variable in RegisterVoiceActivation, would the instance be
        // garbage collected?
        private SpeechRecognizer recognizer;

        private readonly LiquidCrystal lcd;
        private GpioPin buttonPin;
        private GpioPin pirPin;

        public MainPage()
        {
            this.InitializeComponent();
            lcd = Task.Run(StartLcd).Result;
            InitBacklightButton();
            InitPirDetection();
        }
        
        private void TurnOnLights(object sender, RoutedEventArgs e)
        {
            Lighting.On();
        }

        private void TurnOffLights(object sender, RoutedEventArgs e)
        {
            Lighting.Off();
        }

        private async void RegisterVoiceActivation(object sender, RoutedEventArgs e)
        {
            recognizer = new SpeechRecognizer
            {
                Constraints = { new SpeechRecognitionListConstraint(handlers.Keys) }
            };
            recognizer.ContinuousRecognitionSession.ResultGenerated += HandleVoiceCommand;
            recognizer.ContinuousRecognitionSession.AutoStopSilenceTimeout = TimeSpan.FromDays(1000);
            recognizer.StateChanged += HandleStateChange;

            SpeechRecognitionCompilationResult compilationResult = await recognizer.CompileConstraintsAsync();
            if (compilationResult.Status == SpeechRecognitionResultStatus.Success)
            {
                await recognizer.ContinuousRecognitionSession.StartAsync();
            }
            else
            {
                await Dispatcher.RunIdleAsync(_ => lastState.Text = $"Compilation failed: {compilationResult.Status}");
            }
        }

        private async void HandleStateChange(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            await Dispatcher.RunIdleAsync(_ => lastState.Text = args.State.ToString());
        }

        private async void HandleVoiceCommand(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            string text = args.Result.Text;
            // TODO: Clear this 5 seconds later, or add a timestamp.
            await Dispatcher.RunIdleAsync(_ =>
            {
                lastText.Text = text;
                lastConfidence.Text = $"{args.Result.Confidence} ({args.Result.RawConfidence})";
                SetLcdText(text, $"{args.Result.Confidence} ({args.Result.RawConfidence})");
            });
            if (args.Result.RawConfidence >= ConfidenceThreshold)
            {
                Action handler;
                if (handlers.TryGetValue(text, out handler))
                {
                    // Asynchronously call the handler; we don't care about the result.
                    // (We might want ContinueWith on error at some point...)
#pragma warning disable CS4014
                    Task.Run(handler);
#pragma warning restore
                }
            }
        }

        private async Task<LiquidCrystal> StartLcd()
        {
            var settings = new I2cConnectionSettings(0x27); // See https://arduino-info.wikispaces.com/LCD-Blue-I2C
            //settings.BusSpeed = I2cBusSpeed.FastMode;                       /* 400KHz bus speed */

            string selector = I2cDevice.GetDeviceSelector();                     /* Get a selector string that will return all I2C controllers on the system */
            var deviceInfos = await DeviceInformation.FindAllAsync(selector);            /* Find the I2C bus controller devices with our selector string             */
            var lcd = await I2cDevice.FromIdAsync(deviceInfos[0].Id, settings);
            return new LiquidCrystal(lcd);
            // https://github.com/fdebrabander/Arduino-LiquidCrystal-I2C-library/blob/master/LiquidCrystal_I2C.cpp
        }

        private void SetLcdText(string row1, string row2)
        {
            lcd.Clear();
            lcd.SetCursorPosition(0, 0);
            lcd.Print(row1);
            lcd.SetCursorPosition(0, 1);
            lcd.Print(row2);
        }

        private void InitBacklightButton()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                lcd.Print("There is no GPIO controller on this device.");
                return;
            }

            buttonPin = gpio.OpenPin(5);

            // Check if input pull-up resistors are supported
            if (buttonPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonPin.SetDriveMode(GpioPinDriveMode.Input);

            // Set a debounce timeout to filter out switch bounce noise from a button press
            buttonPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);

            // Toggle the LCD backlight when we press the button (rising edge is release)
            buttonPin.ValueChanged += (pin, args) =>
            {
                if (args.Edge == GpioPinEdge.FallingEdge)
                {
                    Dispatcher.RunIdleAsync(_ => lcd.Backlight = !lcd.Backlight);
                }
            };

            lcd.Print("GPIO pins initialized.");
        }

        private void InitPirDetection()
        {
            var gpio = GpioController.GetDefault();
            pirPin = gpio.OpenPin(17);
            pirPin.SetDriveMode(GpioPinDriveMode.Input);
            pirPin.DebounceTimeout = TimeSpan.FromMilliseconds(200);
            pirPin.ValueChanged += (pin, args) =>
            {
                var edge = args.Edge;
                Dispatcher.RunIdleAsync(_ => pir.Text = edge.ToString());
            };
        }
    }
}
