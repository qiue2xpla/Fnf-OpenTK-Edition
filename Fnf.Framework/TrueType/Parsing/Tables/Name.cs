using System.Text;
using System;

namespace Fnf.Framework.TrueType.Parsing.Tables
{
    /// <summary>
    /// Contains all the strings like the font family name, style, version, copyright and so on. If a value
    /// is null then the font doesn't contain the value
    /// </summary>
    public class Name
    {
        public string copyright;
        public string fontFamily;
        public string fontSubFamily;
        public string uniqueFontIdentifier;
        public string fullFontName;
        public string version;
        public string postScript;
        public string trademark;
        public string manufacturer;
        public string designer;
        public string description;
        public string vendorURL;
        public string designerURL;
        public string licenseDescription;
        public string licenseInfoURL;
        public string typographicFamily;
        public string typographicSubFamily;
        public string typographicSamples;
        public string openTypeExtentions;

        public Name(FontParser parser)
        {
            var r = parser.reader;
            var o = parser.tableOffsets["name"];

            r.GoTo(o);
            r.Skip(2); // format
            r.Skip(2); // count
            int stringOffset = r.ReadUInt16(); // Offset (from start of the name table) to the beginning of the string storage area

            SubString[] subStrings = ReadSubStrings(parser);

            for (int i = 0; i < subStrings.Length; i++)
            {
                r.GoTo(o + stringOffset + subStrings[i].offset);

                byte[] bytes = new byte[subStrings[i].length];
                for (int t = 0; t < bytes.Length; t++) bytes[t] = r.ReadByte();

                string encodedString = DecodeString(bytes, subStrings[i].platformID, subStrings[i].encodingID);

                SetValue(encodedString, subStrings[i].nameID);
            }
        }

        void SetValue(string text, int nameID)
        {
            switch (nameID)
            {
                case 0: copyright = text; break;
                case 1: fontFamily = text; break;
                case 2: fontSubFamily = text; break;
                case 3: uniqueFontIdentifier = text; break;
                case 4: fullFontName = text; break;
                case 5: version = text; break;
                case 6: postScript = text; break;
                case 7: trademark = text; break;
                case 8: manufacturer = text; break;
                case 9: designer = text; break;
                case 10: description = text; break;
                case 11: vendorURL = text; break;
                case 12: designerURL = text; break;
                case 13: licenseDescription = text; break;
                case 14: licenseInfoURL = text; break;
                case 16: typographicFamily = text; break;
                case 17: typographicSubFamily = text; break;
                case 18: typographicSamples = text; break;
                case 19: openTypeExtentions = text; break;
                default: throw new NotImplementedException($"Name id '{nameID}' is not a valid value");
            }
        }

        string DecodeString(byte[] bytes, int platformID, int encodingID)
        {
            switch (platformID)
            {
                case 0: // Unicode
                    switch (encodingID)
                    {
                        case 0: case 1: case 2:  case 3: case 4: return Encoding.BigEndianUnicode.GetString(bytes); // Always big-endian UTF-16
                        default: throw new NotImplementedException($"Encoding id '{encodingID}' is not a valid value");
                    }
                case 1: // Macintosh
                    switch (encodingID)
                    {
                        case 0: return Encoding.GetEncoding("macintosh").GetString(bytes);
                        case 1: return Encoding.GetEncoding("x-mac-japanese").GetString(bytes);
                        case 2: return Encoding.GetEncoding("x-mac-chinesetrad").GetString(bytes);
                        case 3: return Encoding.GetEncoding("x-mac-korean").GetString(bytes);
                        case 4: return Encoding.GetEncoding("x-mac-arabic").GetString(bytes);
                        case 5: return Encoding.GetEncoding("x-mac-hebrew").GetString(bytes);
                        case 6: return Encoding.GetEncoding("x-mac-greek").GetString(bytes);
                        case 7: return Encoding.GetEncoding("x-mac-cyrillic").GetString(bytes);
                        case 8: return Encoding.GetEncoding("mac-ukrainian").GetString(bytes);
                        case 9: return Encoding.GetEncoding("x-mac-thai").GetString(bytes);
                        case 10: return Encoding.GetEncoding("x-mac-chinesesimp").GetString(bytes);
                        case 11: return Encoding.GetEncoding("x-mac-romanian").GetString(bytes);
                        case 12: return Encoding.GetEncoding("x-mac-icelandic").GetString(bytes);
                        case 13: return Encoding.GetEncoding("x-mac-turkish").GetString(bytes);
                        case 14: return Encoding.GetEncoding("x-mac-croatian").GetString(bytes);
                        default: throw new NotImplementedException($"Encoding id '{encodingID}' is not a valid value");
                    }
                case 2: // ISO (deprecated) and ignored
                    return null;
                case 3: // Windows
                    switch (encodingID)
                    {
                        case 0: // Symbol (rare)
                            string result = "";
                            for (int c = 0; c < bytes.Length; c += 2)
                            {
                                ushort code = (ushort)((bytes[c] << 8) | bytes[c + 1]);
                                char _char = (char)(0xF000 + (code & 0x00FF)); // map low byte to PUA
                                result += _char;
                            }
                            return result;
                        case 1: // UTF-16BE
                            return Encoding.BigEndianUnicode.GetString(bytes);
                        case 10: // UTF-32BE
                            return Encoding.GetEncoding("utf-32BE").GetString(bytes);
                        default: throw new NotImplementedException($"Encoding id '{encodingID}' is not a valid value");
                    }
            }

            return null;
        }

        SubString[] ReadSubStrings(FontParser parser)
        {
            var r = parser.reader;
            var o = parser.tableOffsets["name"];

            r.GoTo(o);

            r.Skip(2); // format
            int count = r.ReadUInt16();
            r.Skip(2); // stringOffset

            SubString[] subStrings = new SubString[count];
            for (int i = 0; i < count; i++)
            {
                subStrings[i] = new SubString()
                {
                    platformID = r.ReadUInt16(),
                    encodingID = r.ReadUInt16(),
                    languageID = r.ReadUInt16(),
                    nameID = r.ReadUInt16(),
                    length = r.ReadUInt16(), // In bytes
                    offset = r.ReadUInt16()  // Offset from stringOffset to the start of this string
                };
            }
            return subStrings;
        }

        struct SubString
        {
            public int platformID;
            public int encodingID;
            public int languageID;
            public int nameID;
            public int length;
            public int offset;
        }
    }
}