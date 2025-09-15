using Fnf.Framework;

namespace Fnf.Game
{
    public class OpponentConveyor : ConveyorBase
    {
        /*private float[] hitTimes = new float[4];
        private int botReachedNote = 0;

        public OpponentConveyor(ControlsSkin controlsSkin, NotesSkin notesSkin) : base(controlsSkin, notesSkin)
        {
            chart = new Chart(CurrentBeatmap, false);
        }

        public override void Update()
        {
            base.Update();

            for (int i = botReachedNote; i < chart.notes.Length; i++)
            {
                var note = chart.notes[i];
                int column = note.column;
                float delay = (float)Music.Position - note.delay;

                if (delay >= 0)
                {
                    if (note.pressed && note.length == 0) continue;

                    if (note.length > 0)
                    {
                        if(delay > note.length)
                        {
                            note.hold = 1;
                            if (i - botReachedNote == 0) botReachedNote++;
                            continue;
                        }

                        if(note.pressed)
                        {
                            note.hold = delay / note.length;

                            if (hitTimes[column] == 0)
                            {
                                hitTimes[column]= 0.1f;
                                columns[column].PressCooldown = 0.1f;
                                Confirm(column);
                                Hit(column);
                            }
                        }
                        else
                        {
                            note.pressed = true;
                            columns[column].PressCooldown = 0.1f;
                            Confirm(column);
                            Hit(column);
                        }
                    }
                    else
                    {
                        note.pressed = true;
                        columns[column].PressCooldown = 0.1f;
                        Confirm(column);
                        Hit(column);
                        if(i - botReachedNote == 0) botReachedNote++;
                    }
                }
                else break;
            }

            for (int i = 0; i < 4; i++)
            {
                bool HasBeenPressed = columns[i].PressCooldown > 0;
                columns[i].PressCooldown -= Time.deltaTime;
                if (columns[i].PressCooldown < 0)
                {
                    columns[i].PressCooldown = 0;
                    if (HasBeenPressed) Release(i);
                }

                hitTimes[i] -= Time.deltaTime;
                if (hitTimes[i] < 0)
                {
                    hitTimes[i] = 0;
                }
            }
        }*/
    }
}