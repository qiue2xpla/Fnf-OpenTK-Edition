using System.Collections.Generic;
using System.Linq;
using System;

namespace Fnf.Framework.TrueType
{
    internal class FontParser : IDisposable
    {
        public char MissingChar;
        public int UnitsPerEm;

        Dictionary<string, uint> TableOffsets;
        IMappingFormat CharacterMapping;
        FontReader reader;

        (int AdvanceWidth, int LeftBearing)[] HorizontalLayout;
        int IndexToLocationFormat, GlyphCount;
        uint[] GlyphLocations;

        public FontParser(FontReader reader)
        {
            this.reader = reader;

            #region Handling Table Offsets

            TableOffsets = new Dictionary<string, uint>();

            reader.Position = sizeof(uint);
            int tableCount = reader.ReadUInt16();
            reader.Position += 3 * sizeof(ushort);

            for (int i = 0; i < tableCount; i++)
            {
                string tag = reader.ReadTag();
                reader.Position += 4; //Checksum
                uint offset = reader.ReadUInt32();
                reader.Position += 4; //Length

                TableOffsets.Add(tag, offset);
            }

            #endregion
            #region Handling Character Mapping

            Dictionary<(int platID, int PlatSPID), uint> formats = new Dictionary<(int platID, int PlatSPID), uint>();
            reader.Position = TableOffsets["cmap"] + sizeof(ushort);
            int SubtablesCount = reader.ReadUInt16();

            while (SubtablesCount > 0)
            {
                int platformID = reader.ReadUInt16();
                int platformSpecificID = reader.ReadUInt16();
                uint offset = reader.ReadUInt32() + TableOffsets["cmap"];
                formats.Add((platformID, platformSpecificID), offset);
                SubtablesCount--;
            }

            // Format 4
            if (formats.ContainsKey((0,3)))
            {
                CharacterMapping = new Format4(formats[(0,3)], reader);
            }

            if (CharacterMapping == null)
            {
                throw new Exception("Font doesn't contain a supported mapping data");
            }

            MissingChar = CharacterMapping.MissingCharacter;

            #endregion
            #region Handling Reading Some Variables

            reader.Position = TableOffsets["maxp"] + 4;
            GlyphCount = reader.ReadUInt16();

            uint head_offset = TableOffsets["head"];
            reader.Position = head_offset + 18;
            UnitsPerEm = reader.ReadUInt16();
            reader.Position = head_offset + 48;
            IndexToLocationFormat = reader.ReadInt16();

            #endregion
            #region Handling Glyph Locations

            GlyphLocations = new uint[GlyphCount];

            reader.Position = TableOffsets["loca"];
            for (int i = 0; i < GlyphCount; i++)
            {
                GlyphLocations[i] = IndexToLocationFormat == 1 ? reader.ReadUInt32() : reader.ReadUInt16() * 2u;
            }

            #endregion
            #region Handling Layouts

            HorizontalLayout = new (int, int)[GlyphCount];

            reader.Position = TableOffsets["hhea"] += 34;
            int HorizontalMatrixesCount = reader.ReadUInt16();

            int lastAdvanceWidth = 0;

            reader.Position = TableOffsets["hmtx"];
            for (int i = 0; i < HorizontalMatrixesCount; i++)
            {
                int advanceWidth = reader.ReadUInt16();
                int leftSideBearing = reader.ReadInt16();
                lastAdvanceWidth = advanceWidth;

                HorizontalLayout[i] = (advanceWidth, leftSideBearing);
            }

            int Remaining = GlyphCount - HorizontalMatrixesCount;

            for (int i = 0; i < Remaining; i++)
            {
                int leftSideBearing = reader.ReadInt16();
                int glyphIndex = HorizontalMatrixesCount + i;

                HorizontalLayout[glyphIndex] = (lastAdvanceWidth, leftSideBearing);
            }

            #endregion
        }

