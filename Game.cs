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
        public int ScreenWidth;
        public int ScreenHeight;

        public Game(int Width, int Height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.CenterWindow(new Vector2i(Width, Height));
            ScreenWidth = Width;
            ScreenHeight = Height;
        }
    }
}
