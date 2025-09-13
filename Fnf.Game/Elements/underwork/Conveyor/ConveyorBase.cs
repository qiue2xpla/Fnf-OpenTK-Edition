using Fnf.Framework.Graphics;
using Fnf.Framework;
using System;

namespace Fnf.Game
{
    public abstract class ConveyorBase : MovableObject
    {
        public static Beatmap CurrentBeatmap;

        private static float _speed = 2.5f;
        public static float DistanceInSecond => 700 * Speed;
        public static float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        public ControlsSkin controlsSkin;
        public NotesSkin notesSkin;

        public event Action<int> OnHit;
        public NoteColumn[] columns;
        public Chart chart;

        private ViewRange viewRange;

        public ConveyorBase(ControlsSkin controlsSkin, NotesSkin notesSkin)
        {
            this.controlsSkin = controlsSkin;
            this.notesSkin = notesSkin;

            InitializeColumns();
            for (int i = 0; i < 4; i++)  Release(i);

            viewRange = new ViewRange(this);
        }

        public virtual void Update()
        {
            for (int i = 0; i < columns.Length; i++) columns[i].animator.Update();
        }

        public void Render()
        {
            viewRange.Update();

            RenderControls();
            RenderHolds();
            RenderNotes();
        }

        protected void Release(int column)
        {
            columns[column].animator.play("blank");
        }

        protected void Press(int column)
        {
            columns[column].animator.play("press");
        }

        protected void Confirm(int column)
        {
            columns[column].animator.play("confirm");
        }

        protected void Hit(int column)
        {
            OnHit?.Invoke(column);
        }

        void InitializeColumns()
        {
            float offset = controlsSkin.ColumnsSpacing * -1.5f;
            columns = new NoteColumn[4];
            for (int i = 0; i < 4; i++)
            {
                float x = controlsSkin.ColumnsSpacing * i + offset;
                columns[i] = new NoteColumn();

                columns[i].animator = new Animator();
                columns[i].animator.parent = this;
                columns[i].animator.localPosition = new Vector2(x, 0);

                columns[i].animator.add("blank", controlsSkin.Blank[i]);
                columns[i].animator.add("press", controlsSkin.Press[i]);
                columns[i].animator.add("confirm", controlsSkin.Confirm[i]);
            }
        }

        void RenderControls()
        {
            for (int i = 0; i < 4; i++)
                columns[i].animator.Render();
        }

        // Animated notes are not supported for now for my sanity
        void RenderNotes()
        {
            Texture.Use(notesSkin.Note[0].texture);
            Shader.Use(-1);
            OpenGL.Color3(1f, 1f, 1f);
            OpenGL.BeginDrawing(DrawMode.Quads);

            for (int noteIndex = viewRange.LowerIndex; noteIndex >= viewRange.UpperIndex; noteIndex--)
            {
                if (chart.notes[noteIndex].pressed) continue;

                float hitOffset = (float)Music.Position - chart.notes[noteIndex].delay;
                float noteDisplacement = hitOffset * DistanceInSecond;
                int column = chart.notes[noteIndex].column;

                for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                {
                    // Could be animated from here
                    Frame noteFrame = notesSkin.Note[column].frames[0];

                    OpenGL.TextureCoord(noteFrame.coords[vertexIndex]);

                    Vector2 displacement = new Vector2(0, noteDisplacement);
                    displacement = displacement.Rotate(columns[column].animator.globalRotation);

                    Vector2 vertex = noteFrame.verts[vertexIndex];
                    vertex *= columns[column].animator.globalScale;
                    vertex = vertex.Rotate(columns[column].animator.globalRotation);
                    vertex += columns[column].animator.globalPosition;
                    
                    OpenGL.Pixel2(vertex + displacement);
                }
            }

            OpenGL.EndDrawing();
        }

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
        }
    }
}