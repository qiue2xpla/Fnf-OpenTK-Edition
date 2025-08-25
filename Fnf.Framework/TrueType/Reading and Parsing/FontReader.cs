using System.IO;
using System;

namespace Fnf.Framework.TrueType
{
    internal class FontReader : IDisposable
    {
        public long Position { get => stream.Position; set => stream.Position = value; }
        private BinaryReader reader;
        private Stream stream;

        public FontReader(string fontName)
        {
            string fileName = fontName + ".ttf";

            if (!File.Exists(fileName))
            {
                string mainName = "";
                for (int i = 0; i < fontName.Length; i++)
                {
                    mainName += fontName[i];
                    if (fontName[i] == '/')
                        mainName = "";
                }

                // Try to copy it from windows fonts folder
                string pathAtWinFonts = "C:/Windows/Fonts/" + mainName + ".ttf";
                if (File.Exists(pathAtWinFonts))
                {
                    if(!Directory.Exists(fileName))
                    {
                        Directory.CreateDirectory(fontName.Substring(0, fontName.Length - mainName.Length - 1));
                    }

                    File.Copy(pathAtWinFonts, fileName, false);
                }
                else throw new FileNotFoundException(fileName);
            }

            stream = File.Open(fileName, FileMode.Open);
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