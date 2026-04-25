using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoxelTK
{
    internal class Game : GameWindow
    {
        public int ScreenWidth, ScreenHeight;

        public Game(int Width, int Height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            ScreenWidth = Width;
            ScreenHeight = Height;

            CenterWindow(new Vector2i(Width, Height));
        }

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(1f, 1f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Context.SwapBuffers();

            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            this.ScreenWidth = e.Width;
            this.ScreenHeight = e.Height;
        }
    }
}
