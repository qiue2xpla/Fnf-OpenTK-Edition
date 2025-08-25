using System.IO;
using System;

namespace Fnf.Framework.Audio
{
    public class Clip
    {
        public double length { get; private set; }
        public EndAction endAction = EndAction.Nothing;
        public event Action onPlaybackEnded;

        #region Changing Variables

        public VolumeLevel level
        {
            get
            {
                if (!isPlaying) return new VolumeLevel();

                int level = Bass.BASS_ChannelGetLevel(id);

                float left = (float)LowWord32(level) / 32768;
                float right = (float)HighWord32(level) / 32768;

                return new VolumeLevel(left, right);

                int LowWord32(int dWord)
                {
                    return dWord & 0xFFFF;
                }
                int HighWord32(int dWord)
                {
                    return (dWord >> 16) & 0xFFFF;
                }
            }
        }

        public double position
        {
            get => Bass.BASS_ChannelBytes2Seconds(id, Bass.BASS_ChannelGetPosition(id));
            set => Bass.BASS_ChannelSetPosition(id, value);
        }

        public float volume
        {
            get
            {
                float v = 1;
                Bass.BASS_ChannelGetAttribute(id, BASSAttribute.BASS_ATTRIB_VOL, ref v);
                return v;
            }

            set => Bass.BASS_ChannelSetAttribute(id, BASSAttribute.BASS_ATTRIB_VOL, value);
        }

        public bool isPlaying
        {
            get => Bass.BASS_ChannelIsActive(id) == BASSActive.BASS_ACTIVE_PLAYING;
        }

        public bool isLoaded => id != -1;

        #endregion

        //referancing it so GC doesn't collect it
        readonly Bass.SYNCPROC Referance;
        string path;
        int id;

        public Clip(string path)
        {
            if (File.Exists(path))
            {
                id = Bass.BASS_StreamCreateFile(path, 0, 0, BASSFlag.BASS_DEFAULT);
                length = Bass.BASS_ChannelBytes2Seconds(id, Bass.BASS_ChannelGetLength(id, BASSMode.BASS_POS_BYTE));
                Referance = Callback;
                this.path = path;

                Bass.BASS_ChannelSetSync(id, BASSSync.BASS_SYNC_END, 0, Referance, IntPtr.Zero);

                ClipsManager.loadedClips.Add(this);
            }
            else throw new FileNotFoundException(path);
        }

        #region Controls

        public void play()
        {
            Bass.BASS_ChannelPlay(id, true);
        }

        public void stop()
        {
            Bass.BASS_ChannelStop(id);
        }
        
        public void resume()
        {
            Bass.BASS_ChannelPlay(id, false);
        }

        public void unload()
        {
            if(isLoaded)
            {
                Bass.BASS_StreamFree(id);
                id = -1;

                ClipsManager.loadedClips.Remove(this);
            }
        }

        public void slideVolume(float to, float time)
        {
            Bass.BASS_ChannelSlideAttribute(id, BASSAttribute.BASS_ATTRIB_VOL, to, (int)(time * 1000));
        }

        public void slideVolume(float from, float to, float time)
        {
            volume = from;
            Bass.BASS_ChannelSlideAttribute(id, BASSAttribute.BASS_ATTRIB_VOL, to, (int)(time * 1000));
        }

        #endregion

        void Callback(int handle, int channel, int idk, IntPtr user)
        {
            onPlaybackEnded?.Invoke();

            switch (endAction)
            {
                case EndAction.Dispose:
                    unload();
                    break;

                case EndAction.Loop:
                    play();
                    break;
            }
        }

        static Clip()
        {
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
        }
    }

    public enum EndAction
    {
        Nothing,
        Dispose,
        Loop
    }
}