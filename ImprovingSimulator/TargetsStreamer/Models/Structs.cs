﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TargetsStreamerMain.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public class SystemTarget
    {
        public TimeType sentTimeStamp;

        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex =0)]
        public List<TargetData> systemTargets;

        public SystemTarget()
        {
            systemTargets = new List<TargetData>();
        }
        public byte[] ToByteArray()
        {
            byte[] arr;
            var ptr = IntPtr.Zero;
            try
            {
                var timeTypeSize = Marshal.SizeOf(typeof(TimeType));
                arr = new byte[timeTypeSize];
                ptr = Marshal.AllocHGlobal(timeTypeSize);
                Marshal.StructureToPtr(sentTimeStamp, ptr, true);
                Marshal.Copy(ptr, arr, 0, timeTypeSize);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            var systemTargetsAsByteArray = StructureArrayToByteArray(systemTargets);

            return arr.Concat(systemTargetsAsByteArray).ToArray();
        }

        /// <summary>
        /// To be used in converting from byte array to system tracks struct, not used in simulator
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static SystemTarget FromByteArray(byte[] arr)
        {
            IntPtr timeTypePtr = IntPtr.Zero;
            IntPtr targetsArrayPtr = IntPtr.Zero;
            try
            {
                var timeTypeSize = Marshal.SizeOf(typeof(TimeType));

                timeTypePtr = Marshal.AllocHGlobal(timeTypeSize);
                TimeType timeType = new TimeType();
                Marshal.Copy(arr, 0, timeTypePtr, timeTypeSize);
                timeType = Marshal.PtrToStructure<TimeType>(timeTypePtr);

                var len = Marshal.SizeOf(typeof(TargetData));

                var count = (arr.Length - timeTypeSize) / len;

                targetsArrayPtr = Marshal.AllocHGlobal(count * len);

                Marshal.Copy(arr, timeTypeSize, targetsArrayPtr, count * len);

                List<TargetData> list = new List<TargetData>();
                for (int i = 0; i < count; i++)
                {
                    list.Add(Marshal.PtrToStructure<TargetData>(targetsArrayPtr + i * len));
                }

                return new SystemTarget { sentTimeStamp = timeType, systemTargets = list };
            }
            finally
            {
                Marshal.FreeHGlobal(targetsArrayPtr);
                Marshal.FreeHGlobal(timeTypePtr);
            }


        }

        private byte[] StructureArrayToByteArray(List<TargetData> objs)
        {
            int structSize = Marshal.SizeOf(typeof(TargetData));
            int len = objs.Count * structSize;
            byte[] arr = new byte[len];
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.AllocHGlobal(len);
                for (int i = 0; i < objs.Count; i++)
                {
                    Marshal.StructureToPtr(objs[i], ptr + i * structSize, true);
                }
                Marshal.Copy(ptr, arr, 0, len);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return arr;
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TargetData
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
