using System.Collections.Generic;
using Newtonsoft.Json;

namespace Fnf.Game
{
    internal class SectionJson
    {
        [JsonProperty("mustHitSection")]
        public bool MustHitSection;

        [JsonProperty("sectionNotes")]
        public List<object[]> Notes;

        /*[JsonProperty("lengthInSteps")]
        public long LengthInSteps;
        [JsonProperty("bpm")]
        public long? Bpm;
        [JsonProperty("changeBPM")]
        public bool? ChangeBpm;
        [JsonProperty("typeOfSection")]
        public long? TypeOfSection;*/
    }
}