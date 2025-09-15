using Fnf.Framework.Audio;
using Fnf.Framework;
using System.IO;
using System;

namespace Fnf.Game
{
    public static class Music // Every section is 4 beats
    {
        public static event Action<int> OnBeatHit;
        public static int BeatsPerMinute;
        public static float BeatOffset;

        private static Clip current;
        public static Clip Instruments;
        public static Clip Voices_player;
        public static Clip Voices_opponent;

        private static int currentBeat, previousBeat;

        public static void Update()
        {
            currentBeat = PositionInBeats;

            if (currentBeat > previousBeat)
            {
                OnBeatHit?.Invoke(currentBeat);
            }

            previousBeat = currentBeat;
        }

        public static double Position
        {
            get
            {
                if (current == null) return 0;
                return current.position;
            }
            set
            {
                if (Instruments != null) Instruments.position = value;
                if (Voices_player != null) Voices_player.position = value;
                if (Voices_opponent != null) Voices_opponent.position = value;
            }
        }

        public static int PositionInBeats
        {
            get
            {
                float beatsPerSecond = (float)BeatsPerMinute / 60;
                return (int)Math.Floor((Position + BeatOffset) * beatsPerSecond);
            }
        }

        public static void LoadSong(string name, float volume = 1)
        {
            UnloadAll();

            string pathi = $"Assets/Songs/{name}/Inst.ogg";
            string pathvp = $"Assets/Songs/{name}/Voices.ogg";
            string pathvo = $"Assets/Songs/{name}/VoicesOpponent.ogg";

            if (File.Exists(pathi))
            {
                Instruments = new Clip(pathi) { volume = volume };
                current = Instruments;
            }

            if (File.Exists(pathvp))
            {
                Voices_player = new Clip(pathvp) { volume = volume };
                Voices_opponent = null;
                current = Voices_player;
            }
            else
            {
                pathvp = $"Assets/Songs/{name}/VoicesPlayer.ogg";
                if (File.Exists(pathvp))
                {
                    Voices_player = new Clip(pathvp) { volume = volume };
                    current = Voices_player;
                }
                if (File.Exists(pathvo))
                {
                    Voices_opponent = new Clip(pathvo) { volume = volume };
                    current = Voices_opponent;
                }
            }
        }

        public static void LoadMenuMusic(float volume = 1)
        {
            string path = $"{GamePaths.Sounds}/Menu.ogg";

            UnloadAll();

            if (File.Exists(path))
            {
                Instruments = new Clip(path) { volume = volume, endAction = EndAction.Loop };
                current = Instruments;
            }
        }

        public static void UnloadAll()
        {
            Voices_player?.unload();
            Voices_player?.unload();
            Instruments?.unload();

            Voices_player = null;
            Voices_opponent = null;
            Instruments = null;
        }

        public static void Play()
        {
            Instruments?.play();
            Voices_player?.play();
            Voices_opponent?.play();
        }

        public static void Stop()
        {
            Instruments?.stop();
            Voices_player?.stop();
            Voices_opponent?.stop();
        }

        public static void Resume()
        {
            Instruments?.resume();
            Voices_player?.resume();
            Voices_opponent?.resume();
        }
    }
}