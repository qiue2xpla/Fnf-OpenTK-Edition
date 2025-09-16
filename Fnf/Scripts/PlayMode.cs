using System.Collections.Generic;
using System.IO;
using System;

using Fnf.Framework;
using Fnf.Game;

namespace Fnf
{
    public class PlayMode : Script
    {
        Dictionary<string, MovableObject> elements;
        List<(Vector2 factor, MovableObject parent)> parallaxLayers;
        List<Action> updateList;
        List<Action> renderList;

        string[] tracks;
        string weekName;

        int currentTrack;

        Vector2 cameraTarget;
        Vector2 camera;

        public PlayMode(string weekName, string[] tracks)
        {
            elements = new();
            updateList = new();
            renderList = new();
            parallaxLayers = new();
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
            for (int i = 0; i < updateList.Count; i++) updateList[i].Invoke();
            SharedGameSystems.VolumeControl.Update();

            cameraTarget = Input.GetGridMousePosition();
            camera = MathUtility.Lerp(camera, cameraTarget, Time.deltaTime * 8);

            for (int i = 0; i < parallaxLayers.Count; i++)
            {
                MovableObject p = parallaxLayers[i].parent;
                Vector2 effectValue = parallaxLayers[i].factor;
                p.localPosition = camera * effectValue * new Vector2(-1);
            }

            if(Input.GetKeyDown(Key.Escape))
            {
                Active = new StoryMenu();
            }

            Music.Update();
        }

        void Render()
        {
            for (int i = 0; i < renderList.Count; i++) renderList[i].Invoke();
            SharedGameSystems.VolumeControl.Render();
        }

        void OnBeat(int beat)
        {
            foreach (var pair in elements)
            {
                if (pair.Value is Character character)
                {
                    character.Idle();
                }
            }
        }

        void SetupStage(string track)
        {
            string[] stageLines = File.ReadAllLines($"{GamePaths.Songs}/{track}/stage.txt");
            for (int i = 0; i < stageLines.Length; i++)
            {
                if (stageLines[i].Trim() == "[Stage]") // The arguments is in the format of (Type typeName arg1 "arg2" etc.)
                {
                    i++; // Go to the next element after the declaration of the stage section
                    // Make sure we are in range and didn't go beyond the next declaration
                    while (i < stageLines.Length && !stageLines[i].Trim().StartsWith("[")) 
                    {
                        if (string.IsNullOrWhiteSpace(stageLines[i])) { i++; continue; } // If an empty line appears, skip it
                        string[] elementArgs = StringUtility.Segment(stageLines[i++], 2);
                        switch(elementArgs[0]) // Here is the implemented element types
                        {
                            case "Image":
                                Image image = new Image(elementArgs[2]);
                                elements.Add(elementArgs[1], image);
                                renderList.Add(image.Render);
                                break;

                            case "Character":
                                Character character = new Character(elementArgs[2]);
                                elements.Add(elementArgs[1], character);
                                renderList.Add(character.Render);
                                updateList.Add(character.Update);
                                break;

                            case "Conductor":
                                Conductor conductor = new Conductor(elementArgs[2], elementArgs[3]);
                                elements.Add(elementArgs[1], conductor);
                                renderList.Add(conductor.Render);
                                updateList.Add(conductor.Update);
                                break;

                            default: throw new InvalidDataException($"'{elementArgs[0]}' is not a valid element");
                        }
                    }
                    i--; // When we return to the loop, the i++ in the loop is executed making us not able to reach the next decleration
                } 
                else if (stageLines[i].Trim() == "[Layout]")
                {
                    i++;
                    while (i < stageLines.Length && !stageLines[i].Trim().StartsWith("["))
                    {
                        if (string.IsNullOrWhiteSpace(stageLines[i])) { i++; continue; }
                        string[] layoutArgs = StringUtility.Segment(stageLines[i++], 1);
                        MovableObject targetObject = elements[layoutArgs[0]];

                        if (layoutArgs.Length == 7) // With parent
                        {
                            targetObject.parent = elements[layoutArgs[1]];
                            targetObject.localPosition = new Vector2(parse(2), parse(3));
                            targetObject.localRotation = parse(4);
                            targetObject.localScale = new Vector2(parse(5), parse(6));
                        }
                        else if (layoutArgs.Length == 6) // Without parent
                        {
                            targetObject.localPosition = new Vector2(parse(1), parse(2));
                            targetObject.localRotation = parse(3);
                            targetObject.localScale = new Vector2(parse(4), parse(5));
                        }
                        else
                        {
                            throw new ArgumentException($"Layout does not use {layoutArgs.Length} args");
                        }

                        float parse(int element) => float.Parse(layoutArgs[element]);
                    }
                    i--;
                }
                else if (stageLines[i].Trim() == "[Parallax]")
                {
                    i++;
                    while (i < stageLines.Length && !stageLines[i].Trim().StartsWith("["))
                    {
                        if (string.IsNullOrWhiteSpace(stageLines[i])) { i++; continue; }
                        string[] paralaxArgs = StringUtility.Segment(stageLines[i++], 1);
                        MovableObject targetObject = elements[paralaxArgs[0]];

                        Vector2 factor = Vector2.Zero;
                        if (paralaxArgs.Length == 2) // One value initiate
                        {
                            factor = new Vector2(parse(1));
                        }
                        else if (paralaxArgs.Length == 3) // Two value initiate
                        {
                            factor = new Vector2(parse(1), parse(2));
                        }
                        else
                        {
                            throw new ArgumentException($"Paralax does not use {paralaxArgs.Length} args");
                        }

                        MovableObject paralaxLayer = null; 
                        for (int a = 0; a < parallaxLayers.Count; a++)
                        {
                            if (parallaxLayers[a].factor == factor)
                            {
                                paralaxLayer = parallaxLayers[a].parent;
                                break;
                            }
                        }

                        if (paralaxLayer == null)
                        {
                            paralaxLayer = new MovableObject();
                        }

                        parallaxLayers.Add((factor, paralaxLayer));

                        targetObject.parent = paralaxLayer;

                        float parse(int element) => float.Parse(paralaxArgs[element]);
                    }
                    i--;
                }
            }
        }
    }
}