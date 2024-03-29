﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HyperionDecompilerv2.Decompiler.Deserializer.Models
{
    public class Function
    {
        public byte numparams;
        public byte maxstacksize;
        public bool isvararg;
        public int name_idx;

        public IList<Constant> constants;
        public IList<Instruction> instructions;
        public IList<Function> closures;
        public IList<string> upvalues;
        public IList<int> closure_idx;
        public IList<int> lineinfo;
    }
}
