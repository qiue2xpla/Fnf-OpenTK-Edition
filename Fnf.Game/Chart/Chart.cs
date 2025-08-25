using System.Collections.Generic;

namespace Fnf.Game
{
    public class Chart
    {
        public NoteData[] notes;

        public Chart(Beatmap beatmap, bool isPlayer)
        {
            List<NoteData> list = new List<NoteData>();
            for (int i = 0; i < beatmap.notes.Length; i++)
            {
                if (beatmap.notes[i].player == isPlayer)
                {
                    list.Add(new NoteData(beatmap.notes[i]));
                }
            }
            notes = list.ToArray();
        }

        public class NoteData
        {
            public float delay;
            public float length;
            public int column;
            public string type;
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
}