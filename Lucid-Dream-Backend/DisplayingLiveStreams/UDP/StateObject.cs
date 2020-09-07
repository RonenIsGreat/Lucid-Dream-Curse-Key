using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    public class StateObject
    {
        public const int BufferSize = 1400;
        public byte[] buffer = new byte[BufferSize];
        public int bytesCount = 0;
        public StringBuilder sb = new StringBuilder();
    }
}
