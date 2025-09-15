using Fnf.Framework;

namespace Fnf.Game
{
    public class Conductor : MovableObject
    {
        public float[] holdCooldown;
        public float[] hitCooldown;
        public float noteSpeed = 2.5f; // Note's distance per second is (screenHight * noteSpeed)

        public Animator[] columns;

        public Conductor()
        {
            /*float offset = controlsSkin.ColumnsSpacing * -1.5f;
            columns = new NoteColumn[4];
            for (int i = 0; i < 4; i++)
            {
                float x = controlsSkin.ColumnsSpacing * i + offset;
                columns[i] = new NoteColumn();

                columns[i].animator = new Animator();
                columns[i].animator.parent = this;
                columns[i].animator.localPosition = new Vector2(x, 0);

                columns[i].animator.add("blank", controlsSkin.Blank[i]);
                columns[i].animator.add("press", controlsSkin.Press[i]);
                columns[i].animator.add("confirm", controlsSkin.Confirm[i]);
            }*/
        }
    }
}