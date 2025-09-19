using Fnf.Framework;
using Fnf.Framework.Graphics;
using System.IO;

namespace Fnf.Game
{
    public class Conductor : MovableObject, IRenderable, IUpdatable
    {
        // TODO: Fix this shit
        private static Key[] input = new Key[4] { Key.Q, Key.W, Key.BracketLeft, Key.BracketRight };
        public bool isRenderable { get; set; } = true;
        public bool isUpdatable { get; set; } = true;

        public NoteTrack noteTrack;
        public Animator[] columns;
        public float noteSpeed = 2.5f; // Note's distance per second is (screenGridHight * noteSpeed)

        Animation[][] noteAnimations;
        float[] holdCooldown;
        float[] hitCooldown;

        public Conductor(string controlsConfigurations, string notesConfiguration, NoteTrack track)
        {
            noteTrack = track; 
            ApplyNotesConfiguration(notesConfiguration);
            ApplyControlsConfigurations(controlsConfigurations);
            for (int i = 0; i < columns.Length; i++) SetColumnState(i, "blank");
        }

        public void Update()
        {
            for (int i = 0; i < 4; i++)
            {
                if (Input.GetKeyDown(input[i])) SetColumnState(i, "press");
                if (Input.GetKeyUp(input[i])) SetColumnState(i, "blank");
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

            for (int noteIndex = noteTrack.notes.Length - 1; noteIndex >= 0; noteIndex--)
            {
                if (noteTrack.notes[noteIndex].pressed) continue;

                float hitOffset = (float)Music.Position - noteTrack.notes[noteIndex].delay;
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