using Fnf.Framework.Graphics;
using Fnf.Framework;
using System.IO;
using System;
using System.Security.Policy;

namespace Fnf.Game
{
    public class Conductor : MovableObject, IRenderable, IUpdatable
    {
        // TODO: Fix this shit
        private static Key[] input = new Key[4] { Key.Q, Key.W, Key.BracketLeft, Key.BracketRight };
        private static string[] dirs = new string[] { "left", "down", "up", "right" };
        public bool isRenderable { get; set; } = true;
        public bool isUpdatable { get; set; } = true;

        public Character targetCharacter;
        public NoteTrack noteTrack;
        public Animator[] columns;
        public float noteSpeed = 2f; // Note's distance per second is (screenGridHight * noteSpeed)
        public bool botplay;

        Animation[][] noteAnimations;
        float[] holdCooldown;
        float[] hitCooldown;
        float musicPosition;
        int botNoteIndex;
        int startIndex;
        int endIndex;

        public Conductor(string controlsConfigurations, string notesConfiguration, NoteTrack track, bool isPlayer)
        {
            botplay = !isPlayer;
            noteTrack = track; 
            ApplyNotesConfiguration(notesConfiguration);
            ApplyControlsConfigurations(controlsConfigurations);
            for (int i = 0; i < columns.Length; i++) SetColumnState(i, "blank");
            hitCooldown = new float[4];
        }

