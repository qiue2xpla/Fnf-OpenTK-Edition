using System.Collections.Generic;

namespace Fnf.Game
{
    /// <summary>
    /// Contains a collection of <seealso cref="NoteData"/> for 
    /// the <seealso cref="Conductor"/> to use and utilize
    /// </summary>
    public class NoteTrack // TODO: Delete the class and use the notes only
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
        public NoteState state;
        public float holdProgress;

        public NoteData(Note note)
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