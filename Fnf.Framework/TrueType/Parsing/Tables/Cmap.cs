using System.Collections.Generic;
using System.Collections;
using System;

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
            if (formats.ContainsKey((0, 3))) characterToGlyphIndex = GetFormat4Mapping(parser.reader, formats[(0, 3)]);
            else if (formats.ContainsKey((1, 0))) characterToGlyphIndex = GetFormat0Mapping(parser.reader, formats[(1, 0)]);
            else if (characterToGlyphIndex == null) throw new Exception("Font doesn't contain a supported mapping format");
        }

        Dictionary<(int platID, int encodingID), uint> GetFormats(FontReader reader, uint cmapOffset)
        {
            var formats = new Dictionary<(int, int), uint>();
            reader.GoTo(cmapOffset);
            reader.Skip(sizeof(ushort)); // version (always 0)
            int numTables = reader.ReadUInt16();
            for (int i = 0; i < numTables; i++)
            {
                int platformID = reader.ReadUInt16();
                int platformSpecificID = reader.ReadUInt16();
                uint offset = reader.ReadUInt32(); // Offset from cmap start not file
                formats.Add((platformID, platformSpecificID), offset + cmapOffset);
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
                        glyphIndex = (code + idDelta[seg]) % 0xFFFF;
                    }
                    else
                    {
                        reader.GoTo(idRangeOffset[seg] + 2 * (code - startCount[seg]) + idRangeStart);
                        glyphIndex = reader.ReadUInt16();

                        // If code is 0 that means there is no glyph
                        if (glyphIndex != 0)
                        {
                            glyphIndex = (glyphIndex + idDelta[seg]) % 0xFFFF;
                        }
                    }

                    if(glyphIndex != 0)
                    {
                        result.Add((char)code, glyphIndex);
                    }
                }
            }

            if (result.ContainsKey((char)0xFFFF))
            {
                if (result[(char)0xFFFF] != 0)
                {
                    result.Remove((char)0xFFFF);
                    result.Add((char)0xFFFF, 0);
                }
            }
            else
            {
                result.Add((char)0xFFFF, 0);
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