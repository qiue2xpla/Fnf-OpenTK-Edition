using Newtonsoft.Json;

namespace Fnf.Game
{
    internal class SongJson
    {
        [JsonProperty("player2")]
        public string Opponent;

        [JsonProperty("player1")]
        public string Player;

        [JsonProperty("speed")]
        public long ChartSpeed;

        [JsonProperty("song")]
        public string Name;

        [JsonProperty("notes")]
        public SectionJson[] Sections;

        [JsonProperty("bpm")]
        public float BeatsPerMinute;

        /*[JsonProperty("sections")]
        public long Sections;
        [JsonProperty("sectionLengths")]
        public object[] SectionLengths;
        [JsonProperty("needsVoices")]
        public bool NeedsVoices;*/
    }
}