        public void Update()
        {
            musicPosition = (float)Music.Position;

            (float topTime, float bottomTime) = GetTimeOffsets(); // TODO: Add double sided colitions

            for (int i = startIndex; i < noteTrack.notes.Length; i++)
            {
                NoteData note = noteTrack.notes[i];
                float time = musicPosition - (note.delay + note.length); // The length is used so long notes doesn't suddenly dissappear

                if (time > topTime) startIndex++;
                else break;
            }

            for (int i = endIndex; i < noteTrack.notes.Length; i++)
            {
                NoteData note = noteTrack.notes[i];
                float time = musicPosition - note.delay;

                if (time < bottomTime) break;

                endIndex++;
            }

            startIndex = MathUtility.Clamp(startIndex, noteTrack.notes.Length - 1, 0);
            endIndex = MathUtility.Clamp(endIndex, noteTrack.notes.Length - 1, 0);

            if (botplay)
            {
                for (int i = botNoteIndex; i < noteTrack.notes.Length; i++)
                {
                    if (noteTrack.notes[i].delay < musicPosition)
                    {
                        if (!noteTrack.notes[i].pressed)
                        {
                            botNoteIndex++;
                            noteTrack.notes[i].pressed = true;
                            SetColumnState(noteTrack.notes[i].column, "confirm");
                            targetCharacter?.play(dirs[noteTrack.notes[i].column]);
                            hitCooldown[noteTrack.notes[i].column] = 0.1f;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                for (int i = 0; i < hitCooldown.Length; i++)
                {
                    if (hitCooldown[i] > 0)
                    {
                        hitCooldown[i] -= Time.deltaTime;
                        if (hitCooldown[i] == 0) hitCooldown[i] = -1; 
                    }

                    if (hitCooldown[i] < 0)
                    {
                        hitCooldown[i] = 0;
                        SetColumnState(i, "blank");
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (Input.GetKeyDown(input[i]))
                    {
                        int closestNote = -1;
                        float closestDelay = float.MaxValue;

                        for (int n = 0; n < noteTrack.notes.Length; n++)
                        {
                            NoteData note = noteTrack.notes[n];
                            float delay = Math.Abs(note.delay - musicPosition);
                            if (!note.pressed && note.column == i && delay < closestDelay)
                            {
                                closestNote = n;
                                closestDelay = delay;
                            }
                        }

                        if (closestNote == -1)
                        {
                            SetColumnState(i, "press");
                        }
                        else
                        {
                            NoteData note = noteTrack.notes[closestNote];
                            note.pressed = true;
                            targetCharacter?.play(dirs[note.column]);
                            SetColumnState(i, "confirm");
                        }
                    }
                    if (Input.GetKeyUp(input[i])) SetColumnState(i, "blank");
                }
            }

            for (int i = 0; i < columns.Length; i++)
            {
                columns[i].Update();
            }
        }

        public void Render()
        {
            // Render controls
            for (int i = 0; i < columns.Length; i++)
            {
                columns[i].Render();
            }

            RenderHolds();
            RenderNotes();
        }

        void SetColumnState(int column, string state) => columns[column].play(state);

        // TODO: Animated notes and custom notes are not supported for now for my sanity, later
        void RenderNotes()
        {
            //int inUseTexture = OpenGL.NULL;

            Texture.Use(noteAnimations[0][0].texture);
            OpenGL.Color3(1f, 1f, 1f);
            OpenGL.BeginDrawing(DrawMode.Quads);

            for (int noteIndex = endIndex; noteIndex >= startIndex; noteIndex--)
            {
                if (noteTrack.notes[noteIndex].pressed) continue;

                float hitOffset = (float)musicPosition - noteTrack.notes[noteIndex].delay;
                float noteDisplacement = hitOffset * noteSpeed * Window.GridSize.height;
                int column = noteTrack.notes[noteIndex].column;

                for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                {
                    // Could be animated from here
                    Frame noteFrame = noteAnimations[0][column].frames[0];

                    OpenGL.TextureCoord(noteFrame.coords[vertexIndex]);

                    Vector2 displacement = new Vector2(0, noteDisplacement);
                    displacement = displacement.Rotate(columns[column].globalRotation);

                    Vector2 vertex = noteFrame.verts[vertexIndex];
                    vertex *= columns[column].globalScale;
                    vertex = vertex.Rotate(columns[column].globalRotation);
                    vertex += columns[column].globalPosition;

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

            Texture.Use(noteAnimations[0][0].texture);

            for (int i = endIndex; i >= startIndex; i--)
            {
                if (noteTrack.notes[i].length > 0 && noteTrack.notes[i].hold != 1)
                {
                    int column = noteTrack.notes[i].column;

                    Vector2[] Hold_Vert = noteAnimations[1][column].frames[0].verts;
                    Vector2[] Hold_Coord = noteAnimations[1][column].frames[0].coords;
                    Vector2[] End_Vert = noteAnimations[2][column].frames[0].verts;
                    Vector2[] End_Coord = noteAnimations[2][column].frames[0].coords;

                    float hitOffset = (float)Music.Position - noteTrack.notes[i].delay;
                    float noteDisplacement = hitOffset * noteSpeed * Window.GridSize.height;

                    Vector2 endPoint = new Vector2(0, noteDisplacement - noteTrack.notes[i].length * noteSpeed * Window.GridSize.height);
                    Vector2 startPoint = new Vector2(0, Lerp(noteDisplacement, endPoint.y, noteTrack.notes[i].hold));
                    Vector2 drawPoint = endPoint;

                    OpenGL.BeginDrawing(DrawMode.Quads);

                    float remainingLength = noteTrack.notes[i].length * noteSpeed * Window.GridSize.height * (1 - noteTrack.notes[i].hold);

                    while (remainingLength > 0)
                    {
                        if (remainingLength < HoldSegment)
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
                        Animator nc = columns[column];
                        vector2.x *= nc.globalScale.x;
                        vector2 = vector2.Rotate(nc.globalRotation);
                        vector2 += nc.globalPosition;
                        return vector2;
                    }
                }
            }

            float Lerp(float a, float b, float t) => a + (b - a) * t;
        }

        (float topTime, float bottomTime) GetTimeOffsets()
        {
            float topPosition = float.NegativeInfinity;
            float bottomPosition = float.PositiveInfinity;

            for (int i = 0; i < columns.Length; i++)
            {
                // Some trigonometry shit
                float halfHeight = Window.GridSize.height / 2f;
                float cos = (float)Math.Cos(MathUtility.ToRadian(-columns[i].globalRotation));
                topPosition = Math.Max(halfHeight - columns[i].globalPosition.y / cos, topPosition);
                bottomPosition = Math.Min(-(halfHeight + columns[i].globalPosition.y) / cos, bottomPosition);
            }

            // Add some offset so the note doesn't just appear out of thin air
            topPosition += 220 * globalScale.y;
            bottomPosition -= 220 * globalScale.y;
            float distpersec = noteSpeed * Window.GridSize.height;
            return (topPosition / distpersec, bottomPosition / distpersec);
        }

        public void ApplyNotesConfiguration(string noteConfigurations)
        {
            noteAnimations = new Animation[3][];

            var Sections = StringUtility.SplitSections(File.ReadAllLines($"{GamePaths.NotesConfigurations}\\{noteConfigurations}.txt"));
            foreach ((string Section, string[] Entries) in Sections)
            {
                switch(Section)
                {
                    case "Note":
                    {
                        Animation[] note = new Animation[4];
                        for (int i = 0; i < Entries.Length; i++)
                        {
                            string[] entryValues = StringUtility.SplitValues(Entries[i]);

                            string atlasPath = StringUtility.IncertAt(entryValues[0], "@", GamePaths.Notes);
                            string animationName = entryValues[1];

                            TextureAtlas.LoadAtlas(noteConfigurations, atlasPath);
                            Animation animation = TextureAtlas.GetAnimation(noteConfigurations, animationName);
                            note[i] = animation;
                        }
                        noteAnimations[0] = note;
                        break;
                    }
                    case "Hold":
                    {
                        Animation[] hold = new Animation[4];
                        for (int i = 0; i < Entries.Length; i++)
                        {
                            string[] entryValues = StringUtility.SplitValues(Entries[i]);

                            string atlasPath = StringUtility.IncertAt(entryValues[0], "@", GamePaths.Notes);
                            string animationName = entryValues[1];

                            TextureAtlas.LoadAtlas(noteConfigurations, atlasPath);
                            Animation animation = TextureAtlas.GetAnimation(noteConfigurations, animationName);
                            hold[i] = animation;
                        }
                        noteAnimations[1] = hold;
                        break;
                    }
                    case "End":
                    {
                        Animation[] end = new Animation[4];
                        for (int i = 0; i < Entries.Length; i++)
                        {
                            string[] entryValues = StringUtility.SplitValues(Entries[i]);

                            string atlasPath = StringUtility.IncertAt(entryValues[0], "@", GamePaths.Notes);
                            string animationName = entryValues[1];

                            TextureAtlas.LoadAtlas(noteConfigurations, atlasPath);
                            Animation animation = TextureAtlas.GetAnimation(noteConfigurations, animationName);
                            end[i] = animation;
                        }
                        noteAnimations[2] = end;
                        break;
                    }
                }
            }
        }

        public void ApplyControlsConfigurations(string controlsConfigurations)
        {
            string[] configData = File.ReadAllLines($"{GamePaths.ControlsConfigurations}/{controlsConfigurations}.txt");
            string[] head = StringUtility.SplitValues(configData[0], 0);
            float spacing = float.Parse(head[0]);
            TextureAtlas.LoadAtlas("Config-" + controlsConfigurations, head[1]);

            if (columns == null) columns = new Animator[4];

            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i] == null)
                {
                    columns[i] = new Animator();
                }
                else columns[i].clear();

                columns[i].parent = this;
                columns[i].localPosition = new Vector2(spacing * (i - 1.5f), 0);
            }

            for (int l = 1; l < configData.Length; l++)
            {
                string[] args = StringUtility.SplitValues(configData[l], 1);
                for (int i = 0; i < columns.Length; i++)
                {
                    Animator animator = columns[i];
                    animator.add(args[0], TextureAtlas.GetAnimation("Config-" + controlsConfigurations, args[i + 1]));
                }
            }
        }
    }
}