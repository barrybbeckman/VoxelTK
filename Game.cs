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
        float[] verticies =
        {
            0f, 0.5f, 0f, // top vertex
            -0.5f, -0.5f, 0f, // bottom left vertex
            0.5f, -0.5f, 0f // bottom right vertex
        };

        // Render Pipeline Vars
        int VAO;
        int ShaderProgram;

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

            VAO = GL.GenVertexArray();

            int VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, verticies.Length * sizeof(float), verticies, BufferUsageHint.StaticDraw);

            // Bind VAO

            GL.BindVertexArray(VAO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(VAO, 0);


            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // unbinds VBO
            GL.BindVertexArray(0);

            // Create ShaderProgram
            ShaderProgram = GL.CreateProgram();

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, LoadShaderSource("Default.vert"));
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, LoadShaderSource("Default.frag"));
            GL.CompileShader(fragmentShader);

            GL.AttachShader(ShaderProgram, vertexShader);
            GL.AttachShader(ShaderProgram, fragmentShader);

            GL.LinkProgram(ShaderProgram);

            // Delete Shaders

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);


        }

        protected override void OnUnload()
        {
            base.OnUnload();

            GL.DeleteVertexArray(VAO);
            GL.DeleteProgram(ShaderProgram);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(0.6f, 0.0f, 1.0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Draw Triangle
            GL.UseProgram(ShaderProgram);
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

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

        public static string LoadShaderSource(string filePath)
        {
            string shaderSource = "";

            try
            {
                using (StreamReader reader = new StreamReader("../../../Shaders/" + filePath))
                {
                    shaderSource = reader.ReadToEnd();
                }
                Console.WriteLine(shaderSource);
            }
            catch (Exception e)
            {
                Console.WriteLine("failed to load shader source file: " + e.Message);
            }

            return shaderSource;
        }
    }
}