        public Dictionary<char, Glyph> GetGlyphs()
        {
            Dictionary<char, Glyph> glyphs = new Dictionary<char, Glyph>();

            foreach (var pair in CharacterMapping.UnicodeToGlyphIndex)
            {
                char character = pair.Key;
                uint glyphIndex = pair.Value;
                
                if(glyphIndex + 1 < GlyphLocations.Length)
                {
                    if (GlyphLocations[glyphIndex + 1] - GlyphLocations[glyphIndex] == 0)
                    {
                        // Glyph has no outline data
                        // https://www.fast-report.com/blogs/font-truetype-from-inside

                        GlyphMetrics met = new GlyphMetrics()
                        {
                            UnitsPerEm = UnitsPerEm,
                            AdvanceWidth = HorizontalLayout[glyphIndex].AdvanceWidth,
                            LeftSideBearing = HorizontalLayout[glyphIndex].LeftBearing,
                            MinX = 0,
                            MaxX = 0,
                            MinY = 0,
                            MaxY = 0
                        };

                        glyphs.Add(character, new Glyph()
                        {
                            Character = character,
                            Curves = new Curve[0],
                            Metrics = met
                        });

                        continue;
                    }
                }

                GlyphData glyphData = ReadGlyph(glyphIndex);
                Curve[] curves = GetCurves(glyphData.points, glyphData.CountourEndPoints);
                GlyphMetrics metrics = new GlyphMetrics()
                {
                    UnitsPerEm = UnitsPerEm,
                    AdvanceWidth = HorizontalLayout[glyphIndex].AdvanceWidth,
                    LeftSideBearing = HorizontalLayout[glyphIndex].LeftBearing,
                    MinX = glyphData.xMin,
                    MaxX = glyphData.xMax,
                    MinY = glyphData.yMin,
                    MaxY = glyphData.yMax
                };

                glyphs.Add(character, new Glyph()
                {
                    Character = character,
                    Metrics = metrics,
                    Curves = curves
                });
            }

            return glyphs;
        }
        
        public void Dispose()
        {
            TableOffsets = null;
            CharacterMapping = null;
            HorizontalLayout = null;
            GlyphLocations = null;
            reader = null;
        }

        #region Parsing 

        GlyphData ReadGlyph(uint glyphIndex)
        {
            reader.Position = TableOffsets["glyf"] + GlyphLocations[glyphIndex];

            int CountourCount = reader.ReadInt16();
            reader.Position -= 2;

            if (CountourCount < 0)
            {
                return ReadCompoundGlyph(glyphIndex);
            }
            else
            {
                return ReadSimpleGlyph(glyphIndex);
            }
        }

        GlyphData ReadSimpleGlyph(uint glyphIndex)
        {
            int CountourCount = reader.ReadInt16();

            int xMin = reader.ReadInt16();
            int yMin = reader.ReadInt16();
            int xMax = reader.ReadInt16();
            int yMax = reader.ReadInt16();

            ushort[] EndPointsOfContours = reader.ReadUInt16Array(CountourCount);
            int PointsCount = EndPointsOfContours.Max() + 1;

            uint instructionLength = reader.ReadUInt16();
            reader.Position += instructionLength;

            byte[] flags = new byte[PointsCount];
            for (int i = 0; i < PointsCount; i++)
            {
                byte flag = reader.ReadByte();

                flags[i] = flag;

                if (IsFlagSet(flag, OutlineFlags.Repeat))
                {
                    byte repeat_count = reader.ReadByte();

                    while (repeat_count > 0)
                    {
                        i++;
                        flags[i] = flag;
                        repeat_count--;
                    }
                }
            }

            int[] CoordsX = Read(true);
            int[] CoordsY = Read(false);

            GlyphPoint[] points = new GlyphPoint[PointsCount];
            for (int i = 0; i < PointsCount; i++)
            {
                points[i] = new GlyphPoint(CoordsX[i], CoordsY[i], IsFlagSet(flags[i], OutlineFlags.OnCurve));
            }

            return new GlyphData()
            {
                CountourEndPoints = EndPointsOfContours,
                points = points,

                xMin = xMin,
                xMax = xMax,
                yMin = yMin,
                yMax = yMax
            };

            int[] Read(bool isX)
            {
                OutlineFlags Short = isX ? OutlineFlags.XIsByte : OutlineFlags.YIsByte;
                OutlineFlags Delta = isX ? OutlineFlags.XDelta : OutlineFlags.YDelta;
                int[] array = new int[PointsCount];

                for (int i = 0; i < PointsCount; i++)
                {
                    array[i] = array[Math.Max(0, i - 1)];

                    if (IsFlagSet(flags[i], Short))
                    {
                        array[i] += IsFlagSet(flags[i], Delta) ? reader.ReadByte() : -reader.ReadByte();
                    }
                    else if (!IsFlagSet(flags[i], Delta))
                    {
                        array[i] += reader.ReadInt16();
                    }
                }

                return array;
            }

            bool IsFlagSet(byte flag, OutlineFlags outlineFlags)
            {
                return (flag & (byte)outlineFlags) != 0;
            }
        }

