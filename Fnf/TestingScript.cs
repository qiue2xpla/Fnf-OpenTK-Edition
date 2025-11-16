using Fnf.Framework.Graphics;
using Fnf.Framework;

public class TestingScript : Script
{
    Color[] idk = new Color[20];
    int index;

    GUI root;

    public TestingScript()
    {
        for (int i = 0; i < idk.Length; i++)
        {
            idk[i] = RandomColor();
        }

        root = new GUI()
        {
            gap = 5,
            padding = (5, 5, 5, 5),
            layoutMode = LayoutMode.HorizontalFlow,
            horizontalSizeMode = SizeMode.Auto,
            verticalSizeMode = SizeMode.Auto,
            verticalAlignment = VerticalAlignment.Top,
            horizontalAlignment = HorizontalAlignment.Left,
        };

        GUI child1 = new GUI()
        {
            parent = root,
            width = 100,
            height = 100,
            verticalAlignment = VerticalAlignment.Top
        };

        GUI child2 = new GUI()
        {
            parent = root,
            width = 200,
            height = 200,
            layoutMode = LayoutMode.VerticalFlow,
            gap = 5
        };


        GUI item1 = new GUI()
        {
            parent = child2,
            horizontalSizeMode = SizeMode.Flexible,
            verticalSizeMode = SizeMode.Flexible,
        };

        GUI item2 = new GUI()
        {
            parent = child2,
            horizontalSizeMode = SizeMode.Flexible,
            verticalSizeMode = SizeMode.Flexible,
        };

        GUI item3 = new GUI()
        {
            parent = child2,
            horizontalSizeMode = SizeMode.Flexible,
            verticalSizeMode = SizeMode.Flexible,
        };
    }

    void Update()
    {
        if(Input.GetKey(Key.Space))
        {
            root.width += Input.GetScrollWheelDelta() * 10;
        }
        else
        {
            root.height += Input.GetScrollWheelDelta() * 10;
        }

        root.UpdateLayout();
    }

    void Render()
    {
        float roundness = 3;

        index = 0;

        Draw(root);

        void Draw(GUI gui)
        {
            Gizmos.DrawRoundQuad(gui, idk[index], roundness, 0);
            index++;

            for (int i = 0; i < gui.children.Length; i++)
            {
                GUI child = gui.children[i] as GUI;
                Draw(child);
            }
        }
    }

    Color RandomColor()
    {
        int dep = 10;
        return new Color(random(dep), random(dep), random(dep));
        float random(int depth) => (float)RNG.Next(0, depth) / (depth - 1);
    }
}