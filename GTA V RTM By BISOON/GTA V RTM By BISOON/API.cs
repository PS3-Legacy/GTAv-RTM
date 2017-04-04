using PS3Lib;
using System;

namespace GTA_V_RTM_By_BISOON
{
    public static class Exten
    {
      static  PS3API PS3 = API.PS3;
        public static float[] ReadFloat(this Extension ext, uint address, int length)
        {
            byte[] memory = PS3.GetBytes(address, length * 4);
            Array.Reverse(memory);
            float[] numArray = new float[length - 1 + 1];
            for (int i = 0; i <= length - 1; i++)
            {
                numArray[i] = System.Convert.ToSingle(BitConverter.ToSingle(memory, ((length - 1) - i) * 4));
            }
            return numArray;
        }
        public static uint ReadUInt32(this Extension ext, uint offset, bool Reverse)
        {
            byte[] array = PS3.GetBytes(offset, 4);
            if (Reverse)
            {
                Array.Reverse(array, 0, 4);
            }
            return BitConverter.ToUInt32(array, 0);
        }
    }
    public class API
    {
        public static PS3API PS3 = new PS3API();
    }
}
