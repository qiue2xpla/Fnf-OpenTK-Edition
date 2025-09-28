using OpenTK.Graphics.OpenGL;
using System.IO;
using System;
using System.Text;
using System.Xml;

namespace Fnf.Framework.Graphics
{
    public static class Shader
    {
        public static int GenerateShaderFromResource(string name)
        {
            return GenerateShaderFromSource(Encoding.UTF8.GetString(ResourcesAccesser.GetResource<byte[]>(name)).TrimStart('\uFEFF') + "\r\n");
        }

        public static int GenerateShaderFromFolder(string name)
        {
            return GenerateShaderFromSource(File.ReadAllText($"{name}/shader.glsl", Encoding.UTF8));
        }

        public static int GenerateShaderFromSource(string shaderSource)
        {
            var sections = StringUtility.SplitIntoWholeSections(shaderSource.Replace('\r', ' ').Split('\n'));

            string vert = null, frag = null;

            foreach (var section in sections)
            {
                switch(section.Section)
                {
                    case "VertexShader": vert = section.Data; break;
                    case "FragmentShader": frag = section.Data; break;
                    default: throw new InvalidDataException($"Section '{section.Section}' is not a valid shader section");
                }
            }

            return GenerateShaderFromSource(vert, frag);
        }

        public static int GenerateShaderFromSource(string vertSource, string fragSource)
        {
            int VertexShader = GL.CreateShader(ShaderType.VertexShader);
            int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(VertexShader, vertSource);
            GL.ShaderSource(FragmentShader, fragSource);

            GL.CompileShader(VertexShader);
            GL.CompileShader(FragmentShader);
            Check(VertexShader);
            Check(FragmentShader);

            int id = GL.CreateProgram();
            GL.AttachShader(id, VertexShader);
            GL.AttachShader(id, FragmentShader);
            GL.LinkProgram(id);

            GL.GetProgram(id, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0) Console.WriteLine(GL.GetProgramInfoLog(id));

            GL.DetachShader(id, VertexShader);
            GL.DetachShader(id, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);

            GL.LinkProgram(0);

            GL.UseProgram(0);

            return id;

            void Check(int shaderId)
            {
                GL.GetShader(shaderId, ShaderParameter.CompileStatus, out int result);
                if (result == 0) Console.WriteLine(GL.GetShaderInfoLog(FragmentShader));
            }
        }

        public static void Use(int id)
        {
            GL.UseProgram(id);
        }

        public static void DeleteShader(int id)
        {
            GL.DeleteProgram(id);
        }

        public static void Uniform1(int id, string name, int i1)
        {
            GL.Uniform1(GL.GetUniformLocation(id, name), i1);
        }

        public static void Uniform1(int id, string name, float f1)
        {
            GL.Uniform1(GL.GetUniformLocation(id, name), f1);
        }

        public static void Uniform2(int id, string name, Vector2 vector2)
        {
            GL.Uniform2(GL.GetUniformLocation(id, name), vector2.x, vector2.y);
        }

        public static void Uniform3(int id, string name, float a, float b, float c)
        {
            GL.Uniform3(GL.GetUniformLocation(id, name), a, b, c);
        }

        public static void Uniform4(int id, string name, float a, float b, float c, float d)
        {
            GL.Uniform4(GL.GetUniformLocation(id, name), a, b, c, d);
        }

        public static void UniformMat(int id, string name, Matrix3 mat)
        {
            GL.UniformMatrix3(GL.GetUniformLocation(id, name), 1, false, mat.ToColumnMajorFloatArray());
        }
    }
}