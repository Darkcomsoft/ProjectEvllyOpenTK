﻿using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProjectEvlly.src;
using System.Windows;
using ProjectEvlly.src.User;
using System.Runtime.InteropServices;

namespace EvllyEngine
{
    class MainApplication
    {
        private static AuthManager auth = new AuthManager();

        public static void Main()
        {
#if Client
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            if (CheckLibrary("openal32.dll"))
            {
                Debug.Log("You Have OpenAL, you good to go!");
            }
            else
            {
                Debug.LogError("You don't Have OpanAL(Audio Library), you need to Download: https://www.openal.org/");
                GoToSite("https://www.openal.org/");
                return;
            }

            using (Window game = new Window(1000, 600, GlobalData.AppName + " : " + GlobalData.Version))
            {
                try
                {
                    game.Run();
                }
                catch (OutOfMemoryException memoryEx)
                {
                    Debug.LogWarning("GC: " + memoryEx.Message);
                    GC.Collect();
                    return;
                }
                catch (Exception ex)
                {
                    if (Game.Window != null)
                    {
                        Game.Window.Crash();
                        Debug.LogError(ex.Message + " StackTrace: " + ex.StackTrace);
                    }
                    else
                    {
                        Debug.LogError(ex.Message + " StackTrace: " + ex.StackTrace);
                    }
                }
            }

            AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
#endif

#if Server
            using (Server server = new Server())
            {
                server.Run();
            }
#endif
        }

        private static void CurrentDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                Debug.LogError("Unhadled domain exception:\n\n" + ex.Message);
            }
            catch (Exception exc)
            {
                try
                {
                    Debug.LogError("Fatal exception happend inside UnhadledExceptionHandler: \n\n" + exc.Message);
                }
                finally
                {
                    Environment.Exit(1);
                }
            }

            // It should terminate our main thread so Application.Exit() is unnecessary here
        }

        public static void GoToSite(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        [DllImport("kernel32", SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        static bool CheckLibrary(string fileName)
        {
            return LoadLibrary(fileName) != IntPtr.Zero;
        }
    }
}
