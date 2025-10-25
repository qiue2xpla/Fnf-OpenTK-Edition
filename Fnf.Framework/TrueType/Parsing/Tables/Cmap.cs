using System.Collections.Generic;
using System.Collections;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Fnf.Framework.TrueType.Parsing.Tables
{
    /// <summary>
    /// This table is for mapping character to glyph index to locate glyphs
    /// </summary>
    public class Cmap
    {
        public readonly Dictionary<char, int> characterToGlyphIndex;

        public Cmap(FontParser parser)
        {
            var formats = GetFormats(parser.reader, parser.tableOffsets["cmap"]);
            
            if (formats.ContainsKey(4)) characterToGlyphIndex = GetFormat4Mapping(parser.reader, formats[4]);
            else if (formats.ContainsKey(40)) characterToGlyphIndex = GetFormat4Mapping(parser.reader, formats[40]);
            else if (formats.ContainsKey(10)) characterToGlyphIndex = GetFormat0Mapping(parser.reader, formats[10]);
            else if (characterToGlyphIndex == null) throw new Exception("Font doesn't contain a supported mapping format");
        }

        Dictionary<int, uint> GetFormats(FontReader reader, uint cmapOffset)
        {
            var formats = new Dictionary<int, uint>();
            reader.GoTo(cmapOffset);
            reader.Skip(sizeof(ushort)); // version (always 0)
            int numTables = reader.ReadUInt16();
            for (int i = 0; i < numTables; i++)
            {
                int platformID = reader.ReadUInt16();
                int platformSpecificID = reader.ReadUInt16();
                uint offset = reader.ReadUInt32(); // Offset from cmap start not file

                uint finalOffset = offset + cmapOffset;
                switch(platformID * 10 + platformSpecificID)
                {
                    case 31: formats.Add(4, finalOffset); break;
                    case 3: formats.Add(40, finalOffset); break;
                    case 10: formats.Add(0, finalOffset); break;
                }
            }
            return formats;
        }

        Dictionary<char, int> GetFormat0Mapping(FontReader reader, uint offset)
        {
            var result = new Dictionary<char, int>();

            reader.GoTo(offset);

            ushort format = reader.ReadUInt16();    // should be 0
            ushort length = reader.ReadUInt16();    // usually 262
            ushort language = reader.ReadUInt16();  // ignored

            // Format 0 has 256 one-byte character codes (0–255)
            for (int i = 0; i < 256; i++)
            {
                byte glyphId = reader.ReadByte();
                if (glyphId != 0) result.Add((char)i, glyphId);
            }

            // Add .notdef fallback
            if (!result.ContainsKey((char)0xFFFF))
                result[(char)0xFFFF] = 0;

            return result;
        }

        Dictionary<char, int> GetFormat4Mapping(FontReader reader, uint offset)
        {
            var result = new Dictionary<char, int>();

            reader.GoTo(offset);

            reader.Skip(sizeof(ushort)); // format (set to 4)
            reader.Skip(sizeof(ushort)); // length
            reader.Skip(sizeof(ushort)); // language
            int segCount = reader.ReadUInt16() / 2;
            reader.Skip(sizeof(ushort)); // searchRange
            reader.Skip(sizeof(ushort)); // entrySelector
            reader.Skip(sizeof(ushort)); // rangeShift
            ushort[] endCount = ReadUInt16Array(reader, segCount);
            reader.Skip(sizeof(ushort)); // reservedPad (always 0)
            ushort[] startCount = ReadUInt16Array(reader, segCount);
            ushort[] idDelta = ReadUInt16Array(reader, segCount);
            int idRangeStart = (int)reader.GetPos();
            ushort[] idRangeOffset = ReadUInt16Array(reader, segCount);

            for (int seg = 0; seg < segCount; seg++)
            {
                for (int code = startCount[seg]; code <= endCount[seg]; code++)
                {
                    int glyphIndex = 0;

                    if (idRangeOffset[seg] == 0)
                    {
                        glyphIndex = (code + idDelta[seg]) % 65536;
                    }
                    else
                    {
                        reader.GoTo(idRangeOffset[seg] + 2 * (code - startCount[seg]) + idRangeStart + seg * 2);
                        glyphIndex = reader.ReadUInt16();

                        // If code is 0 that means there is no glyph
                        if (glyphIndex != 0)
                        {
                            glyphIndex = (glyphIndex + idDelta[seg]) % 65536;
                        }
                    }

                    if (glyphIndex < 0) glyphIndex += 65536;
                    if (glyphIndex != 0) result.Add((char)code, glyphIndex);
                }
            }

            if (result.ContainsKey((char)ushort.MaxValue))
            {
                if (result[(char)ushort.MaxValue] != 0)
                {
                    result.Remove((char)0xFFFF);
                    result.Add((char)0xFFFF, 0);
                }
            }
            else
            {
                result.Add((char)ushort.MaxValue, 0);
            }

            return result;
        }

        ushort[] ReadUInt16Array(FontReader reader, int length)
        {
            ushort[] array = new ushort[length];
            for (int i = 0; i < length; i++) array[i] = reader.ReadUInt16();
            return array;
        }
    }
}