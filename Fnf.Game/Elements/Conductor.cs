using Fnf.Framework.Graphics;
using Fnf.Framework;
using System.IO;
using System;
using System.Security.Policy;
using System.Xml.XPath;

namespace Fnf.Game
{
    public class Conductor : GameObject, IRenderable, IUpdatable
    {
        // TODO: Fix this shit
        private static Key[] input = new Key[4] { Key.Q, Key.W, Key.BracketLeft, Key.BracketRight };
        private static string[] dirs = new string[] { "left", "down", "up", "right" };
        public bool isRenderable { get; set; } = true;
        public bool isUpdatable { get; set; } = true;

        public Character targetCharacter;
        public NoteData[] notes;
        public Animator[] columns;
        public float noteSpeed = 2f; // Note's distance per second is (screenGridHight * noteSpeed)
        public bool botplay;

        Animation[][] noteAnimations;
        float[] hitCooldown; // Cooldown for the controls to reset 
        float musicPosition;
        float previousMusicPosition;
        int botNoteIndex;
        int startIndex;
        int endIndex;

        public Conductor(string controlsConfigurations, string notesConfiguration, NoteData[] track, bool isPlayer)
        {
            botplay = !isPlayer;
            notes = track; 
            ApplyNotesConfiguration(notesConfiguration);
            ApplyControlsConfigurations(controlsConfigurations);
            for (int i = 0; i < columns.Length; i++) SetColumnState(i, "blank");
            hitCooldown = new float[4];
        }

