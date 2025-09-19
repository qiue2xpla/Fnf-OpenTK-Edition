using Fnf.Framework.Graphics;
using Fnf.Framework;
using System;

namespace Fnf.Game
{
    public abstract class ConveyorBase : MovableObject
    {/*
        public event Action<int> OnHit;

        private ViewRange viewRange;

            viewRange = new ViewRange(this);

        void RenderHolds()
        {
            // Might be moved to the note skin 
            const float HoldSegment = 300;
            const float OverlapInPixels = 10;

            Texture.Use(notesSkin.Hold[0].texture);

            for (int i = viewRange.LowerIndex; i >= viewRange.UpperIndex; i--)
            {
                if (chart.notes[i].length > 0 && chart.notes[i].hold != 1)
                {
                    int column = chart.notes[i].column;

                    Vector2[] Hold_Vert = notesSkin.Hold[column].frames[0].verts;
                    Vector2[] Hold_Coord = notesSkin.Hold[column].frames[0].coords;
                    Vector2[] End_Vert = notesSkin.End[column].frames[0].verts;
                    Vector2[] End_Coord = notesSkin.End[column].frames[0].coords;

                    float hitOffset = (float)Music.Position - chart.notes[i].delay;
                    float noteDisplacement = hitOffset * DistanceInSecond;

                    Vector2 endPoint = new Vector2(0, noteDisplacement - chart.notes[i].length * DistanceInSecond);
                    Vector2 startPoint = new Vector2(0, Lerp(noteDisplacement, endPoint.y, chart.notes[i].hold));
                    Vector2 drawPoint = endPoint;

                    OpenGL.BeginDrawing(DrawMode.Quads);

                    float remainingLength = chart.notes[i].length * DistanceInSecond * (1-chart.notes[i].hold);

                    while(remainingLength > 0)
                    {
                        if(remainingLength < HoldSegment)
                        {
                            OpenGL.TextureCoord(Hold_Coord[0]);
                            OpenGL.Pixel2(transform(new Vector2(Hold_Vert[0].x, startPoint.y)));

                            OpenGL.TextureCoord(Hold_Coord[1]);
                            OpenGL.Pixel2(transform(new Vector2(Hold_Vert[1].x, startPoint.y)));

                            OpenGL.TextureCoord(Hold_Coord[2]);
                            OpenGL.Pixel2(transform(new Vector2(Hold_Vert[2].x, drawPoint.y - OverlapInPixels)));

                            OpenGL.TextureCoord(Hold_Coord[3]);
                            OpenGL.Pixel2(transform(new Vector2(Hold_Vert[3].x, drawPoint.y - OverlapInPixels)));
                        }
                        else
                        {
                            OpenGL.TextureCoord(Hold_Coord[0]);
                            OpenGL.Pixel2(transform(new Vector2(Hold_Vert[0].x, drawPoint.y + HoldSegment)));

                            OpenGL.TextureCoord(Hold_Coord[1]);
                            OpenGL.Pixel2(transform(new Vector2(Hold_Vert[1].x, drawPoint.y + HoldSegment)));

                            OpenGL.TextureCoord(Hold_Coord[2]);
                            OpenGL.Pixel2(transform(new Vector2(Hold_Vert[2].x, drawPoint.y - OverlapInPixels)));

                            OpenGL.TextureCoord(Hold_Coord[3]);
                            OpenGL.Pixel2(transform(new Vector2(Hold_Vert[3].x, drawPoint.y - OverlapInPixels)));
                        }

                        drawPoint.y += HoldSegment;
                        remainingLength -= HoldSegment;
                    }

                    float offset = End_Vert[3].y - End_Vert[0].y;

                    OpenGL.TextureCoord(End_Coord[0]);
                    OpenGL.Pixel2(transform(new Vector2(End_Vert[0].x, endPoint.y)));

                    OpenGL.TextureCoord(End_Coord[1]);
                    OpenGL.Pixel2(transform(new Vector2(End_Vert[1].x, endPoint.y)));

                    OpenGL.TextureCoord(End_Coord[2]);
                    OpenGL.Pixel2(transform(new Vector2(End_Vert[2].x, endPoint.y + offset)));

                    OpenGL.TextureCoord(End_Coord[3]);
                    OpenGL.Pixel2(transform(new Vector2(End_Vert[3].x, endPoint.y + offset)));

                    OpenGL.EndDrawing();

                    Vector2 transform(Vector2 vector2)
                    {
                        NoteColumn nc = columns[column];
                        vector2.x *= nc.animator.globalScale.x;
                        vector2 = vector2.Rotate(nc.animator.globalRotation);
                        vector2 += nc.animator.globalPosition;
                        return vector2;
                    }
                }
            }

            float Lerp(float a, float b, float t) => a + (b - a) * t;
        }*/
    }
}