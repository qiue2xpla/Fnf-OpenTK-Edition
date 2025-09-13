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
        List<Action> updateList;
        List<Action> renderList;

        string[] tracks;
        string weekName;

        int currentTrack;
  
        public PlayMode(string weekName, string[] tracks)
        {
            elements = new();
            updateList = new();
            renderList = new();
            this.weekName = weekName;
            this.tracks = tracks;
        }

        void Start()
        {
            SetupStage(tracks[currentTrack]);
        }

        void Update()
        {
            for (int i = 0; i < updateList.Count; i++) updateList[i].Invoke();
            SharedGameSystems.VolumeControl.Update();

            if(Input.GetKeyDown(Key.Escape))
            {
                Active = new StoryMenu();
            }
        }

        void Render()
        {
            for (int i = 0; i < renderList.Count; i++) renderList[i].Invoke();
            SharedGameSystems.VolumeControl.Render();
        }

        void SetupStage(string track)
        {
            string[] stageLines = File.ReadAllLines($"Assets/Songs/{track}/stage.txt");
            for (int i = 0; i < stageLines.Length; i++)
            {
                if (stageLines[i].Trim() == "[Stage]") // The arguments is in the format of (Type typeName arg1 "arg2" etc.)
                {
                    i++; // Go to the next element after the declaration of the stage section
                    // Make sure we are in range and didn't go beyond the next declaration
                    while (i < stageLines.Length && !stageLines[i].Trim().StartsWith("[")) 
                    {
                        if (string.IsNullOrWhiteSpace(stageLines[i])) { i++; continue; } // If an empty line appears, skip it
                        string[] elementArgs = GetSegments(stageLines[i++], 2);
                        switch(elementArgs[0]) // Here is the implemented element types
                        {
                            case "Image":
                                Image image = new Image(elementArgs[2]);
                                elements.Add(elementArgs[1], image);
                                renderList.Add(image.Render);
                                break;
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
                        string[] layoutArgs = GetSegments(stageLines[i++], 1);
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
            }
        }

        // Splits a line thats seperated with spaces into segments while allowing
        // some segments to be combined with the spaces between then using quotes
        string[] GetSegments(string element, int allowStringAfterTheseArgs)
        {
            List<string> args = new();
            string currentArg = "";
            bool inString = false;

            for (int i = 0; i < element.Length; i++)
            {
                if (element[i] == ' ')
                {
                    if(inString && args.Count >= allowStringAfterTheseArgs)
                    {
                        currentArg += ' ';
                    }
                    else if (currentArg.Length > 0)
                    {
                        args.Add(currentArg);
                        currentArg = "";
                    }
                }
                else
                {
                    if (element[i] == '"')
                    {
                        if (args.Count < allowStringAfterTheseArgs) throw new InvalidDataException("A type or its name can't be loaded as a string");
                        inString = !inString;
                    }
                    else
                    {
                        currentArg += element[i];
                    }
                }
            }

            if (currentArg.Length > 0) args.Add(currentArg);
            if (inString) throw new InvalidDataException("A string was not ended");

            return args.ToArray();
        }
    }
}