        public void Update()
        {
            previousMusicPosition = musicPosition;
            musicPosition = (float)Music.Position;

            (float topTime, float bottomTime) = GetTimeOffsets(); // TODO: Add double sided colitions

            for (int i = startIndex; i < notes.Length; i++)
            {
                NoteData note = notes[i];
                float time = musicPosition - (note.delay + note.length); // The length is used so long notes doesn't suddenly dissappear

                if (time > topTime) startIndex++;
                else break;
            }

            for (int i = endIndex; i < notes.Length; i++)
            {
                NoteData note = notes[i];
                float time = musicPosition - note.delay;

                if (time < bottomTime) break;

                endIndex++;
            }

            startIndex = MathUtility.Clamp(startIndex, notes.Length - 1, 0);
            endIndex = MathUtility.Clamp(endIndex, notes.Length - 1, 0);

            if (botplay)
            {
                for (int i = botNoteIndex; i < notes.Length; i++)
                {
                    NoteData note = notes [i];

                    // Gets rid of note that we didn't reach yet
                    if (note.delay > musicPosition) break;

                    if (note.length > 0)
                    { // Its a hold note
                        if (note.state == NoteState.None)
                        { // The note is not pressed
                            note.state = NoteState.Bot;
                            note.holdProgress = note.delay + note.length - musicPosition;
                            hitCooldown[note.column] = 0.12f;
                            SetColumnState(note.column, "confirm");
                            targetCharacter?.Hit(dirs[note.column]);
                        }
                        else
                        { // The note is pressed
                            note.holdProgress = MathUtility.Clamp(note.delay + note.length - musicPosition, float.PositiveInfinity, 0);

                            if (note.holdProgress > 0 && hitCooldown[note.column] - Time.deltaTime <= 0)
                            {
                                hitCooldown[note.column] = 0.12f;
                                SetColumnState(note.column, "confirm");
                                targetCharacter?.Hit(dirs[note.column]);
                            }
                        }
                    }
                    else
                    { // Its a notmal note
                        if (note.state == NoteState.None)
                        { // The note is not pressed yet
                            note.state = NoteState.Bot;
                            hitCooldown[note.column] = 0.1f;
                            SetColumnState(note.column, "confirm");
                            targetCharacter?.Hit(dirs[note.column]);
                        }
                    }

                    if ((int)note.state > 0 && note.holdProgress == 0 && i == botNoteIndex + 1)
                    {
                        botNoteIndex = i;
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

                        for (int n = startIndex; n < endIndex + 1; n++)
                        {
                            NoteData note = notes[n];
                            float delay = Math.Abs(note.delay - musicPosition);
                            if (note.state == NoteState.None && note.column == i && delay < closestDelay)
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
                            NoteData note = notes[closestNote];
                            note.state = NoteState.Perfect;
                            targetCharacter?.Hit(dirs[note.column]);
                            SetColumnState(i, "confirm");
                        }
                    }
                    if (Input.GetKeyUp(input[i])) SetColumnState(i, "blank");
                }

                for (int n = startIndex; n < endIndex + 1; n++)
                {
                    NoteData note = notes[n];

                    if (note.length == 0) continue;

                    float noteDis = musicPosition - note.delay;
                    if (noteDis > 0)
                    {
                        if (noteDis <= note.length)
                        {
                            if (Input.GetKey(input[note.column]))
                            {
                                note.holdProgress = note.delay + note.length - musicPosition;

                                if (hitCooldown[note.column] == 0)
                                {
                                    targetCharacter?.Hit(dirs[note.column]);
                                    SetColumnState(note.column, "confirm");
                                    hitCooldown[note.column] = 0.12f;
                                }
                            }
                            else
                            {
                                if (hitCooldown[note.column] == 0)
                                {
                                    // Miss
                                    hitCooldown[note.column] = 0.12f;
                                }

                            }
                        }
                        else
                        {
                            float prevDis = previousMusicPosition - note.delay;
                            if (prevDis <= note.length && Input.GetKey(input[note.column]))
                            {
                                note.holdProgress = 0;
                            }
                        }
                    }
                }

                for (int i = 0; i < hitCooldown.Length; i++)
                {
                    hitCooldown[i] = MathUtility.Clamp(hitCooldown[i] - Time.deltaTime, 100, 0);
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
                NoteData note = notes[noteIndex];
                // Don't render notes that are pressed
                if (note.state > NoteState.Miss) continue;

                float notePlacement = (musicPosition - note.delay) * noteSpeed * Window.GridSize.height;

                for (int vertexIndex = 0; vertexIndex < 4; vertexIndex++)
                {
                    // Could be animated from here
                    Frame noteFrame = noteAnimations[0][note.column].frames[0];

                    OpenGL.TextureCoord(noteFrame.coords[vertexIndex]);

                    // We only need the rotation to be applied to the placement
                    Vector2 placement = (Matrix3.Rotation(-MathUtility.ToRadian(columns[note.column].globalRotation)) * new Vector3(0, notePlacement, 1)).ToEuclidean();
                    Vector2 frame = (columns[note.column].WorldlTransformMatrix() * noteFrame.verts[vertexIndex].ToHomogeneous()).ToEuclidean();
        
                    OpenGL.Pixel2(placement + frame);
                }
            }

            OpenGL.EndDrawing();
        }

        void RenderHolds()
        {
            const float Segment = 300, Overlap = 10;

            Texture.Use(noteAnimations[0][0].texture);

            for (int i = endIndex; i >= startIndex; i--)
            {
                NoteData note = notes[i];
                int column = notes[i].column;
                float progress = 1 - note.holdProgress / note.length;

                if (note.length > 0 && progress != 1)
                {
                    Vector2[] Hold_Vert = noteAnimations[1][column].frames[0].verts;
                    Vector2[] Hold_Coord = noteAnimations[1][column].frames[0].coords;
                    Vector2[] End_Vert = noteAnimations[2][column].frames[0].verts;
                    Vector2[] End_Coord = noteAnimations[2][column].frames[0].coords;

                    float startPlacement = (musicPosition - notes[i].delay) * noteSpeed * Window.GridSize.height;
                    float endPlacement = startPlacement - notes[i].length * noteSpeed * Window.GridSize.height;

                    // Used to know hold progress position
                    float midPlacement = MathUtility.Lerp(startPlacement, endPlacement, progress);
                    float lengthToDraw = Math.Abs(midPlacement - endPlacement);
                    float pointer = endPlacement;

                    OpenGL.BeginDrawing(DrawMode.Quads);

                    while (lengthToDraw > 0)
                    {
                        if (lengthToDraw < Segment)
                        {
                            float topTextureCoordinate = MathUtility.Lerp(Hold_Coord[2].y, Hold_Coord[0].y, lengthToDraw / Segment);
                            OpenGL.TextureCoord(Hold_Coord[0].x, topTextureCoordinate);
                            Pixel2(Hold_Vert[0].x, midPlacement);

                            OpenGL.TextureCoord(Hold_Coord[1].x, topTextureCoordinate);
                            Pixel2(Hold_Vert[1].x, midPlacement);

                            OpenGL.TextureCoord(Hold_Coord[2]);
                            Pixel2(Hold_Vert[2].x, pointer - Overlap);

                            OpenGL.TextureCoord(Hold_Coord[3]);
                            Pixel2(Hold_Vert[3].x, pointer - Overlap);
                        }
                        else
                        {
                            OpenGL.TextureCoord(Hold_Coord[0]);
                            Pixel2(Hold_Vert[0].x, pointer + Segment);

                            OpenGL.TextureCoord(Hold_Coord[1]);
                            Pixel2(Hold_Vert[1].x, pointer + Segment);

                            OpenGL.TextureCoord(Hold_Coord[2]);
                            Pixel2(Hold_Vert[2].x, pointer - Overlap);

                            OpenGL.TextureCoord(Hold_Coord[3]);
                            Pixel2(Hold_Vert[3].x, pointer - Overlap);
                        }

                        pointer += Segment;
                        lengthToDraw -= Segment;
                    }

                    float endPieceHeight = End_Vert[3].y - End_Vert[0].y; 

                    OpenGL.TextureCoord(End_Coord[0]);
                    Pixel2(End_Vert[0].x, endPlacement);

                    OpenGL.TextureCoord(End_Coord[1]);
                    Pixel2(End_Vert[1].x, endPlacement);

                    OpenGL.TextureCoord(End_Coord[2]);
                    Pixel2(End_Vert[2].x, endPlacement + endPieceHeight);

                    OpenGL.TextureCoord(End_Coord[3]);
                    Pixel2(End_Vert[3].x, endPlacement + endPieceHeight);

                    OpenGL.EndDrawing();

                    void Pixel2(float x, float y)
                    {
                        // Only translation and rotation and scaleX is allowed to not alter the note position
                        Animator c = columns[column];
                        OpenGL.Pixel2((
                            Matrix3.Transform(c.globalPosition, -MathUtility.ToRadian(c.globalRotation), new Vector2(c.globalScale.x, 1)) * 
                            new Vector3(x, y, 1)).ToEuclidean()); 
                    }
                }
            }
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

            var Sections = StringUtility.SplitIntoSegmentedSections(File.ReadAllLines($"{GamePaths.NotesConfigurations}\\{noteConfigurations}.txt"));
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