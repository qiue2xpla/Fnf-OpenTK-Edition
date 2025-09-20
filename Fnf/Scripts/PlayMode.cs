using System.Collections.Generic;
using System.IO;
using System;

using Fnf.Framework;
using Fnf.Game;
using System.Security.Policy;

namespace Fnf
{
    public class PlayMode : Script
    {
        public static Dictionary<string, MovableObject> Elements;
        public static List<(Vector2 factor, MovableObject parent)> ParallaxLayers;
        public static List<Action> UpdateList;
        public static List<IRenderable> RenderList;
        public static Beatmap Beatmap;

        string[] tracks;
        string difficulty;
        string weekName;
        int currentTrack;

        Vector2 cameraTarget;
        Vector2 camera;

        public PlayMode(string weekName, string difficulty, string[] tracks)
        {
            Elements = new();
            UpdateList = new();
            RenderList = new();
            ParallaxLayers = new();
            this.difficulty = difficulty;
            this.weekName = weekName;
            this.tracks = tracks;
        }

        void Start()
        {
            SetupStage(tracks[currentTrack]);

            Music.OnBeatHit += OnBeat;
        }

        void Update()
        {
            for (int i = 0; i < UpdateList.Count; i++) UpdateList[i].Invoke();
            SharedGameSystems.VolumeControl.Update();

            cameraTarget = Input.GetGridMousePosition();
            camera = MathUtility.Lerp(camera, cameraTarget, Time.deltaTime * 8);

            for (int i = 0; i < ParallaxLayers.Count; i++)
            {
                MovableObject p = ParallaxLayers[i].parent;
                Vector2 effectValue = ParallaxLayers[i].factor;
                p.localPosition = camera * effectValue * new Vector2(-1);
            }

            if(Input.GetKeyDown(Key.Escape))
            {
                Active = new StoryMenu();
            }

            if (Input.GetKeyDown(Key.Right))
            {
                Music.Position += 10;
            }

            if (Input.GetKeyDown(Key.F2))
            {
                SetupStage(tracks[currentTrack]);
            }    

            Music.Update();
        }

        void Render()
        {
            for (int i = 0; i < RenderList.Count; i++) RenderList[i].Render();
            SharedGameSystems.VolumeControl.Render();
        }

        void OnBeat(int beat)
        {
            foreach (var pair in Elements)
            {
                if (pair.Value is Character character)
                {
                    character.Idle();
                }
            }
        }

        void SetupStage(string track)
        {
            Beatmap = new Beatmap(track, difficulty, new NoteParser());

            Music.LoadSong(track);
            Music.Play();

            Elements.Clear();
            UpdateList.Clear();
            RenderList.Clear();
            ParallaxLayers.Clear();

            var Sections = StringUtility.SplitSections(File.ReadAllLines($"{GamePaths.Songs}/{track}/stage.txt"));
            foreach ((string Section, string[] Entries) in Sections)
            {
                switch (Section)
                {
                    case "Stage": // Data format is as follows: ObjectType objectName arg1 "arg2" etc...
                    {
                        for (int i = 0; i < Entries.Length; i++)
                        {
                            string[] entryValues = StringUtility.SplitValues(Entries[i], 2);

                            string objectType = entryValues[0];
                            string objectName = entryValues[1];

                            switch (objectType) // The implemented object types
                            {
                                case "Image":
                                {
                                    string imagePath = entryValues[2];

                                    Image image = new Image(imagePath);

                                    Elements.Add(objectName, image);
                                    break;
                                }
                                case "Character":
                                {
                                    string characterConfiguration = entryValues[2];

                                    Character character = new Character(characterConfiguration);

                                    Elements.Add(objectName, character);
                                    UpdateList.Add(character.Update);
                                    break;
                                }

                                case "Conductor":
                                {
                                    string controlsConfig = entryValues[2];
                                    string notesConfig = entryValues[3]; // TODO: huh
                                    string targetNotes = entryValues[4];
                                    string targetCharacter = entryValues[5];

                                    NoteTrack noteTrack = new NoteTrack(Beatmap, targetNotes);
                                    Conductor conductor = new Conductor(controlsConfig, notesConfig, noteTrack, targetNotes == "player");

                                    conductor.targetCharacter = Elements[targetCharacter] as Character;

                                    Elements.Add(objectName, conductor);
                                    UpdateList.Add(conductor.Update);
                                    break;
                                }

                                default: throw new InvalidDataException($"'{objectType}' is not a valid object type");
                            }
                        }
                        break;
                    }
                    case "RenderOrder":
                    {
                        for (int i = 0; i < Entries.Length; i++)
                        {
                            string[] entryValues = StringUtility.SplitValues(Entries[i], 1);

                            string targetObject = entryValues[0];

                            if (Elements[targetObject] is IRenderable renderable)
                            {
                                RenderList.Add(renderable);
                            }
                        }
                        break;
                    }
                    case "Layout": // ObjectName parentName(Optional) x y rotation sizeX sizeY
                    {
                        for (int i = 0; i < Entries.Length; i++)
                        {
                            string[] entryValues = StringUtility.SplitValues(Entries[i], 1);

                            string targetName = entryValues[0];
                            int indexOffset = 0;

                            MovableObject targetObject = Elements[targetName];

                            if (entryValues.Length == 7)
                            {
                                indexOffset = 1;
                                string parentName = entryValues[1];
                                targetObject.parent = Elements[parentName];
                            }
                            else if (entryValues.Length != 6) new InvalidDataException($"Layout does not use '{entryValues.Length}' args");

                            float parse(int element) => float.Parse(entryValues[element + indexOffset]);
                            float x = parse(1);
                            float y = parse(2);
                            float rotation = parse(3);
                            float sizeX = parse(4);
                            float sizeY = parse(5);

                            targetObject.localPosition = new Vector2(x, y);
                            targetObject.localRotation = rotation;
                            targetObject.localScale = new Vector2(sizeX, sizeY);
                        }
                        break;
                    }
                    case "Parallax": // ObjectName factor(one value or two for xy)
                    {
                        for (int i = 0; i < Entries.Length; i++)
                        {
                            string[] entryValues = StringUtility.SplitValues(Entries[i], 1);

                            string targetName = entryValues[0];

                            MovableObject targetObject = Elements[targetName];
                            MovableObject paralaxLayer = null;
                            Vector2 factor = Vector2.Zero;

                            if (entryValues.Length == 2) factor = new Vector2(parse(1));
                            else if (entryValues.Length == 3) factor = new Vector2(parse(1), parse(2));
                            else throw new InvalidDataException($"Paralax does not use '{entryValues.Length}' args");

                            for (int a = 0; a < ParallaxLayers.Count; a++)
                            {
                                if (ParallaxLayers[a].factor == factor)
                                {
                                    paralaxLayer = ParallaxLayers[a].parent;
                                    break;
                                }
                            }

                            if (paralaxLayer == null)
                            {
                                paralaxLayer = new MovableObject();
                                ParallaxLayers.Add((factor, paralaxLayer));
                            }

                            targetObject.parent = paralaxLayer;

                            float parse(int element) => float.Parse(entryValues[element]);
                        }
                        break;
                    }
                }
            }
        }
    }
}