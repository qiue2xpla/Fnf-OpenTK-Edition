using System.Collections.Generic;

namespace Fnf.Framework.TrueType
{
    internal class Format4 : IMappingFormat
    {
        public Dictionary<char, uint> UnicodeToGlyphIndex { get; } = new Dictionary<char, uint>();
        public char MissingCharacter { get; }

        public Format4(uint offset, FontReader reader)
        {
            reader.Position = offset + sizeof(ushort) * 3; //Format(Set to 4), Length, Language
            int SegCount = reader.ReadUInt16() / 2;
            reader.Position += sizeof(ushort) * 3; //searchRange, entrySelector, rangeShift
            long PreviousPosition = reader.Position;
            ushort[] endCode_array = reader.ReadUInt16Array(SegCount);
            reader.Position += 2; //reservedPad(zero)
            ushort[] startCode_array = reader.ReadUInt16Array(SegCount);
            ushort[] idDelta_array = reader.ReadUInt16Array(SegCount);
            long idrangestart = reader.Position;
            ushort[] idRangeOffset_array = reader.ReadUInt16Array(SegCount);
            bool hasReadMissingCharGlyph = false;

            for (int i = 0; i < SegCount; i++)
            {
                int Start = startCode_array[i];
                int Current = startCode_array[i];
                int End = endCode_array[i];
                if (Current == 65535) break;

                while (Current <= End)
                {
                    int glyphIndex = 0;

                    if (idRangeOffset_array[i] == 0)
                    {
                        glyphIndex = (Current + idDelta_array[i]) % 65536;
                    }
                    else
                    {
                        reader.Position = idRangeOffset_array[i] + 2 * (Current - Start) + (int)idrangestart;
                        glyphIndex = reader.ReadUInt16();

                        if (glyphIndex != 0)
                        {
                            glyphIndex = (glyphIndex + idDelta_array[i]) % 65536; // & 0xffff is modulo 65536.
                        }
                    }

                    UnicodeToGlyphIndex.Add((char)Current, (uint)glyphIndex);
                    if (glyphIndex == 0)
                    {
                        hasReadMissingCharGlyph = true;
                        MissingCharacter = (char)Current;
                    }
                    Current++;
                }
            }

            if (!hasReadMissingCharGlyph)
            {
                UnicodeToGlyphIndex.Add((char)65535, 0);
                MissingCharacter = (char)65535;
            }

            return;
        }
    }
}