﻿using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Threading;
using ProjectEvlly;
using ProjectEvlly.src.Net;
using ProjectEvlly.src.UI;
using ProjectEvlly.src.Utility;
using System.Drawing;
using ProjectEvlly.src;
using ProjectEvlly.src.Engine.Render;
using ProjectEvlly.src.Engine;

namespace EvllyEngine
{
    public class Window : GameWindow
    {
        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) {
            Instance = this;
            Game.Window = this;
        }

        public static Window Instance;
        private AppDomain currentDomain;

        public Client OGame;

        private AssetsManager _assetsManager;
        private Physics _physics;
        private SplashScreen _SplashScreen;
        private Frustum frustum;
        private SoundSystem _soundSystem;
        private GUI _GUIRender;

        private bool IsDebug = false;//this is not the visual studio solution plataform, is for testing debugs things like OpenGL Render only lines of meshes
        private BeginMode GLBeginMode = BeginMode.Triangles;

        private bool GameLoaded = false;
        private bool EngineIsReady = false;
        private int crashTimeOut = 0;

        private int FRB;

        protected override void OnLoad(EventArgs e)//Load the Window, but not the systems, loading only the splash screen
        {
            gl.ClearColor(Color.Black);
            Utilitys.CheckGLError("Set ClearColor");

            //Start GL Frame Buffer

            GlobalData.TargetFrameRate = 60;

            VSync = VSyncMode.Off;
            WindowBorder = WindowBorder.Resizable;

            _SplashScreen = new SplashScreen();
            
            currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);

            base.OnLoad(e);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Time._DeltaTime = (float)e.Time;
            Time._DTime += e.Time;

            NextFrameQueue.Tick();
            
            if (EngineIsReady)
            {
                if (Input.GetKeyDown(Key.F4))
                {
                    if (IsDebug)
                    {
                        IsDebug = false;
                        GLBeginMode = BeginMode.Triangles;
                    }
                    else
                    {
                        IsDebug = true;
                        GLBeginMode = BeginMode.Lines;
                    }
                }

                OGame.Tick();
                _GUIRender.Tick();

                if (Focused) // check to see if the window is focused  
                {
                    MouseCursor.CursorLockPosition();
                }

                _physics.UpdatePhisics((float)e.Time);
            }
            else if (!GameLoaded)//first load the window, before load the systems
            {
                LoadGame();
            }

            Time.UPS = (int)(1f / e.Time);
            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Utilitys.CheckGLError("GL.Clear");

            if (EngineIsReady)
            {
                gl.Enable(EnableCap.DepthTest);
                OGame.Draw(e);
                gl.Disable(EnableCap.DepthTest);
                _GUIRender.TickRender();
            }

            SwapBuffers();
            Utilitys.CheckGLError("Window SwapBuffers");

            Time.FPS = (int)(1f / e.Time);

            Time._Time++;
            Time._Tick = Time._Time % 60;

            if (Time._Time >= double.MaxValue)
            {
                Time._Time = -Time._Time;
            }
            Thread.Sleep(60 / 5);
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            gl.Viewport(0, 0, Width, Height);

