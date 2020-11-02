using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AudioCreate
{
    class TargetManagement
    {
        private  Dictionary<long, WaveFileWriter> TargetWriters = new Dictionary<long, WaveFileWriter>();
        private  WaveFormat waveFormat = new WaveFormat(31250, 16, 1);
        private WaveFileWriter getWriter(long tID)
        {

            if (!(TargetWriters.ContainsKey(tID)))
            {
                string filename = "Audio/" + tID + ".wav";
                WaveFileWriter newWriter = new WaveFileWriter(filename, waveFormat);
                TargetWriters.Add(tID, newWriter);
            }
            return TargetWriters[tID];
        }
    }
}
