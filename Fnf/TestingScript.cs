using Fnf.Framework.Graphics;
using Fnf.Framework;

public class TestingScript : Script
{
    GUI root;

    GUI child1;
    GUI child2;
    GUI child3;

    public TestingScript()
    {
        root = new GUI();
        child1 = new GUI();
        child2 = new GUI();
        child3 = new GUI();

        child1.parent = root;
        child2.parent = root;
        child3.parent = root;

        root.layoutMode = LayoutMode.HorizontalFlow;
        root.horizontalSizeMode = SizeMode.Fixed;
        root.verticalSizeMode = SizeMode.Auto;
        root.width = 600;
        root.gap = 5;
        root.padding = (5, 5, 5, 5);

        child1.width = 100;
        child1.height = 100;

        child2.horizontalSizeMode = SizeMode.Flexible;
        child2.verticalSizeMode = SizeMode.Flexible;

        child3.width = 70;
        child3.height = 50;
        child3.verticalAlignment = VerticalAlignment.Center;
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
        Gizmos.DrawRoundQuad(root, Color.Red, 4, 0);
        Gizmos.DrawRoundQuad(child1, Color.White, 4, 0);
        Gizmos.DrawRoundQuad(child2, Color.Blue, 4, 0);
        Gizmos.DrawRoundQuad(child3, Color.Green, 4, 0);
    }
}