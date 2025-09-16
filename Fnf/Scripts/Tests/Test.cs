/*
        Music.LoadSong("Unknown Suffering", 0.5f);
        Music.Play();

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
*/