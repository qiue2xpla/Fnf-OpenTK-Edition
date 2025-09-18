using Fnf.Framework;
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

        float[] holdCooldown;
        float[] hitCooldown;

        public Conductor(string controlsConfigurations, string notesConfiguration, NoteTrack track)
        {
            noteTrack = track; 
            ApplyControlConfigurations(controlsConfigurations);
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
        }

        void SetColumnState(int column, string state) => columns[column].play(state);

        public void ApplyControlConfigurations(string controlsConfigurations)
        {
            string[] configData = File.ReadAllLines($"{GamePaths.ControlsConfigurations}/{controlsConfigurations}.txt");
            string[] head = StringUtility.Segment(configData[0], 0);
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
                string[] args = StringUtility.Segment(configData[l], 1);
                for (int i = 0; i < columns.Length; i++)
                {
                    Animator animator = columns[i];
                    animator.add(args[0], TextureAtlas.GetAnimation("Config-" + controlsConfigurations, args[i + 1]));
                }
            }
        }
    }
}