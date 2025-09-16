using System.Collections.Generic;
using System.IO;

namespace Fnf.Framework
{
    public static class StringUtility
    {
        /// <summary>
        /// Splits a line thats seperated with spaces into segments while allowing
        /// some segments to be combined with the spaces between them using quotes
        /// </summary>
        /// <param name="text">The text to segment</param>
        /// <param name="allowQuotesAfterTheseArgs">The amount of args that can't be in quotes from start</param>
        public static string[] Segment(string text, int allowQuotesAfterTheseArgs)
        {
            List<string> segments = new List<string>();
            string currentSegment = "";
            bool inQuotes = false;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                {
                    if (inQuotes && segments.Count >= allowQuotesAfterTheseArgs)
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
                        if (segments.Count < allowQuotesAfterTheseArgs) 
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