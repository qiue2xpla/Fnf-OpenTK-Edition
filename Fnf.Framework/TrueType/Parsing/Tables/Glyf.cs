using System.Collections.Generic;
using System.Linq;
using System;

namespace Fnf.Framework.TrueType.Parsing.Tables
{
    /// <summary>
    /// Contains glyph outlines
    /// </summary>
    public class Glyf
    {
        public Dictionary<char, GlyphData> glyphs;

        public Glyf(FontParser parser, Cmap cmap, Loca loca)
        {
            glyphs = new Dictionary<char, GlyphData>();

            parser.reader.GoTo(parser.tableOffsets["glyf"]);

            foreach (var pair in cmap.characterToGlyphIndex)
            {
                char character = pair.Key;
                int glyphIndex = pair.Value;

                if (glyphIndex >= loca.glyphLocations.Length - 1)
                {
                    continue; // Skip
                }

                if (loca.glyphLocations[glyphIndex + 1] - loca.glyphLocations[glyphIndex] == 0)
                {
                    // If the data length is 0 then the glyph has no outlines
                    // https://www.fast-report.com/blogs/font-truetype-from-inside

                    // Add an empty glyph
                    glyphs.Add(character, new GlyphData()
                    {
                        countourEndPoints = new int[0],
                        points = new GlyphPoint[0],
                        glyphIndex = glyphIndex
                    });

                    continue;
                }

                if(character == 0xFFFF)
                {
                    glyphIndex = 0;
                }

                glyphs.Add(character, ReadGlyph(parser, loca, glyphIndex));
            }

            if(!glyphs.ContainsKey((char)0xFFFF))
            {
                glyphs.Add((char)0xFFFF, ReadGlyph(parser, loca, 0));
            }
        }

        public Dictionary<char, Glyph> GetGlyphs(Head head, Hmtx hmtx)
        {
            var newGlyphs = new Dictionary<char, Glyph>();
            foreach (var pair in glyphs)
            {
                newGlyphs.Add(pair.Key, new Glyph(pair.Value, head, hmtx));
            }

            return newGlyphs;
        }

        GlyphData ReadGlyph(FontParser parser, Loca loca, int glyphIndex)
        {
            var reader = parser.reader;
            var glyfOffset = parser.tableOffsets["glyf"];

            GlyphData glyph = new GlyphData();

            if(glyphIndex >= loca.glyphLocations.Length)
            {
                // TODO: Add logger
                glyphIndex = 0;
            }

            glyph.glyphIndex = glyphIndex;
            reader.GoTo(glyfOffset + loca.glyphLocations[glyphIndex]);
            // Common header
            glyph.numberOfContours = reader.ReadInt16();
            glyph.xMin = reader.ReadInt16();
            glyph.yMin = reader.ReadInt16();
            glyph.xMax = reader.ReadInt16();
            glyph.yMax = reader.ReadInt16();

            if (glyph.numberOfContours >= 0)
            {
                // Read a simple glyph

                // Read the contours endPoint
                glyph.countourEndPoints = ReadUInt16Array(reader, glyph.numberOfContours);

                // Get rid of the instructions
                reader.Skip(reader.ReadUInt16());

                // Get points count
                int pointsCount = 0;
                for (int i = 0; i < glyph.countourEndPoints.Length; i++) pointsCount = Math.Max(pointsCount, glyph.countourEndPoints[i] + 1);

                // Read flags
                byte[] flags = ReadFlags(reader, pointsCount);
                
                // Read points
                glyph.points = ReadPoints(reader, pointsCount, flags);
            }
            else
            {
                // Read a compound glyph

                // Read glyph components
                GlyphComponent[] components = ReadComponents(reader);

                // Add glyph components
                List<int> allContourEndIndices = new List<int>();
                List<GlyphPoint> allPoints = new List<GlyphPoint>();

                foreach (var component in components)
                {
                    //if (component.GlyphIndex >= loca.offset.Length) continue;

                    // The components can be simple and composite glyphs
                    GlyphData componentGlyph = ReadGlyph(parser, loca, component.GlyphIndex);

                    // Add endPoints
                    foreach (ushort endIndex in componentGlyph.countourEndPoints)
                        allContourEndIndices.Add(endIndex + allPoints.Count);

                    foreach (var point in componentGlyph.points)
                    {
                        GlyphPoint newPoint = Transform(point, component);

                        if (!FlagBitIsSet(component.Flags, 1)) //if not xy values
                        {
                            // (des - src)
                            if (inRange(component.DestPointIndex, allPoints.Count) && inRange(component.SrcPointIndex, componentGlyph.points.Length))
                            {
                                var src = Transform(componentGlyph.points[component.SrcPointIndex], component);
                                double dx = allPoints[component.DestPointIndex].x - src.x;
                                double dy = allPoints[component.DestPointIndex].y - src.y;
                                newPoint.x += (int)dx;
                                newPoint.y += (int)dy;
                            }
                        }
                        allPoints.Add(newPoint);
                    }

                    bool inRange(int x, int max)
                    {
                        return x >= 0 && x < max;
                    }

                    GlyphPoint Transform(GlyphPoint point, GlyphComponent comp)
                    {

                        int newX = (int)(comp.A * point.x + comp.C * point.y + comp.E);
                        int newY = (int)(comp.B * point.x + comp.D * point.y + comp.F);
                        (point.x, point.y) = (newX, newY);  
                        return point;
                    }
                }

                glyph.points = allPoints.ToArray();
                glyph.countourEndPoints = allContourEndIndices.ToArray();
            }

            return glyph;
        }

