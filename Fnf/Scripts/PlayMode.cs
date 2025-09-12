using Fnf.Framework;

namespace Fnf
{
    public class PlayMode : Script
    {
        string[] tracks;
        string weekName;

        int currentTrack;
          
        public PlayMode(string weekName, string[] tracks)
        {
            this.weekName = weekName;
            this.tracks = tracks;
        }

        void Start()
        {
            SetupScene(tracks[currentTrack]);
        }

        void SetupScene(string track)
        {
            // TODO: Setup current track
        }
    }
}