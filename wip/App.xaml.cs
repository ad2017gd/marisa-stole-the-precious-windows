using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Media;
using System.Windows.Media;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;

namespace wip
{

    public class IconExtractor
    {

        public static Icon Extract(string file, int number, bool largeIcon)
        {
            IntPtr large;
            IntPtr small;
            ExtractIconEx(file, number, out large, out small, 1);
            try
            {
                return Icon.FromHandle(largeIcon ? large : small);
            }
            catch
            {
                return null;
            }

        }
        [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

    }

    public static class IconHelper
    {
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter,
            int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hwnd, uint msg,
            IntPtr wParam, IntPtr lParam);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_DLGMODALFRAME = 0x0001;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOZORDER = 0x0004;
        const int SWP_FRAMECHANGED = 0x0020;
        const uint WM_SETICON = 0x0080;

        public static void RemoveIcon(Window window)
        {
            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(window).Handle;

            // Change the extended window style to not show a window icon
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_DLGMODALFRAME);

            // Update the window's non-client area to reflect the changes
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE |
                  SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }
    }
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Random rnd = new Random();

        public static List<BitmapImage> icons = new List<BitmapImage>();

        public static ImageSource? appIcon = null;


        public static List<CustomDialogWindow> openWindows = new List<CustomDialogWindow>();

        public enum PositionType
        {
            Relative,
            Absolute
        }

        public enum InstructionType
        {
            Window,
            Goto,
            Close,
            Sound,
            ColorFontWindow,
            Time
        }

        public static Dictionary<string, dynamic> instructions = new Dictionary<string, dynamic>()
        {



            // --------- WINDOW   WINDOW   WINDOW   WINDOW   WINDOW   WINDOW   WINDOW    ------------



            ["NVIDIALTR"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "NVIDIA Setup Error",
                Content =
@"Setup detected that the operating system in use is not Windows 
2000 or XP. This setup program and its associated drivers are
designed to run only on Windows 2000 or XP. The installation will
be terminated.",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(25, 25),
                Buttons = new List<string>() { "OK" },
                Random = false,
                Icon = System.Windows.Forms.MessageBoxIcon.Error
            },

            ["NVIDIARTL"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "NVIDIA Setup Error",
                Content =
@"Setup detected that the operating system in use is not Windows
2000 or XP. This setup program and its associated drivers are
designed to run only on Windows 2000 or XP. The installation will
be terminated.",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(-25, 25),
                Buttons = new List<string>() { "OK" },
                Random = false,
                Icon = System.Windows.Forms.MessageBoxIcon.Error
            },

            ["CATFAIL"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "Rename",
                Content =
@"If you change a file name extension, the file might become unusable.
Are you sure you want to change it?
",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(0, 150),
                Buttons = new List<string>() { "Yes", "No" },
                Random = false,
                Icon = System.Windows.Forms.MessageBoxIcon.Warning
            },

            ["DENIED"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "Access denied",
                Content =
@"Access denied.",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(575, 0),
                Random = false,
                Buttons = new List<string>() { "OK" },
                Icon = System.Windows.Forms.MessageBoxIcon.Information
            },

            ["OPENCAT"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "Open",
                Content =
@"Recycle Bin
Catastrophic Failure
",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(200, 0),
                Buttons = new List<string>() { "OK" },
                Random = false,
                Icon = System.Windows.Forms.MessageBoxIcon.Warning
            },

            ["CHORD"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                @"C:\Windows\notepad.exe",
                Content =
@"The version of this file is not compatible with the version of Windows you're running. Check
your computer's system information to see whether you need an x86 (32-bit) or x64 (64-bit)
version of the program, and then contact the software publisher.",
                PositionType = PositionType.Absolute,
                Position = new System.Drawing.Point((int)SystemParameters.PrimaryScreenWidth / 2 - 250, (int)SystemParameters.PrimaryScreenHeight / 2 - 150),
                Buttons = new List<string>() { "OK" },
                Random = false,
                Icon = System.Windows.Forms.MessageBoxIcon.Asterisk
            },

            ["BACKWARD"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "Open",
                Content =
@"Are you sure you want to delete these 2 items permanently?",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(-200, 0),
                Random = false,
                Buttons = new List<string>() { "Yes", "No", "Cancel" },
                Icon = System.Windows.Forms.MessageBoxIcon.Warning
            },

            ["LBL"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "Confirm Save As",
                Content =
@"Label Track.txt already exists.
Do you want to replace it?",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(0, 100),
                Random = true,
                Buttons = new List<string>() { "Yes", "Cancel" },
                Icon = System.Windows.Forms.MessageBoxIcon.Warning
            },

            ["IDOT"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "You Are an Idiot!",
                Content =
@"You Are an Idiot!",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(0, 100),
                Random = true,
                Buttons = new List<string>() { "You Are an Idiot!" },
                Icon = System.Windows.Forms.MessageBoxIcon.Information
            },


            ["APPERR"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "explorer.exe - Application Error",
                Content =
@"The instruction at 0x00007FFD061FCC60 referenced memory at
0x0000000000000000. The memory could not be read.
Click on OK to terminate the program",
                PositionType = PositionType.Relative,
                Random = false,
                Position = new System.Drawing.Point(300, 150),
                Buttons = new List<string>() { "OK" },
                Icon = System.Windows.Forms.MessageBoxIcon.Information
            },

            ["MAR"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "marisa.exe",
                Content =
@"Are you sure you want to quit?",
                PositionType = PositionType.Relative,
                Random = true,
                Position = new System.Drawing.Point(0, 0),
                Buttons = new List<string>() { "Yes", "No", "Cancel" },
                Icon = System.Windows.Forms.MessageBoxIcon.Question
            },

            ["UPDATE"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "Windows Update",
                Content =
@"Windows Update cannot currently check for updates, because the
service is not running. You may need to restart your computer.",
                PositionType = PositionType.Relative,
                Random = true,
                Position = new System.Drawing.Point(0, 0),
                Buttons = new List<string>() { "OK" },
                Icon = System.Windows.Forms.MessageBoxIcon.Information
            },

            ["SAVE"] = new
            {
                InstructionType = InstructionType.ColorFontWindow,
                Title =
                "Notepad",
                Content =
@"Do you want to save changes to Untitled?",
                Color = System.Windows.Media.Color.FromArgb(255, 38, 66, 139),
                FontSize = 16,
                PositionType = PositionType.Relative,
                Random = false,
                Position = new System.Drawing.Point(25, 25),
                Buttons = new List<string>() { "Save", "Don't save", "Cancel" },
                Icon = System.Windows.Forms.MessageBoxIcon.Warning,
            },

            ["ACTIVATE"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "Error details",
                Content =
@"The following information was found for this error:
Code:
0xC004C008
Description:
The activation server determined that the specified product key could
not be used.",
                PositionType = PositionType.Relative,
                Random = false,
                Position = new System.Drawing.Point(25, 25),
                Buttons = new List<string>() { "OK" },
                Icon = System.Windows.Forms.MessageBoxIcon.Asterisk,
            },

            ["DENIEDRND"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "Access denied",
                Content =
@"Access denied.",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(575, 0),
                Random = true,
                Buttons = new List<string>() { "OK" },
                Icon = System.Windows.Forms.MessageBoxIcon.Information
            },

            ["NET"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "InstallShield Wizard",
                Content =
@"The installation of Microsoft .NET Framework 4.7.2 Web appears to
have failed. Do you want to continue the installation?",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(25, 25),
                Random = false,
                Buttons = new List<string>() { "Yes", "No" },
                Icon = System.Windows.Forms.MessageBoxIcon.Question
            },

            ["TERRARIA"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "Open",
                Content =
@"Terraria
Catastrophic Failure
",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(-25, -25),
                Buttons = new List<string>() { "OK" },
                Random = false,
                Icon = System.Windows.Forms.MessageBoxIcon.Information
            },

            ["UNABL"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "winlogon.exe - Application Error",
                Content =
@"The application was unable to start correctly (0xc000007b). Click
OK to close the application.",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(0, -25),
                Random = false,
                Buttons = new List<string>() { "OK" },
                Icon = System.Windows.Forms.MessageBoxIcon.Error
            },

            ["RECYCL"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "Delete folder",
                Content =
@"Are you sure you want to move this folder to the Recycle Bin?",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(0, -25),
                Random = false,
                Buttons = new List<string>() { "Yes", "No" },
                Icon = System.Windows.Forms.MessageBoxIcon.Question
            },

            ["SYSPREP"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "System Preparation Tool",
                Content =
@"Sysprep was not able to validate your Windows installation.
Review the log file at
%WINDIR%\System32\Sysprep\Panther\setupact.log for
details. After resolving the issue, use Sysprep to validate your
installation again.",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(0, -25),
                Random = false,
                Buttons = new List<string>() { "OK" },
                Icon = System.Windows.Forms.MessageBoxIcon.Asterisk
            },


            ["USB"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "USB device not recognized",
                Content =
@"The last USB device you connected to this computer malfunctioned.
and Windows does not recognize it.",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(0, -25),
                Random = false,
                Buttons = new List<string>() { "OK" },
                Icon = System.Windows.Forms.MessageBoxIcon.Warning
            },

            ["LTRUNABL"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "winlogon.exe - Application Error",
                Content =
@"The application was unable to start correctly (0xc000007b). Click
OK to close the application.",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(100, 0),
                Random = false,
                Buttons = new List<string>() { "OK" },
                Icon = System.Windows.Forms.MessageBoxIcon.Error
            },

            ["LTRMAR"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "marisa.exe",
                Content =
@"Are you sure you want to quit?",
                PositionType = PositionType.Relative,
                Random = false,
                Position = new System.Drawing.Point(100, 0),
                Buttons = new List<string>() { "Yes", "No", "Cancel" },
                Icon = System.Windows.Forms.MessageBoxIcon.Question
            },

            ["RETRY"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "Error",
                Content =
@"Connection to https://ad2017.dev:5002 failed. Retrying...",
                PositionType = PositionType.Relative,
                Random = false,
                Position = new System.Drawing.Point(50, 0),
                Buttons = new List<string>() { "OK" },
                Icon = System.Windows.Forms.MessageBoxIcon.Warning
            },

            ["RTLSAVE"] = new
            {
                InstructionType = InstructionType.ColorFontWindow,
                Title =
                "Notepad",
                Content =
@"Do you want to save changes to Untitled?",
                Color = System.Windows.Media.Color.FromArgb(255, 38, 66, 139),
                FontSize = 16,
                PositionType = PositionType.Relative,
                Random = false,
                Position = new System.Drawing.Point(-25, 25),
                Buttons = new List<string>() { "Save", "Don't save", "Cancel" },
                Icon = System.Windows.Forms.MessageBoxIcon.Warning,
            },

            ["RTLRETRY"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "Error",
                Content =
@"Couldn't connect to https://ad2017.dev:5002 after 5 tries. Do you want to try again?",
                PositionType = PositionType.Relative,
                Random = false,
                Position = new System.Drawing.Point(-25, 25),
                Buttons = new List<string>() { "Yes", "No" },
                Icon = System.Windows.Forms.MessageBoxIcon.Question
            },

            ["RTLSYSPREP"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "System Preparation Tool",
                Content =
@"Sysprep was not able to validate your Windows installation.
Review the log file at
%WINDIR%\System32\Sysprep\Panther\setupact.log for
details. After resolving the issue, use Sysprep to validate your
installation again.",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(-25, 25),
                Random = false,
                Buttons = new List<string>() { "OK" },
                Icon = System.Windows.Forms.MessageBoxIcon.Asterisk
            },

            ["RTLRETRY"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "Error",
                Content =
@"Connection to https://ad2017.dev:5002 failed. Retrying...",
                PositionType = PositionType.Relative,
                Random = false,
                Position = new System.Drawing.Point(-25, 25),
                Buttons = new List<string>() { "OK" },
                Icon = System.Windows.Forms.MessageBoxIcon.Warning
            },

            ["RTLDENIED"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "Access denied",
                Content =
@"Access denied.",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(-25, 25),
                Random = false,
                Buttons = new List<string>() { "OK" },
                Icon = System.Windows.Forms.MessageBoxIcon.Information
            },

            ["RTLMAR"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "marisa.exe",
                Content =
@"Are you sure you want to quit?",
                PositionType = PositionType.Relative,
                Random = false,
                Position = new System.Drawing.Point(-25, 25),
                Buttons = new List<string>() { "Yes", "No", "Cancel" },
                Icon = System.Windows.Forms.MessageBoxIcon.Question
            },

            ["RTLUSB"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "USB device not recognized",
                Content =
@"The last USB device you connected to this computer malfunctioned.
and Windows does not recognize it.",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(-25, 25),
                Random = false,
                Buttons = new List<string>() { "OK" },
                Icon = System.Windows.Forms.MessageBoxIcon.Warning
            },

            ["RTLAPPERR"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "explorer.exe - Application Error",
                Content =
@"The instruction at 0x00007FFD061FCC60 referenced memory at
0x0000000000000000. The memory could not be read.
Click on OK to terminate the program",
                PositionType = PositionType.Relative,
                Random = false,
                Position = new System.Drawing.Point(-25, 25),
                Buttons = new List<string>() { "OK" },
                Icon = System.Windows.Forms.MessageBoxIcon.Error
            },

            ["RTLNET"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "InstallShield Wizard",
                Content =
@"The installation of Microsoft .NET Framework 4.7.2 Web appears to
have failed. Do you want to continue the installation?",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(25, 25),
                Random = false,
                Buttons = new List<string>() { "Yes", "No" },
                Icon = System.Windows.Forms.MessageBoxIcon.Question
            },

            ["RTLNVIDIA"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "NVIDIA Setup Error",
                Content =
@"Setup detected that the operating system in use is not Windows
2000 or XP. This setup program and its associated drivers are
designed to run only on Windows 2000 or XP. The installation will
be terminated.",
                PositionType = PositionType.Relative,
                Position = new System.Drawing.Point(-25, 25),
                Buttons = new List<string>() { "OK" },
                Random = false,
                Icon = System.Windows.Forms.MessageBoxIcon.Error
            },

            ["RTLAPPERROR"] = new
            {
                InstructionType = InstructionType.Window,
                Title =
                "explorer.exe - Application Error",
                Content =
@"The instruction at 0x00007FFD061FCC60 referenced memory at
0x0000000000000000. The memory could not be read.
Click on OK to terminate the program",
                PositionType = PositionType.Relative,
                Random = false,
                Position = new System.Drawing.Point(-25, 25),
                Buttons = new List<string>() { "OK" },
                Icon = System.Windows.Forms.MessageBoxIcon.Error
            },

            // -------------GOTO   GOTO   GOTO   GOTO   GOTO   GOTO   GOTO   GOTO   GOTO   GOTO   GOTO   --------------------




            ["GOTOLEFT-500"] = new
            {
                 InstructionType = InstructionType.Goto,
                Position = new System.Drawing.Point((int)SystemParameters.PrimaryScreenWidth - 500, 50)
            },

            ["GOTOBL"] = new
            {
                InstructionType = InstructionType.Goto,
                Position = new System.Drawing.Point(5, (int)SystemParameters.PrimaryScreenHeight - 300)
            },

            ["GOTOMR"] = new
            {
                InstructionType = InstructionType.Goto,
                Position = new System.Drawing.Point((int)SystemParameters.PrimaryScreenWidth - 400, (int)SystemParameters.PrimaryScreenHeight / 2 - 115)
            },
            ["GOTOML"] = new
            {
                InstructionType = InstructionType.Goto,
                Position = new System.Drawing.Point(0, (int)SystemParameters.PrimaryScreenHeight / 2 - 150)
            },

            ["GOTOTL"] = new
            {
                InstructionType = InstructionType.Goto,
                Position = new System.Drawing.Point(5, 50)
            },

            ["GOTOCENTER"] = new
            {
                 InstructionType = InstructionType.Goto,
                Position = new System.Drawing.Point((int)SystemParameters.PrimaryScreenWidth / 2 - 460/2, 50)
            },

            ["GOTOCENTERB"] = new
            {
                InstructionType = InstructionType.Goto,
                Position = new System.Drawing.Point((int)SystemParameters.PrimaryScreenWidth / 2 - 460 / 2, (int)SystemParameters.PrimaryScreenHeight - 250)
            },

            ["GOTOCENTER3"] = new
            {
                InstructionType = InstructionType.Goto,
                Position = new System.Drawing.Point((int)SystemParameters.PrimaryScreenWidth / 2, 50)
            },



            ["GOTOCENTER2"] = new
            {
                InstructionType = InstructionType.Goto,
                Position = new System.Drawing.Point(0, (int)SystemParameters.PrimaryScreenHeight / 2 - 75)
            },

            // -------------- SOUND   SOUND   SOUND   SOUND   SOUND   SOUND   SOUND   ----------

            ["DING"] = new
            {
                InstructionType = InstructionType.Sound,
                Location = @"C:\Windows\Media\Windows Ding.wav"
            },

            ["HWIN"] = new
            {
                InstructionType = InstructionType.Sound,
                Location = @"C:\Windows\Media\Windows Hardware Insert.wav"
            },


            ["HWOUT"] = new
            {
                InstructionType = InstructionType.Sound,
                Location = @"C:\Windows\Media\Windows Hardware Remove.wav"
            },


            //////////////////////////close/////////////////////

            ["CLOSE"] = new
            {
                InstructionType = InstructionType.Close
            },



            ["STOP"] = new
            {
                InstructionType = InstructionType.Time
            }
        };

        public static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
        public static void NOP(double durationSeconds)
        {
            var durationTicks = Math.Round(durationSeconds * Stopwatch.Frequency);
            var sw = Stopwatch.StartNew();

            while (sw.ElapsedTicks < durationTicks)
            {

            }
        }

        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint MM_BeginPeriod(uint uMilliseconds);

        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        public static extern uint MM_EndPeriod(uint uMilliseconds);

        public App()
        {
            CultureInfo culture = new CultureInfo("en-US");

            icons.Add(ToBitmapImage(SystemIcons.Question.ToBitmap()));
            icons.Add(ToBitmapImage(SystemIcons.Warning.ToBitmap()));
            icons.Add(ToBitmapImage(SystemIcons.Hand.ToBitmap()));
            icons.Add(ToBitmapImage(SystemIcons.Information.ToBitmap()));

            Video.Load();
            var nw = new CustomDialogWindow("a", "a", new List<string>() { "Yes" }, System.Windows.Forms.MessageBoxIcon.Error);
            nw.Show();
            Thread.Sleep(1000);
            nw.Left = 9999;


            var lines = File.ReadAllLines("Label Track.txt");




            Thread tht = new Thread(() =>
            {
                Video.Play();
            });

            SoundPlayer sp = new SoundPlayer("NormalizedMusic.wav");
            //NOP(11/1000.0);
            tht.Start();

            var cursor = new System.Drawing.Point(50, 50);
            var i = 0;


            var stopwatch = new Stopwatch();

            var timer = new HighResolutionTimer.HighResolutionTimer(1.0f);
            timer.UseHighPriorityThread = true;
            timer.Elapsed += (_,_) =>
            {
                Debug.WriteLine("Tick " + stopwatch.ElapsedMilliseconds / 1000.0f);
                var line = lines[i];
                var previous = i == 0 ? "0.000000" : lines[i - 1];

                var matches = Regex.Matches(line, @"[^	\n]+")
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .ToArray();
                var matchesprev = Regex.Matches(previous, @"[^	\n]+")
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .ToArray();

                if (!(stopwatch.ElapsedMilliseconds / 1000.0 >= double.Parse(matches[0], culture))) return;
                i++;

                double time = double.Parse(matches[0]) - double.Parse(matchesprev[0], culture);
                //if (time > 0) NOP(time); ;


                Thread t2 = new Thread(new ThreadStart(() =>
                {
                    dynamic data = instructions[matches[2]];
                    if (data.InstructionType == InstructionType.Window || data.InstructionType == InstructionType.ColorFontWindow)
                    {
                        var wnd = new CustomDialogWindow(data.Content, data.Title, data.Buttons, data.Icon);

                        App.Current.Dispatcher.Invoke(() => App.openWindows.Add(wnd));
                        if (data.Random)
                        {
                            wnd.Left = rnd.NextInt64(0, (int)SystemParameters.PrimaryScreenWidth - 350);
                            wnd.Top = rnd.NextInt64(0, (int)SystemParameters.PrimaryScreenHeight - 200);
                        }
                        else
                        if (data.PositionType == PositionType.Relative)
                        {
                            wnd.Left = cursor.X + data.Position.X;
                            wnd.Top = cursor.Y + data.Position.Y;
                        }
                        else
                        {
                            wnd.Left = data.Position.X;
                            wnd.Top = data.Position.Y;
                        }
                        cursor.X = (int)wnd.Left;
                        cursor.Y = (int)wnd.Top;

                        if (data.InstructionType == InstructionType.ColorFontWindow)
                        {
                            wnd.IconImage.Visibility = Visibility.Collapsed;
                            wnd.Label.Foreground = new SolidColorBrush(data.Color);
                            wnd.Label.FontSize = data.FontSize;
                            Thickness margin = wnd.StackPanel.Margin;
                            margin.Top = 12;
                            wnd.StackPanel.Margin = margin;
                        }


                        wnd.Show();


                        System.Windows.Threading.Dispatcher.Run();
                    }
                    else if (data.InstructionType == InstructionType.Goto)
                    {
                        cursor.X = (int)data.Position.X;
                        cursor.Y = (int)data.Position.Y;
                    }
                    else if (data.InstructionType == InstructionType.Close)
                    {
                        List<CustomDialogWindow>? wnds = null;
                        Dispatcher.Invoke(() => wnds = new List<CustomDialogWindow>(openWindows));
                        foreach (var w in wnds.ToList())
                        {
                            if (!w.IsClosed) w.Dispatcher.InvokeAsync(() => w.Close());
                        }
                        Dispatcher.Invoke(() => openWindows = new List<CustomDialogWindow>());
                    }

                    else if (data.InstructionType == InstructionType.Sound)
                    {
                        var player = new MediaPlayer();
                        player.Open(new Uri(data.Location));
                        player.Volume = 1;
                        player.Play();
                    }

                    else if (data.InstructionType == InstructionType.Time)
                    {
                        Video.stop = true;
                        Thread.Sleep(1500);

                        Environment.Exit(0);
                    }
                }));
                t2.IsBackground = true;
                t2.SetApartmentState(ApartmentState.STA);
                t2.Start();

            };
            stopwatch.Start();
            sp.Play();
            NOP(30.0 / 1000.0);
            timer.Start();
            
            //Thread.Sleep(500);



        }
    }
}
