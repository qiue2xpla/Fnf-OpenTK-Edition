using Newtonsoft.Json;

namespace Fnf.Game
{
    internal class BeatmapJson
    {
        [JsonProperty("song")]
        public SongJson songData;

        public static BeatmapJson LoadBeatmap(string json)
        {
            return JsonConvert.DeserializeObject<BeatmapJson>(json);
        }
    }
}