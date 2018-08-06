using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriFormat.Pers
{
    public class GifPart
    {
        public byte[] Data;
        public string Name;
        public int Frames;
        public int Offset;
        public int Size;
        public static bool IsGif(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 5)
            {
                return false;
            }
            return bytes[0] == 'G' && bytes[1] == 'I' && bytes[2] == 'F' && bytes[3] == '8' && bytes[4] == '9';
        }
    }
}
