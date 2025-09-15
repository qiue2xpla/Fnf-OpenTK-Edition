using Fnf.Framework;

namespace Fnf.Game
{
    public class PlayerConveyor : ConveyorBase
    {
        /*public static Key[] inputKeys = { Key.Q, Key.W, Key.BracketLeft, Key.BracketRight };
        public bool botPlay;

        private int botReachedNote = 0;


        public PlayerConveyor(ControlsSkin controlsSkin, NotesSkin notesSkin) : base (controlsSkin, notesSkin)
        {
            chart = new Chart(CurrentBeatmap, true);
        }

        public override void Update()
        {
            base.Update();

            if (botPlay)
            {
                for (int i = botReachedNote; i < chart.notes.Length; i++)
                {
                    var note = chart.notes[i];
                    float delay = note.delay - (float)Music.Position;

                    if (delay <= 0)
                    {
                        botReachedNote++;
                        note.pressed = true;
                        //Health.health += 3;
                        columns[note.column].PressCooldown = 0.1f;
                        Confirm(note.column);
                        Hit(note.column);
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
                }
            }
            else
            {
                /*for (int a = i; a < Chart.Length; a++)
                {
                    Note note = Chart[a];
                    float delay = (note.Delay / 1000) - BackMusic.Inst.time;

                    if (delay < -OD.Shit)
                    {
                        i++;
                        if (!note.Pressed)
                        {
                            Health.health -= 4;
                            OnHit.Invoke(note.Slot, delay, true, false);
                        }
                    }
                    else break;
                }*/

                /*for (int i = 0; i < 4; i++)
                {
                    if (Input.GetKeyDown(inputKeys[i])) Press(i);
                    if (Input.GetKeyUp(inputKeys[i])) Release(i);
                }
            }
        }
            /*
            int GetClosest(int slot)
            {
                int Index = -1;

                float ClosestTime = 9999;

                for (int i = UpperIndex; i < DownIndex + 1; i++)
                {
                    Note note = beatmap.Notes[side][i];

                    if (note.Slot == slot && !replay[i].Pressed)
                    {
                        float delay = Math.Abs(note.Delay - SongTime);

                        //if (delay <= OD.Shit && delay <= Delay)
                        if (delay <= 0.5f && delay <= ClosestTime)
                        {
                            ClosestTime = delay;
                            Index = i;
                        }
                    }
                }

                return Index;
            }*/
    }
}