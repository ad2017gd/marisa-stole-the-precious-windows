using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace wip
{
    public class Video
    {
        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
    IntPtr hWnd,
    uint Msg,
    UIntPtr wParam,
    IntPtr lParam,
    SendMessageTimeoutFlags fuFlags,
    uint uTimeout,
    out UIntPtr lpdwResult);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr hWndChildAfter, string? className, string? windowTitle);


        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);


        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        public static bool stop = false;

        enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, uint flags);

        public static IntPtr FindWindowRecursive(IntPtr hParent, string? szClass, string? szCaption)
        {
            IntPtr hResult = FindWindowEx(hParent, IntPtr.Zero, szClass, szCaption);
            if (hResult != IntPtr.Zero)
                return hResult;
            IntPtr hChild = FindWindowEx(hParent, IntPtr.Zero, null, null);
            if (hChild != IntPtr.Zero)
            {
                do
                {
                    hResult = FindWindowRecursive(hChild, szClass, szCaption);
                    if (hResult != IntPtr.Zero)
                        return hResult;
                } while ((hChild = GetWindow(hChild, (uint)GetWindow_Cmd.GW_HWNDNEXT)) != IntPtr.Zero);
                return IntPtr.Zero;
            }
            else
                return IntPtr.Zero;
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
    IntPtr windowHandle,
    uint Msg,
    IntPtr wParam,
    IntPtr lParam,
    SendMessageTimeoutFlags flags,
    uint timeout,
    out IntPtr result);

        public delegate bool CallBackPtr(IntPtr hwnd, int lParam);

        [DllImport("user32.dll")]
        private static extern int EnumWindows(CallBackPtr callPtr, int lPar);



        public static VideoCapture video;
        public static Mat img = new Mat();
        public static Graphics gr;
        public static Bitmap bmp;
        public static IntPtr test;


        public static void Load()
        {
            IntPtr window = FindWindowRecursive(IntPtr.Zero, "Progman", null);

            IntPtr res = (IntPtr)0;

            SendMessageTimeout(window,
                0x052C,
                IntPtr.Zero,
                 IntPtr.Zero,
                SendMessageTimeoutFlags.SMTO_NORMAL,
                1000,
                out res);

            IntPtr test = IntPtr.Zero;


            EnumWindows(new CallBackPtr((tophandle, topparamhandle) =>
            {
                IntPtr p = FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            null);

                if (p != IntPtr.Zero)
                {
                    // Gets the WorkerW Window after the current one.
                    test = FindWindowEx(IntPtr.Zero,
                                               tophandle,
                                               "WorkerW",
                                               null);
                }

                return true;
            }), 0);


            //IntPtr shel = FindWindowRecursive(IntPtr.Zero, "SysListView32", null);
            //IntPtr dw = GetDC(test);



            gr = Graphics.FromHwnd(test);

            video = new VideoCapture("DesktopVideo.mp4");
        }

        public static Timer frameTimer;
        public static Timer timer;

        public static void Play()
        {
            
            int fps = (int)video.Get(Emgu.CV.CvEnum.CapProp.Fps);
            

            frameTimer = new Timer((_) =>
            {
                if (video.Grab())
                {
                    using (var img = video.QueryFrame())
                    {
                        bmp = img.ToBitmap();
                    }
                } else
                {
                    MessageBox.Show("No Video");
                }
            }, null, 0, 100);

            while(true)
            {
                if (bmp is not null && !stop)
                {
                    gr.DrawImage(bmp, new System.Drawing.Point(0, 0));
                }
            }


        }
    }
}
