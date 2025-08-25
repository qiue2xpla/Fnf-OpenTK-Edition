using System.Collections.Generic;
using System;

namespace Fnf.Framework.Audio
{
    public static class ClipsManager
    {
        internal static List<Clip> loadedClips = new List<Clip>();
        internal static List<Clip> pauseBuffer = new List<Clip>();

        /// <summary>
        /// Changes the volume of the system.<br/>
        /// BE CAREFUL AND DON'T MAKE IT 1 OR YOU WILL GO DEAF LIKE ME
        /// </summary>
        public static float SystemVolume
        {
            get => Bass.BASS_GetVolume();
            set => Bass.BASS_SetVolume(value);
        }

        /// <summary>
        /// Changes the volume of the app
        /// </summary>
        public static float AppVolume
        {
            get => (float)Bass.BASS_GetConfig(BASSConfig.BASS_CONFIG_GVOL_STREAM) / 10000;
            set => Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_GVOL_STREAM, (int)(value * 10000));
        }

        public static void UnloadAll()
        {
            foreach (var clip in loadedClips.ToArray())
            {
                clip.unload();
                pauseBuffer.Clear();
            }
        }

        public static void StopAll()
        {
            foreach (var clip in loadedClips)
            {
                clip.stop();
            }
        }

        public static void ResumeAll()
        {
            foreach (var clip in loadedClips)
            {
                clip.resume();
            }
        }

        public static void StopAllBuffered()
        {
            if (pauseBuffer.Count != 0) return;

            foreach (var clip in loadedClips)
            {
                if (clip.isPlaying) pauseBuffer.Add(clip);

                clip.stop();
            }
        }

        public static void ResumeAllBuffered()
        {
            if (pauseBuffer.Count == 0) return;

            foreach (var clip in pauseBuffer)
            {
                clip.resume();
            }

            pauseBuffer.Clear();
        }

        static ClipsManager()
        {
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
        }
    }
}