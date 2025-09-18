using System.Collections.Generic;

namespace Fnf.Game
{
    /// <summary>
    /// Contains a collection of <seealso cref="NoteData"/> for 
    /// the <seealso cref="Conductor"/> to use and utilize
    /// </summary>
    public class NoteTrack
    {
        public NoteData[] notes;

        /// <summary>
        /// Load notes from the beatmap that match the target
        /// </summary>
        public NoteTrack(Beatmap beatmap, string target)
        {
            List<NoteData> list = new List<NoteData>();
            for (int i = 0; i < beatmap.notes.Length; i++)
            {
                if (beatmap.notes[i].target == target)
                {
                    list.Add(new NoteData(beatmap.notes[i]));
                }
            }
            notes = list.ToArray();
        }
    }

    /// <summary>
    /// Contains extra data for the <seealso cref="Conductor"/>
    /// to render the notes correctly
    /// </summary>
    public class NoteData :  Note
    {
        public bool pressed;
        public float hold;

        public NoteData(Note note)
        {
            delay = note.delay;
            length = note.length;
            column = note.column;
            type = note.type;
        }
    }
}