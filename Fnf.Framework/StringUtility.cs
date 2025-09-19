using System;
using System.Collections.Generic;
using System.IO;

namespace Fnf.Framework
{
    /// <summary>
    /// Contains code to make working with readable-data simple and easy
    /// </summary>
    public static class StringUtility
    {
        public static string IncertAt(string text, string incertTarget, string textToIncert)
        {
            if (textToIncert.Contains(incertTarget)) throw new StackOverflowException("textToIncert should not contain incertTarget!"); 
            
            int indexOfTarget = text.IndexOf(incertTarget);

            while (indexOfTarget >= 0)
            {
                text = text.Substring(0, indexOfTarget) + textToIncert + 
                    text.Substring(indexOfTarget + incertTarget.Length, 
                    text.Length - indexOfTarget - incertTarget.Length);
                indexOfTarget = text.IndexOf(incertTarget);
            }

            return text;
        }

        /// <summary>
        /// Splits sections and catagorize its enries and removes comment lines
        /// </summary>
        public static (string Section, string[] Entries)[] SplitSections(string[] lines)
        {
            List<(string, string[])> result = new List<(string, string[])>();
            List<string> currentSectionEntries = new List<string>();
            string currentSection = null;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (line.Contains("//"))
                {
                    // Remove comment line
                    int commentStartPoint = line.IndexOf("//");
                    line = line.Substring(0, commentStartPoint).Trim();
                }
                if (line.StartsWith("[") && line.EndsWith("]")) // Section declaration line
                {
                    if(currentSectionEntries.Count > 0) // There is loaded data
                    {
                        result.Add((currentSection, currentSectionEntries.ToArray()));
                    }
                    currentSection = line.Substring(1, line.Length - 2);
                    currentSectionEntries.Clear();
                }
                else // Data line
                {
                    if (currentSection == null) throw new InvalidDataException("Data does not fall under a section");
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    currentSectionEntries.Add(line);
                }
            }

            if (currentSectionEntries.Count > 0) // There is loaded data
            {
                result.Add((currentSection, currentSectionEntries.ToArray()));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Splits values that is seperated with spaces while allowing some
        /// values to be combined with the spaces between them using quotes
        /// </summary>
        /// <param name="text">The text to segment</param>
        /// <param name="allowQuotesAfterTheseValues">The amount of args that can't be in quotes from start</param>
        public static string[] SplitValues(string text, int allowQuotesAfterTheseValues = 0)
        {
            List<string> segments = new List<string>();
            string currentSegment = "";
            bool inQuotes = false;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                {
                    if (inQuotes && segments.Count >= allowQuotesAfterTheseValues)
                    {
                        currentSegment += ' ';
                    }
                    else if (currentSegment.Length > 0)
                    {
                        segments.Add(currentSegment);
                        currentSegment = "";
                    }
                }
                else
                {
                    if (text[i] == '"')
                    {
                        if (segments.Count < allowQuotesAfterTheseValues) 
                            throw new InvalidDataException("A type or its name can't be loaded as a string");
                        inQuotes = !inQuotes;
                    }
                    else
                    {
                        currentSegment += text[i];
                    }
                }
            }

            if (currentSegment.Length > 0) segments.Add(currentSegment);
            if (inQuotes) throw new InvalidDataException("A string was not ended");

            return segments.ToArray();
        }
    }
}