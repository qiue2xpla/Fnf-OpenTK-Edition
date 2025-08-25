using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Fnf.Game
{
    public class Beatmap
    {
        public readonly string name;
        public readonly string difficulty;
        public readonly float beatsPerMinute;

        public readonly Note[] notes;
        public readonly string[] noteTypes;

        public Beatmap(string name, string difficulty, NoteParser parser)
        {
            string jsonPath = $"Assets/Songs/{name}/{difficulty}.json";
            string json = File.ReadAllText(jsonPath);
            BeatmapJson beatmapJson = BeatmapJson.LoadBeatmap(json);

            this.name = name;
            this.difficulty = difficulty;
            beatsPerMinute = beatmapJson.songData.BeatsPerMinute;

            List<Note> notesList = new List<Note>();
            List<string> noteTypesList = new List<string>();

            foreach (SectionJson section in beatmapJson.songData.Sections)
            {
                foreach (object[] noteData in section.Notes)
                {
                    Note note = parser.Parse(noteData, section.MustHitSection);
                    if (note == null) continue;
                    notesList.Add(note);
                    if (!noteTypesList.Contains(note.type)) noteTypesList.Add(note.type);
                }
            }

            noteTypes = noteTypesList.ToArray();
            notes = notesList.OrderBy(x => x.delay).ToArray();
        }
    }
}