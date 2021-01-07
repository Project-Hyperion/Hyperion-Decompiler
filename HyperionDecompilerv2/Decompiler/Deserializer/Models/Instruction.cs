using System;
using System.Collections.Generic;
using System.Text;

namespace HyperionDecompilerv2.Decompiler.Deserializer.Models
{
    public class Instruction
    {
        public OpProperties Property;

        public byte A;
        public byte B;
        public byte C;
        public ushort Bx;
        public short sBx;

        public uint sub;

        public Instruction(OpProperties Property, byte A, byte B, byte C)
        {
            this.Property = Property;
            this.A = A;
            this.B = B;
            this.C = C;
        }

        public Instruction(OpProperties Property, byte A, ushort Bx)
        {
            this.Property = Property;
            this.A = A;
            this.Bx = Bx;
        }

        public Instruction(OpProperties Property, byte A, short sBx)
        {
            this.Property = Property;
            this.A = A;
            this.sBx = sBx;
        }

        public Instruction(uint sub)
        {
            this.sub = sub;
        }
    }
}
