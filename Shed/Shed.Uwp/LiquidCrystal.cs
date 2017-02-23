using System.Diagnostics;
using System.Text;
using Windows.Devices.I2c;

namespace Shed.Uwp
{
    public class LiquidCrystal
    {
        private readonly I2cDevice device;

        private bool backlight = true;
        public bool Backlight
        {
            get => backlight;
            set
            {
                backlight = value;
                ExpanderWrite(0);
            }
        }

        private bool display;
        public bool Display
        {
            get => display;
            set
            {
                display = value;
                SetDisplayControl();
            }
        }

        private bool cursor;
        public bool Cursor
        {
            get => cursor;
            set
            {
                cursor = value;
                SetDisplayControl();
            }
        }

        private bool blink;
        public bool Blink
        {
            get => blink;
            set
            {
                blink = value;
                SetDisplayControl();
            }
        }

        public LiquidCrystal(I2cDevice device)
        {
            this.device = device;
            Backlight = true;
            DelayMilliseconds(1000);

            Write4Bits(0x3 << 4);
            DelayMicroseconds(4500);
            Write4Bits(0x3 << 4);
            DelayMicroseconds(4500);
            Write4Bits(0x3 << 4);
            DelayMicroseconds(150);
            Write4Bits(0x2 << 4);

            Command(Commands.FunctionSet | FunctionSetFlag.Lines2 | FunctionSetFlag.BitMode4 | FunctionSetFlag.Dots5x8);
            Display = true;
            Clear();
            Command(Commands.EntryModeSet | DisplayEntryFlag.EntryLeft | DisplayEntryFlag.EntryShiftDecrement);
            Home();
        }

        private void SetDisplayControl()
        {
            byte command = Commands.DisplayControl;
            command |= Display ? DisplayControlFlag.DisplayOn : DisplayControlFlag.DisplayOff;
            command |= Cursor ? DisplayControlFlag.CursorOn : DisplayControlFlag.CursorOff;
            command |= Blink ? DisplayControlFlag.BlinkOn : DisplayControlFlag.BlinkOff;
            Command(command);
        }

        public void Print(string text)
        {
            var bytes = Encoding.ASCII.GetBytes(text);
            foreach (byte b in bytes)
            {
                Write(b);
            }
        }

        private static readonly byte[] rowOffsets = { 0x00, 0x40, 0x14, 0x54 };
        public void SetCursorPosition(int col, int row)
        {
            Command((byte) (Commands.SetDDramAddr | (col + rowOffsets[row])));
        }

        public void Clear()
        {
            Command(Commands.ClearDisplay);
            DelayMicroseconds(2000);
        }

        public void Home()
        {
            Command(Commands.ReturnHome);
            DelayMicroseconds(2000);
        }

        private void Command(byte command)
        {
            Send(command, 0);
        }

        private void Write(byte value)
        {
            Send(value, Foo.RegisterSelectBit);
        }

        private void Send(byte value, byte mode)
        {
            byte highNibble = (byte) (value & 0xf0);
            byte lowNibble = (byte) ((value << 4) & 0xf0);
            Write4Bits((byte) (highNibble | mode));
            Write4Bits((byte) (lowNibble | mode));
        }

        private void Write4Bits(byte value)
        {
            ExpanderWrite(value);
            PulseEnable(value);
        }

        private void ExpanderWrite(byte value)
        {
            if (backlight)
            {
                value |= BacklightFlag.On;
            }
            device.Write(new[] { value });
        }

        private void PulseEnable(byte value)
        {
            ExpanderWrite((byte) (value | Foo.EnableBit));
            DelayMicroseconds(1);
            ExpanderWrite((byte) (value & ~Foo.EnableBit));
            DelayMicroseconds(50);
        }

        private static void DelayMicroseconds(int microseconds)
        {
            var stopwatch = Stopwatch.StartNew();
            long ticks = Stopwatch.Frequency * microseconds / 1000000;
            while (stopwatch.ElapsedTicks < ticks)
            {
                // Just tight loop...
            }
        }

        private static void DelayMilliseconds(int milliseconds)
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < milliseconds)
            {
                // Just tight loop...
            }
        }

        private static class Commands
        {
            public const byte ClearDisplay = 1 << 0;
            public const byte ReturnHome = 1 << 1;
            public const byte EntryModeSet = 1 << 2;
            public const byte DisplayControl = 1 << 3;
            public const byte CursorShift = 1 << 4;
            public const byte FunctionSet = 1 << 5;
            public const byte SetCGramAddr = 1 << 6;
            public const byte SetDDramAddr = 1 << 7;
        }

        private static class DisplayEntryFlag
        {
            public const byte EntryRight = 0;
            public const byte EntryShiftIncrement = 1;
            public const byte EntryLeft = 2;
            public const byte EntryShiftDecrement = 0;
        }

        private static class DisplayControlFlag
        {
            public const byte DisplayOn = 4;
            public const byte DisplayOff = 0;
            public const byte CursorOn = 2;
            public const byte CursorOff = 0;
            public const byte BlinkOn = 1;
            public const byte BlinkOff = 0;
        }

        private static class CursorShiftFlag
        {
            public const byte DisplayMove = 8;
            public const byte CursorMove = 0;
            public const byte MoveRight = 4;
            public const byte MoveLeft = 0;
        }

        private static class FunctionSetFlag
        {
            public const byte BitMode8 = 0x10;
            public const byte BitMode4 = 0;
            public const byte Lines1 = 0;
            public const byte Lines2 = 8;
            public const byte Dots5x10 = 4;
            public const byte Dots5x8 = 0;
        }

        private static class BacklightFlag
        {
            public const byte On = 8;
            public const byte Off = 0;
        }

        private static class Foo
        {
            public const byte EnableBit = 0b100;
            public const byte ReadWriteBit = 0b010;
            public const byte RegisterSelectBit = 0b001;
        }

    }
}
