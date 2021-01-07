using System;
using System.Collections.Generic;
using System.Text;

namespace HyperionDecompilerv2.Decompiler.Deserializer.Models
{
    public enum OpMode
    {
        iABC,
        iABx,
        iAsBx,
        Default
    }

    public enum OpCode : byte
    {
        GetGlobalOpt = 0xA4,
        GetGlobal = 0x35,
        InitVarArg = 0xA3,
        LoadInt16 = 0x8C,
        Return = 0x82,
        Call = 0x9F,
        LoadConst = 0x6F,
        GetTableConst = 0x4D,
        SetTableConst = 0x30,
        Move = 0x52,
        SetGlobal = 0x18,
        LoadNil = 0xC6,
        LoadBoolean = 0xA9,
        Closure = 0xD9,
        Init = 0xC0,
        Add = 0x43,
        Sub = 0x26,
        Mul = 0x09,
        Div = 0xEC,
        Mod = 0xCF,
        Pow = 0xB2,
        Jump = 0x65,
        Jump2 = 0x48,
        Equals = 0xF1,
        NotEqual = 0x9A,
        LessThan = 0xB7,
        GreaterThan = 0x7D,
        GreaterEqual = 0x60,
        LessEqual = 0xD4,
        Test = 0x0E,
        NotTest = 0x2B
    }

    public class OpProperties
    {
        public OpCode OpCode;
        public OpMode OpMode = OpMode.Default;
        public bool HasPsuedo;

        public OpProperties(OpCode OpCode, OpMode OpMode, bool HasPsuedo = false)
        {
            this.OpCode = OpCode;
            this.OpMode = OpMode;
            this.HasPsuedo = HasPsuedo;
        }
    }
}
