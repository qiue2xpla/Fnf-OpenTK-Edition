using System.Runtime.InteropServices;
using System;

namespace Fnf.Framework.Audio
{
    internal static class Bass
    {
        public static bool Initialized { get; set; } = false;

        public delegate void SYNCPROC(int handle, int channel, int data, IntPtr user);

        public static bool BASS_Init(int device, int freq, BASSInit flags, IntPtr win)
        {
            if (Initialized) return false;

            Initialized = true;
            return BASS_Init(device, freq, flags, win, IntPtr.Zero);
        }

        public static int BASS_StreamCreateFile(string file, long offset, long length, BASSFlag flags)
        {
            flags |= BASSFlag.BASS_UNICODE;
            return BASS_StreamCreateFileUnicode(false, file, offset, length, flags);
        }

        public static long BASS_ChannelGetPosition(int handle)
        {
            return BASS_ChannelGetPosition(handle, BASSMode.BASS_POS_BYTE);
        }

        public static bool BASS_ChannelSetPosition(int handle, double seconds)
        {
            return BASS_ChannelSetPosition(handle, BASS_ChannelSeconds2Bytes(handle, seconds), BASSMode.BASS_POS_BYTE);
        }

        public static bool BASS_ChannelSetPosition(int handle, long pos)
        {
            return BASS_ChannelSetPosition(handle, pos, BASSMode.BASS_POS_BYTE);
        }

        public static bool BASS_ChannelSetPosition(int handle, int order, int row)
        {
            return BASS_ChannelSetPosition(handle, MakeLong(order, row), BASSMode.BASS_POS_MUSIC_ORDER);

            int MakeLong(int lowWord, int highWord)
            {
                return (highWord << 16) | (lowWord & 0xFFFF);
            }
        }

        #region External Functions

        [DllImport("bass")]
        public static extern int BASS_ChannelSetSync(int handle, BASSSync type, long param, SYNCPROC proc, IntPtr user);

        [DllImport("bass", EntryPoint = "BASS_PluginLoad")]
        private static extern int BASS_PluginLoadUnicode([In][MarshalAs(UnmanagedType.LPWStr)] string file, BASSFlag flags);
        
        [DllImport("bass")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_Free();

        [DllImport("bass")]
        public static extern float BASS_GetCPU();

        [DllImport("bass")]
        public static extern int BASS_GetDevice();

        [DllImport("bass")]
        public static extern int BASS_GetVersion();

        [DllImport("bass")]
        public static extern float BASS_GetVolume();

        [DllImport("bass")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_Pause();

        [DllImport("bass")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ChannelStop(int handle);

        [DllImport("bass")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_SetVolume(float volume);

        [DllImport("bass")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_StreamFree(int handle);

        [DllImport("bass")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ChannelIsSliding(int handle, BASSAttribute attrib);

        [DllImport("bass")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ChannelPause(int handle);

        [DllImport("bass")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ChannelPlay(int handle, [MarshalAs(UnmanagedType.Bool)] bool restart);

        [DllImport("bass")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ChannelSlideAttribute(int handle, BASSAttribute attrib, float value, int time);

        [DllImport("bass")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ChannelSetAttribute(int handle, BASSAttribute attrib, float value);

        [DllImport("bass")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ChannelGetAttribute(int handle, BASSAttribute attrib, ref float value);

        [DllImport("bass")]
        public static extern long BASS_ChannelSeconds2Bytes(int handle, double pos);

        [DllImport("bass")]
        public static extern long BASS_ChannelGetLength(int handle, BASSMode mode);

        [DllImport("bass")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BASS_ChannelSetPosition(int handle, long pos, BASSMode mode);
        
        [DllImport("bass")]
        public static extern long BASS_ChannelGetPosition(int handle, BASSMode mode);
        
        [DllImport("bass")]
        public static extern double BASS_ChannelBytes2Seconds(int handle, long pos);

        [DllImport("bass", EntryPoint = "BASS_StreamCreateFile")]
        private static extern int BASS_StreamCreateFileUnicode([MarshalAs(UnmanagedType.Bool)] bool mem, [In][MarshalAs(UnmanagedType.LPWStr)] string file, long offset, long length, BASSFlag flags);

        [DllImport("bass")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BASS_Init(int device, int freq, BASSInit flags, IntPtr win, IntPtr clsid);

        [DllImport("bass")]
        public static extern int BASS_ChannelGetLevel(int handle);

        [DllImport("bass")]
        public static extern BASSActive BASS_ChannelIsActive(int handle);

        [DllImport("bass")]
        public static extern int BASS_ChannelGetData(int handle, IntPtr buffer, int length);

        [DllImport("bass")]
        public static extern bool BASS_SetConfig(BASSConfig option, int newvalue);

        [DllImport("bass")]
        public static extern int BASS_GetConfig(BASSConfig option);

        #endregion
    }
}