using OpenTK.Graphics.OpenGL;
using Fnf.Framework;
using Fnf.Game;
using Fnf.Framework.Graphics;

public class Test : Script
{
    Animator animator;
    Character character;
    Image background;

    PlayerConveyor player;
    OpponentConveyor opponent;
    ControlsSkin controlsSkin;
    NotesSkin notesSkin;

    void Start()
    {
        TextureAtlas.LoadAtlas("op", "Assets/Characters/OppositonExpunged");
        TextureAtlas.LoadAtlas("notes", "Assets/Notes/Default");

        animator = new Animator();
        animator.add("idle", TextureAtlas.GetAnimation("op", "OppoExpungedIdle"));
        animator.play("idle");
        character = new Character();



        background = new Image("Assets/Background/metabg.png");

        Music.LoadSong("Unknown Suffering", 0.5f);
        Music.Play();

        GL.PointSize(10);

        controlsSkin = new ControlsSkin()
        {
            Blank = TextureAtlas.GetAnimations("notes", "arrowLEFT", "arrowDOWN", "arrowUP", "arrowRIGHT"),
            Press = TextureAtlas.GetAnimations("notes", "left press", "down press", "up press", "right press"),
            Confirm = TextureAtlas.GetAnimations("notes", "left confirm", "down confirm", "up confirm", "right confirm")
        };

        controlsSkin.ColumnsSpacing = controlsSkin.Blank[0].frames[0].verts[0].x * 2 + 5;

        for (int i = 0; i < 4; i++)
        {
            controlsSkin.Blank[i].looped = false;
            controlsSkin.Press[i].looped = false;
            controlsSkin.Confirm[i].looped = false;
        }


        notesSkin = new NotesSkin()
        {
            Note = TextureAtlas.GetAnimations("notes", "purple", "blue", "green", "red"),
            Hold = TextureAtlas.GetAnimations("notes", "purple hold piece", "blue hold piece", "green hold piece", "red hold piece"),
            End = TextureAtlas.GetAnimations("notes", "pruple end hold", "blue hold end", "green hold end", "red hold end")
        };

        for (int i = 0; i < 4; i++)
        {
            notesSkin.Note[i].looped = false;
            notesSkin.Hold[i].looped = false;
            notesSkin.End[i].looped = false;
        }
        

        ConveyorBase.CurrentBeatmap = new Beatmap("Unknown Suffering", "hard", new NoteParser());

        opponent = new OpponentConveyor(controlsSkin, notesSkin);
        opponent.localPosition = new Vector2(350, 680);
        opponent.localScale = new Vector2(0.75f);

        player = new PlayerConveyor(controlsSkin, notesSkin);
        player.localPosition = new Vector2(-350, 680);
        player.localScale = new Vector2(0.75f);

        //player.columns[0].animator.localScale.x = 0.5f;
        //player.columns[0].animator.localRotation = -45;

        //player.columns[2].animator.localPosition.y -= 100;
        //player.columns[2].animator.localRotation = -135;
        //player.columns[2].animator.localScale.y = 0.1f;

        //player.columns[3].animator.localRotation += -90;

        //player.localRotation = 45;

        player.botPlay = true;
    }

    void Update()
    {
        animator.Update();
        character.Update();
        character.localPosition = new Vector2(500, 500);
        bool miss = Input.GetAnyKeys(Key.LShift, Key.RShift);
        /*if (Input.GetKeyDown(Key.Q)) character.Hit(3, miss);
        if (Input.GetKeyDown(Key.W)) character.Hit(2, miss);
        if (Input.GetKeyDown(Key.BracketLeft)) character.Hit(1, miss);
        if (Input.GetKeyDown(Key.BracketRight)) character.Hit(0, miss);*/

        if (Input.GetKeyDown(Key.Space)) Music.Position += 10;

        player.Update();
        opponent.Update();
    }

    void Render()
    {
        background.Render();
        var position = Input.GetGridMousePosition();
        var v22 = new Vector2(position.x, position.y);
        animator.Render();
        RenderCursor();

        character.Render();


        opponent.Render();
        player.Render();
    }

    void RenderCursor()
    {
        GL.BindTexture(TextureTarget.Texture2D,0);
        GL.Begin(PrimitiveType.Points);
        GL.Color3(1f, 0f, 0f);
        var position = Input.GetGridMousePosition();
        OpenGL.Pixel2(position.x, position.y);
        GL.End();
    }
}