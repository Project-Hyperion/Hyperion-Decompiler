using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperionDecompilerv2.Decompiler.Lifter
{
    public static class Decoding
    {
        /// <summary>
        /// Decodes the global index of optimized getglobal (0xA4)
        /// </summary>
        public static int GlobalIndex(uint index)
        {
            return (int)(index >> 20) & 0x3FF;
        }

        /// <summary>
        /// Decodes multiple global indexes of optimized getglobal (0xA4)
        /// </summary>
        public static int[] GlobalIndexes(uint index)
        {
            int index2 = (int)(index >> 10) & 0x3FF;
            int index1 = index2 >> 20;

            return new int[] { index1, index2 };
        }
    }
}
