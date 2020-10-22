using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TargetsStreamerMain.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public class SystemTracks
    {
        public const int ARRAY_SIZE = 3;

        public TimeType sentTimeStamp;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ARRAY_SIZE)]
        public TrackData[] systemTracks;

        public SystemTracks()
        {
            systemTracks = new TrackData[ARRAY_SIZE];
        }
        public byte[] ToByteArray()
        {
            byte[] arr;
            var ptr = IntPtr.Zero;
            try
            {
                var size = Marshal.SizeOf(this);
                arr = new byte[size];
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(this, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return arr;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TrackData
    {
        public long trackID;
        public float relativeBearing;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TimeType
    {
        public long hours;
        public long minutes;
        public long seconds;
        public long m_seconds;
        public long year;
        public long month;
        public long day;

        public static TimeType ParseFromDateTime(DateTime dateTime)
         {
             var dateTimeUtc = dateTime.ToUniversalTime();
             var timeType = new TimeType
             {
                 day = dateTimeUtc.Day,
                 m_seconds = dateTimeUtc.Millisecond,
                 hours = dateTimeUtc.Hour,
                 minutes = dateTimeUtc.Minute,
                 seconds = dateTimeUtc.Second,
                 year = dateTimeUtc.Year,
                 month = dateTime.Month
             };
             return timeType;
         }
    }
}
