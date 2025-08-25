using System;

namespace Fnf.Game
{
    public class NoteParser
    {
        protected const int DelayIndex = 0;
        protected const int ColumnIndex = 1;
        protected const int LengthIndex = 2;
        protected const int NoteTypeIndex = 3; //if exists

        public virtual Note Parse(object[] data, bool mustHitSection)
        {
            if (Convert.ToInt32(data[ColumnIndex]) < 0) return null;

            Note note = new Note();
            note.delay = Convert.ToInt32(data[DelayIndex]) / 1000f;
            note.column = Convert.ToInt32(data[ColumnIndex]);
            note.length = Convert.ToInt32(data[LengthIndex]) / 1000f;
            note.player = mustHitSection == note.column % 8 < 4;
            note.type = "Default";

            if (data.Length > 3) note.type = Convert.ToString(data[NoteTypeIndex]);
            if (note.column > 7 && note.type == "Default") note.type = "Odd";

            note.column = note.column % 4;
            return note;
        }
    }
}