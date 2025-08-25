using System.Collections.Generic;

namespace Fnf.Framework.TrueType
{
    internal interface IMappingFormat
    {
        Dictionary<char, uint> UnicodeToGlyphIndex { get; }
        char MissingCharacter { get; }
    }
}