            if (EngineIsReady)
            {
                _GUIRender.OnResize();
            }
            base.OnResize(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            _GUIRender.OnKeyPress(e);
            base.OnKeyPress(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (EngineIsReady)
            {
                _GUIRender.OnMouseMove(e);
                Input.SetMousePos(e.Position);
            }
            base.OnMouseMove(e);
        }

        private void LoadGame()
        {
            GameLoaded = true;//let this on the top

            SplashScreen.SetState("Starting SoundEngine...", SplashScreenStatus.Loading);
            _soundSystem = new SoundSystem();
            
            SplashScreen.SetState("Loading Assets...", SplashScreenStatus.Loading);
            _assetsManager = new AssetsManager();

            SplashScreen.SetState("Starting Physics", SplashScreenStatus.Loading);
            _physics = new Physics();

            SplashScreen.SetState("Starting GUI", SplashScreenStatus.Loading);
            _GUIRender = new GUI();

            SplashScreen.SetState("Starting Frustum", SplashScreenStatus.Loading);
            frustum = new Frustum();

            SplashScreen.SetState("Starting Client", SplashScreenStatus.Loading);
            OGame = new Client();

            SplashScreen.SetState("Setting-Up OpenGL", SplashScreenStatus.Loading);

            SplashScreen.SetState("Finished Loading. We Good to Go (:", SplashScreenStatus.finished);
            EngineIsReady = true;
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Debug.LogError("GameLoop: Unhandled exception: " + args.ExceptionObject);
            System.Environment.Exit(2);
        }

        public void Crash()
        {
            if (crashTimeOut >= 10)
            {
                Debug.Log("We cant start again the game, before the crash! Sorry ): Shutingdown....");
                SplashScreen.SetState("We cant start again the game, before the crash! Sorry ): Shutingdown....", SplashScreenStatus.Error);
                Thread.Sleep(1000);
                Exit();
            }

            crashTimeOut++;
            ReloadGame();
            Debug.Log("Crashed Trying Yo Start Back!");
            SplashScreen.SetState("Crashed Trying Yo Start Back!", SplashScreenStatus.Error);
            Thread.Sleep(1000);
        }

        /// <summary>
        /// This is gona disconnect/quit you from any world or server etc. and unload assets, and other things, this is for reload the game, like the firts time
        /// </summary>
        public void ReloadGame()
        {
            Network.Disconnect();//Disconnect you from any server you are in, or if you is the host closing the server

            if (OGame != null)
            {
                OGame.Dispose();
                OGame = null;
            }

            if (_physics != null)
            {
                _physics.Dispose();
                _physics = null;
            }

            /*if (_GUI != null)
            {
                _GUI.Dispose();
                _GUI = null;
            }*/

            if (_GUIRender != null)
            {
                _GUIRender.Dispose();
                _GUIRender = null;
            }

            _assetsManager.Dispose();
            _assetsManager = null;

            _soundSystem.Dispose();

            GameLoaded = false;
            EngineIsReady = false;
        }

        protected override void OnUnload(EventArgs e)
        {
            currentDomain.UnhandledException -= new UnhandledExceptionEventHandler(MyHandler);
            currentDomain = null;

            if (EngineIsReady)
            {
                //_Sky.OnDestroy();

                OGame.Dispose();
                _assetsManager.Dispose();
                _physics.Dispose();

                if (_GUIRender != null)
                {
                    _GUIRender.Dispose();
                    _GUIRender = null;
                }

                _soundSystem.Dispose();

                GameLoaded = false;
                EngineIsReady = false;
            }
            else
            {
                Debug.Log("Window is finished, but the game isnt, somthing is not right (:");
            }

            _SplashScreen.Dispose();
            base.OnUnload(e);
        }

        public static BeginMode GetGLBeginMode{ get { return Window.Instance.GLBeginMode; } }
    }

    public static class Time
    {
        public static float _DeltaTime;
        public static int _Time;
        public static double _DTime;
        public static int _Tick;

        public static int FPS;
        public static int UPS;
    }

    public static class MouseCursor
    {
        private static bool _isLocked;

        public static bool MouseLocked { get { return _isLocked; } }

        public static void LockCursor()
        {
            Window.Instance.CursorVisible = false;
            _isLocked = true;
        }

        public static void UnLockCursor()
        {
            _isLocked = false;
            Window.Instance.CursorVisible = true;
        }

        public static void CursorLockPosition()
        {
            if (_isLocked)
            {
                Mouse.SetPosition(Window.Instance.X + Window.Instance.Width / 2f, Window.Instance.Y + Window.Instance.Height / 2f);
            }
        }
    }
}

public enum CullType : byte
{
    Front, Back, Both
}