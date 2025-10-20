using OpenTK.Graphics.OpenGL;
using System.IO;
using System;

namespace Fnf.Framework.TrueType.Parsing
{
    /// <summary>
    /// The backbone for reading font data while acounting
    /// for little and big endian automaticaly
    /// </summary>
    public class FontReader : IDisposable
    {
        BinaryReader reader;
        Stream stream;

        public FontReader(string fontPath, bool lookInWindowsFonts)
        {
            // Make sure the file exists
            CheckFontExists(fontPath, lookInWindowsFonts);

            stream = File.Open(fontPath, FileMode.Open);
            reader = new BinaryReader(stream);
        }

        void CheckFontExists(string fontPath, bool lookInWindowsFonts)
        {
            // If exists do nothing
            if (File.Exists(fontPath)) return;

            // Try to copy it from windows fonts folder
            string pathAtWindowsFolder = "C:/Windows/Fonts/" + Path.GetFileName(fontPath);
            if (lookInWindowsFonts && File.Exists(pathAtWindowsFolder))
            {
                // Make sure the directory exists
                string fontDirectory = Path.GetDirectoryName(Path.GetFullPath(fontPath));
                if (!Directory.Exists(fontDirectory)) Directory.CreateDirectory(fontDirectory);

                // Copy the font
                File.Copy(pathAtWindowsFolder, fontPath, false);
            }
            else throw new FileNotFoundException($"Could not find the given '{fontPath}' font");
        }

        public void GoTo(long position) => stream.Position = position;
        public void Skip(int bytes) => stream.Position += bytes;
        public long GetPos() => stream.Position;

        public sbyte ReadSByte() => reader.ReadSByte();
        public byte ReadByte() => reader.ReadByte();
        public int ReadInt32() => (int)ReadUInt32();
        public int ReadInt16() => (short)ReadUInt16();
        public double ReadFixedPoint2Dot14() => ReadInt16() / (double)(1 << 14);

        public ushort ReadUInt16()
        {
            ushort value = reader.ReadUInt16();
            if (BitConverter.IsLittleEndian) value = (ushort)(value >> 8 | value << 8);
            return value;
        }

        public uint ReadUInt32()
        {
            uint value = reader.ReadUInt32();
            if (BitConverter.IsLittleEndian)
            {
                const byte ByteMask = 0b11111111;
                uint a = (value >> 24) & ByteMask;
                uint b = (value >> 16) & ByteMask;
                uint c = (value >> 8) & ByteMask;
                uint d = (value >> 0) & ByteMask;
                value = a | b << 8 | c << 16 | d << 24;
            }
            return value;
        }

        public void Dispose()
        {
            // Close the stream
            stream.Dispose();
            reader.Dispose();
        }
    }
}