        GlyphData ReadCompoundGlyph(uint glyphIndex)
        {
            GlyphData glyph = new GlyphData();

            reader.Position += 2;
            glyph.xMin = reader.ReadInt16();
            glyph.yMin = reader.ReadInt16();
            glyph.xMax = reader.ReadInt16();
            glyph.yMax = reader.ReadInt16();

            //Read components
            List<GlyphComponent> components = new List<GlyphComponent>();
            bool hasMoreComponents = true;
            while (hasMoreComponents)
            {
                GlyphComponent component = new GlyphComponent();
                component.A = 1; component.D = 1;
                component.Flags = reader.ReadUInt16();
                component.GlyphIndex = reader.ReadUInt16();

                // Decode flags
                bool argsAre2Bytes = FlagBitIsSet(component.Flags, 0);
                bool argsAreXYValues = FlagBitIsSet(component.Flags, 1);
                bool roundXYToGrid = FlagBitIsSet(component.Flags, 2);
                bool isSingleScaleValue = FlagBitIsSet(component.Flags, 3);
                hasMoreComponents = FlagBitIsSet(component.Flags, 5);
                bool isXAndYScale = FlagBitIsSet(component.Flags, 6);
                bool is2x2Matrix = FlagBitIsSet(component.Flags, 7);
                bool hasInstructions = FlagBitIsSet(component.Flags, 8);
                bool useThisComponentMetrics = FlagBitIsSet(component.Flags, 9);
                bool componentsOverlap = FlagBitIsSet(component.Flags, 10);

                if (argsAre2Bytes)
                {
                    component.Argument1 = reader.ReadInt16();
                    component.Argument2 = reader.ReadInt16();
                }
                else
                {
                    component.Argument1 = reader.ReadByte();
                    component.Argument2 = reader.ReadByte();
                }

                if (argsAreXYValues)
                {
                    component.E = component.Argument1;
                    component.F = component.Argument2;
                }
                else
                {
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

                if(hasInstructions)
                {
                    ushort numInstr = reader.ReadUInt16();
                    reader.Position += numInstr;
                }

                components.Add(component);
            }

            //Add components
            List<ushort> allContourEndIndices = new List<ushort>();
            List<GlyphPoint> allPoints = new List<GlyphPoint>();
            foreach (var component in components)
            {
                if (component.GlyphIndex >= GlyphLocations.Length) continue;

                GlyphData componentGlyph = ReadGlyph(component.GlyphIndex);

                foreach (ushort endIndex in componentGlyph.CountourEndPoints)
                    allContourEndIndices.Add((ushort)(endIndex + allPoints.Count));

                foreach (var point in componentGlyph.points)
                {
                    GlyphPoint newPoint = Transform(point, component);

                    if (!FlagBitIsSet(component.Flags, 1)) //if not xy values
                    {
                        if (inRange(component.DestPointIndex, allPoints.Count) && inRange(component.SrcPointIndex, componentGlyph.points.Length))
                        {
                            newPoint.x +=
                                allPoints[component.DestPointIndex].x -
                                componentGlyph.points[component.SrcPointIndex].x;

                            newPoint.y +=
                                allPoints[component.DestPointIndex].y -
                                componentGlyph.points[component.SrcPointIndex].y;
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
                    point.x = (int)(comp.A * point.x + comp.C * point.y + comp.E);
                    point.y = (int)(comp.B * point.x + comp.D * point.y + comp.F);
                    return point;
                }
            }

            glyph.points = allPoints.ToArray();
            glyph.CountourEndPoints = allContourEndIndices.ToArray();
            return glyph;

            bool FlagBitIsSet(uint flag, int bitIndex) => ((flag >> bitIndex) & 1) == 1;
        }

        Curve[] GetCurves(GlyphPoint[] points, ushort[] contourEndPoints)
        {
            List<Curve> result = new List<Curve>();

            GlyphPoint[][] contours = GetContours(points, contourEndPoints);
            foreach (GlyphPoint[] contour in contours)
            {
                GlyphPoint[] impliedContour = GetImplidContour(contour);

                for (int i = 0; i < impliedContour.Length; i++)
                {
                    if (!impliedContour[i].OnCurve)
                    {
                        Vector2 a = get(impliedContour[i + -1]);
                        Vector2 b = get(impliedContour[i]);
                        Vector2 c = get(impliedContour[i + 1]);

                        Curve cur = new Curve(a, b, c);

                        if (cur.isSplitable())
                        {
                            (Curve c1, Curve c2) = cur.Split();
                            result.Add(c1);
                            result.Add(c2);
                        }
                        else
                        {
                            result.Add(cur);
                        }

                        Vector2 get(GlyphPoint p) => new Vector2(p.x, p.y);
                    }
                }
            }

            return result.ToArray();
        }

        GlyphPoint[][] GetContours(GlyphPoint[] points, ushort[] contourEndPoints)
        {
            GlyphPoint[][] contours = new GlyphPoint[contourEndPoints.Length][];

            int startIndex = 0;

            for (int contour = 0; contour < contourEndPoints.Length; contour++)
            {
                int length = contourEndPoints[contour] - startIndex + 1;
                contours[contour] = GetRange(length);
                startIndex += length;
            }

            return contours;

            GlyphPoint[] GetRange(int length)
            {
                GlyphPoint[] arr = new GlyphPoint[length];
                for (int i = startIndex; i < startIndex + length; i++) arr[i - startIndex] = points[i];
                return arr;
            }
        }

        GlyphPoint[] GetImplidContour(GlyphPoint[] contour)
        {
            List<GlyphPoint> result = new List<GlyphPoint>();

            for (int i = 0; i < contour.Length; i++)
            {
                result.Add(contour[i]);

                if (i + 1 < contour.Length && contour[i].OnCurve == contour[i + 1].OnCurve)
                {
                    result.Add(new GlyphPoint()
                    {
                        x = (int)((contour[i].x + contour[i + 1].x) / 2f),
                        y = (int)((contour[i].y + contour[i + 1].y) / 2f),
                        OnCurve = !contour[i].OnCurve
                    });
                }
            }

            if (result.Count <= 2) return result.ToArray();

            if (result[0].OnCurve != result[result.Count - 1].OnCurve)
            {
                if (result[0].OnCurve)
                {
                    result.Add(result[0]);
                }
                else
                {
                    result.Insert(0, result[result.Count - 1]);
                }
            }
            else
            {
                if (result[0].OnCurve)
                {
                    if (result[0].x != result[result.Count - 1].x ||
                        result[0].y != result[result.Count - 1].y)
                    {
                        result.Add(new GlyphPoint()
                        {
                            x = (int)((result[0].x + result[result.Count - 1].x) / 2f),
                            y = (int)((result[0].y + result[result.Count - 1].y) / 2f),
                            OnCurve = false
                        });
                        result.Add(result[0]);
                    }
                }
                else
                {
                    throw new NotImplementedException(
                        "This case of (nonCurve-nonCurve) at the start and end of contour is not implumented");
                }
            }

            return result.ToArray();
        }

        #endregion

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
}