        GlyphComponent[] ReadComponents(FontReader reader)
        {
            var components = new List<GlyphComponent>();
            bool hasMoreComponents = true;
            while (hasMoreComponents)
            {
                GlyphComponent component = new GlyphComponent() { A = 1, D = 1 };

                component.Flags = reader.ReadUInt16();
                component.GlyphIndex = reader.ReadUInt16();

                // Decode flags
                bool argsAre2Bytes = FlagBitIsSet(component.Flags, 0);
                bool argsAreXYValues = FlagBitIsSet(component.Flags, 1);
                bool roundXYToGrid = FlagBitIsSet(component.Flags, 2);
                bool isSingleScaleValue = FlagBitIsSet(component.Flags, 3);
                // 4 is reserved
                hasMoreComponents = FlagBitIsSet(component.Flags, 5);
                bool isXAndYScale = FlagBitIsSet(component.Flags, 6);
                bool is2x2Matrix = FlagBitIsSet(component.Flags, 7);
                bool hasInstructions = FlagBitIsSet(component.Flags, 8);
                bool useThisComponentMetrics = FlagBitIsSet(component.Flags, 9);
                bool componentsOverlap = FlagBitIsSet(component.Flags, 10);
                // Remaining bits are reserved

                if (argsAre2Bytes)
                {
                    component.Argument1 = reader.ReadInt16();
                    component.Argument2 = reader.ReadInt16();
                }
                else
                {
                    component.Argument1 = reader.ReadSByte();
                    component.Argument2 = reader.ReadSByte();
                }

                if (argsAreXYValues)
                {
                    component.E = component.Argument1;
                    component.F = component.Argument2;
                }
                else
                {
                    // Args are point indicies
                    // This is rare in modern fonts
                    // TODO: Understand this part later
                    component.DestPointIndex = component.Argument1;
                    component.SrcPointIndex = component.Argument2;
                }

                if (isSingleScaleValue)
                {
                    component.A = reader.ReadFixedPoint2Dot14();
                    component.D = component.A;
                }
                else if (isXAndYScale)
                {
                    component.A = reader.ReadFixedPoint2Dot14();
                    component.D = reader.ReadFixedPoint2Dot14();
                }
                else if (is2x2Matrix)
                {
                    component.A = reader.ReadFixedPoint2Dot14();
                    component.B = reader.ReadFixedPoint2Dot14();
                    component.C = reader.ReadFixedPoint2Dot14();
                    component.D = reader.ReadFixedPoint2Dot14();
                }

                // Skip instructions
                if (hasInstructions) reader.Skip(reader.ReadUInt16());

                components.Add(component);
            }
            return components.ToArray();
        }

        bool FlagBitIsSet(uint flag, int bitIndex) => ((flag >> bitIndex) & 1) == 1;

        GlyphPoint[] ReadPoints(FontReader reader, int pointsCount, byte[] flags)
        {
            GlyphPoint[] points = new GlyphPoint[pointsCount];
            for (int i = 0; i < pointsCount; i++)
            {
                points[i].x = points[Math.Max(0, i - 1)].x;
                points[i].onCurve = IsFlagSet(flags[i], OutlineFlags.OnCurve);

                if (IsFlagSet(flags[i], OutlineFlags.XIsByte))
                {
                    points[i].x += IsFlagSet(flags[i], OutlineFlags.XDelta) ? reader.ReadByte() : -reader.ReadByte();
                }
                else if (!IsFlagSet(flags[i], OutlineFlags.XDelta))
                {
                    points[i].x += reader.ReadInt16();
                }
            }
            for (int i = 0; i < pointsCount; i++)
            {
                points[i].y = points[Math.Max(0, i - 1)].y;

                if (IsFlagSet(flags[i], OutlineFlags.YIsByte))
                {
                    points[i].y += IsFlagSet(flags[i], OutlineFlags.YDelta) ? reader.ReadByte() : -reader.ReadByte();
                }
                else if (!IsFlagSet(flags[i], OutlineFlags.YDelta))
                {
                    points[i].y += reader.ReadInt16();
                }
            }
            return points;
        }

        byte[] ReadFlags(FontReader reader, int pointsCount)
        {
            byte[] flags = new byte[pointsCount];
            for (int i = 0; i < pointsCount; i++)
            {
                flags[i] = reader.ReadByte();

                if (IsFlagSet(flags[i], OutlineFlags.Repeat))
                {
                    byte repeat_count = reader.ReadByte();
                    while (repeat_count-- > 0) flags[i + 1] = flags[i++];
                }
            }
            return flags;
        }

        bool IsFlagSet(byte flag, OutlineFlags outlineFlags)
        {
            return (flag & (byte)outlineFlags) != 0;
        }

        int[] ReadUInt16Array(FontReader reader, int length)
        {
            var result = new int[length];
            for (int i = 0; i < length; i++) result[i] = reader.ReadUInt16();
            return result;
        }
    }
    internal struct GlyphComponent
    {
        public ushort Flags;
        public ushort GlyphIndex;
        public int Argument1;
        public int Argument2;
        public int DestPointIndex;
        public int SrcPointIndex;

        // For 2x2 matrix
        // A topleft
        // B topright
        // C downLeft
        // D downRight
        public double A, B, C, D;

        // For translation
        // E is for x
        // F is for y
        public double E, F;
    }

    internal enum OutlineFlags
    {
        OnCurve = 1,
        XIsByte = 2,
        YIsByte = 4,
        Repeat = 8,
        XDelta = 16,
        YDelta = 32
    }
}