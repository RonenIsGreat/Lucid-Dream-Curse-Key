using System.Text;

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