using System.IO;
using System;

namespace Fnf.Framework.TrueType
{
    internal class FontReader : IDisposable
    {
        public long Position { get => stream.Position; set => stream.Position = value; }
        BinaryReader reader;
        Stream stream;

        public FontReader(string font)
        {
            string fontPath = font + ".ttf";

            // If the font doesn't exist then check the windows folder
            if (!File.Exists(fontPath))
            {
                char[] seperators = new char[] { '/', '\\' };
                
                string fontFileName = "";
                for (int i = 0; i < fontPath.Length; i++)
                {
                    fontFileName += fontPath[i];
                    for (int s = 0; s < seperators.Length; s++)
                    {
                        if (fontPath[i] == seperators[s])
                        {
                            fontFileName = "";
                            break;
                        }
                    }
                }

                // Try to copy it from windows fonts folder
                string pathAtWindowsFolder = "C:/Windows/Fonts/" + fontFileName;
                if (File.Exists(pathAtWindowsFolder))
                {
                    if(!Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(fontPath))))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(fontPath)));
                    }

                    File.Copy(pathAtWindowsFolder, fontPath, false);
                }
                else throw new FileNotFoundException($"Could not find the given '{fontPath}' in the given path or in system fonts");
            }

            stream = File.Open(fontPath, FileMode.Open);
            reader = new BinaryReader(stream);
        }

        public void Dispose()
        {
            stream.Dispose();
            reader.Dispose();
        }

        #region Reading

        public string ReadTag()
        {
            string tag = "";

            for (int i = 0; i < 4; i++)
            {
                tag += (char)reader.ReadByte();
            }

            return tag;
        }

        public double ReadFixedPoint2Dot14()
        {
            // No idea whats going on here
            return (short)ReadUInt16() / (double)(1 << 14);
        }

        public byte ReadByte() => reader.ReadByte();
        public int ReadInt32() => (int)ReadUInt32();
        public int ReadInt16() => (short)ReadUInt16();

        public ushort ReadUInt16()
        {
            ushort value = reader.ReadUInt16();

            if (BitConverter.IsLittleEndian)
            {
                // Convert data to little endian
                // TTF files are big endian through out
                value = (ushort)(value >> 8 | value << 8);
            }

            return value;
        }

        public uint ReadUInt32()
        {
            uint value = reader.ReadUInt32();

            if (BitConverter.IsLittleEndian)
            {
                // Convert data to little endian
                // TTF files are big endian through out
                const byte ByteMask = 0b11111111;
                uint a = (value >> 24) & ByteMask;
                uint b = (value >> 16) & ByteMask;
                uint c = (value >> 8) & ByteMask;
                uint d = (value >> 0) & ByteMask;
                value = a | b << 8 | c << 16 | d << 24;
            }

            return value;
        }
        
        public ushort[] ReadUInt16Array(int length)
        {
            ushort[] array = new ushort[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadUInt16();
            }
            return array;
        }

        public int[] ReadUInt16ArrayAsInts(int length)
        {
            int[] array = new int[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadUInt16();
            }
            return array;
        }

        #endregion
    }
}