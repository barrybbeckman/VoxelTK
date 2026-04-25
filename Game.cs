using StbImageSharp;
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
            -0.5f, 0.5f, 0f, // Top Left 
            0.5f, 0.5f, 0f, // Top Right 
            -0.5f, -0.5f, 0f, // Bottom Left
            0.5f, -0.5f, 0f // Bottom Right
        };

        float[] texCoords =
        {
            0f, 1f,
            1f, 1f,
            0f, 0f,
            1f, 0f
        };

        uint[] indices = 
        { 
            // Top Triangle
            0, 1, 2,
            // Bottom Triangle
            3, 2, 1
        };

        // Render Pipeline Vars
        int VAO;
        int ShaderProgram;
        int VBO;
        int TextureVBO;
        int EBO;
        int TextureId;

        // Transformation Variables
        float yRot = 0f;

        public int Width, Height;

        public Game(int Width, int Height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.Width = Width;
            this.Height = Height;

            CenterWindow(new Vector2i(Width, Height));
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Generate and bind VAO
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            // Position VBO
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, verticies.Length * sizeof(float), verticies, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(VAO, 0);

            // Texture Coord VBO
            TextureVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, TextureVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Length * sizeof(float), texCoords, BufferUsageHint.StaticDraw);

            // Set attrib pointer for tex coords (location = 1) while texture VBO is bound
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(VAO, 1);

            // EBO must be bound INSIDE the VAO bind so the VAO remembers it
            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            // Create ShaderProgram
            ShaderProgram = GL.CreateProgram();

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, LoadShaderSource("Default.vert"));
            GL.CompileShader(vertexShader);
            Console.WriteLine(GL.GetShaderInfoLog(vertexShader));

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, LoadShaderSource("Default.frag"));
            GL.CompileShader(fragmentShader);
            Console.WriteLine(GL.GetShaderInfoLog(fragmentShader)); // Fix: was GetProgramInfoLog

            GL.AttachShader(ShaderProgram, vertexShader);
            GL.AttachShader(ShaderProgram, fragmentShader);

            GL.LinkProgram(ShaderProgram);
            Console.WriteLine(GL.GetProgramInfoLog(ShaderProgram));

            // Delete Shaders after linking
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            // Textures
            TextureId = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);

            // Texture Parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // Load Grass Texture
            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult GrassTexture = ImageResult.FromStream(File.OpenRead("../../../Textures/block/grass_block_side.png"), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, GrassTexture.Width, GrassTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, GrassTexture.Data);

            // Unbind Texture
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            GL.DeleteVertexArray(VAO);
            GL.DeleteBuffer(VBO); // Fix: was DeleteVertexArray(VBO)
            GL.DeleteBuffer(TextureVBO);
            GL.DeleteBuffer(EBO);
            GL.DeleteTexture(TextureId);
            GL.DeleteProgram(ShaderProgram);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(0.6f, 0.0f, 1.0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Bind texture before drawing
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);

            // Draw Triangle
            GL.UseProgram(ShaderProgram);
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            // Transform Matrices
            Matrix4 model = Matrix4.Identity;
            Matrix4 view = Matrix4.Identity;
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), Width / Height, 0.1f, 100.0f);


            model = Matrix4.CreateRotationY(yRot);
            yRot += 0.002f;

            
            Matrix4 translation = Matrix4.CreateTranslation(0f, 0f, -3f);

            model *= translation;

            int modelLocation = GL.GetUniformLocation(ShaderProgram, "model");
            int viewLocation = GL.GetUniformLocation(ShaderProgram, "view");
            int projectionLocation = GL.GetUniformLocation(ShaderProgram, "projection");

            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

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
            this.Width = e.Width;
            this.Height = e.Height;
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