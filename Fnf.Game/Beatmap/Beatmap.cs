using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Fnf.Game
{
    /// <summary>
    /// Main source of beatmap data
    /// </summary>
    public class Beatmap
    {
        /// <summary>
        /// Name of the beatmap, set by the object's constructor
        /// </summary>
        public readonly string name;

        /// <summary>
        /// Difficulty of the beatmap, set by the object's constructor
        /// </summary>
        public readonly string difficulty;

        /// <summary>
        /// Beats per minute of the beatmap, loaded from the beatmap file
        /// </summary>
        public readonly float beatsPerMinute;

        /// <summary>
        /// Note data of the beatmap, loaded from the beatmap file
        /// </summary>
        public readonly NoteBase[] notes;

        public readonly string[] noteTypes;

        /// <summary>
        /// Loads beatmap data from a json file
        /// </summary>
        public Beatmap(string name, string difficulty, NoteParser parser)
        {
            this.name = name;
            this.difficulty = difficulty;

            string jsonContent = File.ReadAllText($"{GamePaths.Songs}/{name}/{difficulty}.json");
            BeatmapJson beatmapJson = BeatmapJson.LoadBeatmap(jsonContent);

            beatsPerMinute = beatmapJson.songData.BeatsPerMinute;

            List<NoteBase> notesList = new List<NoteBase>();
            List<string> noteTypesList = new List<string>();
            foreach (SectionJson section in beatmapJson.songData.Sections)
            {
                foreach (object[] noteData in section.Notes)
                {
                    NoteBase note = parser.Parse(noteData, section.MustHitSection);
                    if (note == null) continue;
                    notesList.Add(note);
                    if (!noteTypesList.Contains(note.type)) 
                        noteTypesList.Add(note.type);
                }
            }

            noteTypes = noteTypesList.ToArray();
            notes = notesList.OrderBy(x => x.delay).ToArray();
        }

        /// <summary>
        /// Load notes from the beatmap that match the target
        /// </summary>
        public NoteData[] GetTargetNotes(string target)
        {
            return notes.Where(n => { return n.target == target; }).Select((n) => { return new NoteData(n); }).ToArray();
        }
    }

    /// <summary>
    /// Main note data, used by <seealso cref="Beatmap">Beatmap</seealso>
    /// </summary>
    public class NoteBase
    {
        public float delay;
        public float length;
        public int column;
        public string type;
        public string target;
    }

    /// <summary>
    /// Contains extra data for the <seealso cref="Conductor"/>
    /// to render the notes correctly
    /// </summary>
    public class NoteData : NoteBase
    {
        public NoteState state;
        public float holdProgress;

        public NoteData(NoteBase note)
        {
            delay = note.delay;
            length = note.length;
            column = note.column;
            type = note.type;
            holdProgress = length;
        }
    }

    public enum NoteState
    {
        None,
        Miss,
        Bad,
        Good,
        Perfect,
        Bot,
    }

}