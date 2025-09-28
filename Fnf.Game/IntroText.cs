using System.Collections.Generic;
using System.IO;
using System;
using Fnf.Framework;
using System.Linq;

namespace Fnf.Game
{
    // Can be made cooler but iam making 
    // it so it looks like the original
    // + doesnt support the triple line
    // random text for now and it never will
    public class IntroText : IRenderable
    {
        public bool isRenderable { get; set; } = true;

        FnfText fnfText;
        string[] currentText = new string[0];
        int index = 0;

        string[] currentRandomLine = new string[0];
        string[] allRandomLines;
        List<int> notUsed = new List<int>(0);


        public IntroText()
        {
            fnfText = new FnfText();
            allRandomLines = File.ReadAllLines($"{GamePaths.Shared}/IntroText.txt");
            for (int i = 0; i < allRandomLines.Length; i++) notUsed.Add(i);
        }

        public void Render()
        {
            fnfText.Render();
        }

        public void Clear(int linesCount)
        {
            // Make sure the list is not empty
            if (notUsed.Count == 0) for (int i = 0; i < allRandomLines.Length; i++) notUsed.Add(i);

            // Select a random text index and remove it from list
            int randomSelectedIndex = notUsed[RNG.Next(0, notUsed.Count)];
            notUsed.Remove(randomSelectedIndex);

            // Set the current line with the new one
            currentRandomLine = allRandomLines[randomSelectedIndex].Split('%');

            index = 0;
            currentText = new string[linesCount];
            RefreshFnfText();
        }

        // Use carefully because the order can be messed up
        public void AddRandom()
        {
            // Do nothing if its filled
            if (index >= currentText.Length) return;

            currentText[index] = currentRandomLine[index];
            index++;

            RefreshFnfText();
        }

        public void AddText(string text)
        {
            if (index >= currentText.Length) return;

            currentText[index++] = text;

            RefreshFnfText();
        }

        void RefreshFnfText()
        {
            string text = "";

            for (int i = 0; i < currentText.Length; i++)
            {
                if (currentText[i] == "")
                {
                    text += " ";
                }
                else
                {
                    text += currentText[i];
                }

                text += "\n";
            }

            fnfText.text = text;
        }
    }
}