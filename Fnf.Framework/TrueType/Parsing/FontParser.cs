using Fnf.Framework.TrueType.Parsing.Tables;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Fnf.Framework.TrueType.Parsing
{
    /// <summary>
    /// Removes the need to work with font file structure
    /// </summary>
    public class FontParser : IDisposable
    {
        public Dictionary<string, uint> tableOffsets;
        public FontReader reader;

        public FontParser(FontReader reader)
        {
            this.reader = reader;
            tableOffsets = GetTableOffsets();
        }

        /// <summary>
        /// Returns the table offsets of the font
        /// </summary>
        public Dictionary<string, uint> GetTableOffsets()
        {
            var result = new Dictionary<string, uint>();

            // Go to the begining of the file
            reader.GoTo(0);

            // Load the necessary data
            reader.Skip(sizeof(uint)); // sfntVersion
            int tableCount = reader.ReadUInt16();
            reader.Skip(sizeof(ushort)); // searchRange
            reader.Skip(sizeof(ushort)); // entrySelector
            reader.Skip(sizeof(ushort)); // rangeShift

            for (int i = 0; i < tableCount; i++)
            {
                string tag = ReadTag();
                reader.Skip(sizeof(uint)); // Checksum
                uint offset = reader.ReadUInt32();
                reader.Skip(sizeof(uint)); // Length

                result.Add(tag, offset);
            }
            return result;

            /// <summary>
            /// Reads 4 bytes and converts them to a 4 character string
            /// </summary>
            string ReadTag()
            {
                string tag = "";
                for (int i = 0; i < 4; i++) tag += (char)reader.ReadByte();
                return tag;
            }
        }

        public Cmap GetCharacterMapping()
        {
            return new Cmap(this);
        }

        public Glyf GetGlyphTable(Cmap cmap, Loca loca)
        {
            return new Glyf(this, cmap, loca);
        }

        public Head GetHeaderTable()
        {
            return new Head(this);
        }

        public Hhea GetHorizontalHeaderTable()
        {
            return new Hhea(this);
        }

        public Hmtx GetHorizontalMetricsTable(Hhea hhea, Maxp maxp)
        {
            return new Hmtx(this, hhea, maxp);
        }

        public Loca GetLocationTable(Maxp maxp, Head head)
        {
            return new Loca(this, maxp, head);
        }

        public Maxp GetMaximumProfile()
        {
            return new Maxp(this);
        }



        public Name GetNameTable()
        {
            return new Name(this);
        }

        


        

        

        

        

        

        

        
        
        
        
        public void Dispose()
        {
            reader = null;
        }
    }
}