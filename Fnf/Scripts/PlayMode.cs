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
        StageContext stageContext;
        public PlayMode(string weekName, string difficulty, string[] tracks)
        {
            stageContext = new StageContext(weekName, difficulty, tracks);
        }

        void Render()
        {
            stageContext.Render();
        }

        void Update()
        {
            stageContext.Update();
        }
    }
}