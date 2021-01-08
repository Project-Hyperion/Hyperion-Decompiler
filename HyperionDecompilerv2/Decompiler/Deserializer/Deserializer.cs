using System;
using System.Collections.Generic;
using System.Text;
using HyperionDecompilerv2.Decompiler.Deserializer.Models;

namespace HyperionDecompilerv2.Decompiler.Deserializer
{
    public class Deserializer
    {
        private Reader.BytecodeReader Reader;

        public Deserializer(byte[] bytes) =>
            Reader = new Reader.BytecodeReader(bytes);

        public static IDictionary<OpCode, OpProperties> OpProperties = new Dictionary<OpCode, OpProperties>()
        {
            { OpCode.GetGlobal, new OpProperties(OpCode.GetGlobal, OpMode.iABx, true) },
            { OpCode.GetGlobalOpt, new OpProperties(OpCode.GetGlobalOpt, OpMode.iABx, true) },
            { OpCode.InitVarArg, new OpProperties(OpCode.InitVarArg, OpMode.iABC, false) },
            { OpCode.LoadInt16, new OpProperties(OpCode.LoadInt16, OpMode.iABx, false) },
            { OpCode.Return, new OpProperties(OpCode.Return, OpMode.iABC, false) },
            { OpCode.Call, new OpProperties(OpCode.Call, OpMode.iABC, false) },
            { OpCode.LoadConst, new OpProperties(OpCode.LoadConst, OpMode.iABx, false) },
            { OpCode.GetTableConst, new OpProperties(OpCode.GetTableConst, OpMode.iABC, true) },
            { OpCode.SetTableConst, new OpProperties(OpCode.SetTableConst, OpMode.iABC, true) },
            { OpCode.Move, new OpProperties(OpCode.Move, OpMode.iABC, false) },
            { OpCode.SetGlobal, new OpProperties(OpCode.SetGlobal, OpMode.iABC, true) },
            { OpCode.LoadNil, new OpProperties(OpCode.LoadNil, OpMode.iABC, false) },
            { OpCode.LoadBoolean, new OpProperties(OpCode.LoadBoolean, OpMode.iABC, false) },
            { OpCode.Closure, new OpProperties(OpCode.Closure, OpMode.iABx, false) },
            { OpCode.Init, new OpProperties(OpCode.Init, OpMode.iABC, false) },
            { OpCode.Add, new OpProperties(OpCode.Add, OpMode.iABC, false) },
            { OpCode.Sub, new OpProperties(OpCode.Sub, OpMode.iABC, false) },
            { OpCode.Div, new OpProperties(OpCode.Div, OpMode.iABC, false) },
            { OpCode.Mul, new OpProperties(OpCode.Mul, OpMode.iABC, false) },
            { OpCode.Mod, new OpProperties(OpCode.Mod, OpMode.iABC, false) },
            { OpCode.Pow, new OpProperties(OpCode.Pow, OpMode.iABC, false) },
            { OpCode.Jump, new OpProperties(OpCode.Jump, OpMode.iAsBx, false) },
            { OpCode.Test, new OpProperties(OpCode.Test, OpMode.iAsBx, false) },
            { OpCode.NotTest, new OpProperties(OpCode.NotTest, OpMode.iAsBx, false) },
            { OpCode.Jump2, new OpProperties(OpCode.Jump2, OpMode.iABx, false) }
        };

        public Chunk Deserialize()
        {
            Chunk Main = new Chunk();

            Reader.ReadBoolean(); // Retarded first byte

            Main.strings = ReadStrings();
            Main.closures = ReadClosures();

            return Main;
        }

        private IList<string> ReadStrings()
        {
            int size = Reader.ReadInt32Compressed();

            IList<string> strings = new List<string>(size);

            while (strings.Count < size)
                strings.Add(Reader.ReadASCII(Reader.ReadInt32Compressed()));

            return strings;
        }

        private IList<Function> ReadClosures()
        {
            int size = Reader.ReadInt32Compressed();

            IList<Function> closures = new List<Function>(size);

            while (closures.Count < size)
                closures.Add(ReadClosure(closures.Count, size));

            return closures;
        }

        private Function ReadClosure(int index, int max)
        {
            Function function;
            
            function = new Function();

            function.maxstacksize = Reader.ReadByte();
            function.numparams = Reader.ReadByte();
            function.upvalues = new List<string>(Reader.ReadByte());
            function.isvararg = Reader.ReadBoolean();
            function.instructions = ReadInstructions();
            function.constants = ReadConstants();

            if (index + 1 != max)
            {
                function.closure_idx = ReadClosureIndexs();
                function.name_idx = Reader.ReadInt32Compressed();
                function.lineinfo = ReadLineInfo(function);

                Reader.ReadBoolean();
            }   

            return function;
        }

        private IList<int> ReadLineInfo(Function function)
        {
            if (!Reader.ReadBoolean()) return new List<int>();

            int size = Reader.ReadInt32Compressed();

            if (size != 0)
            {
                int v92 = (int)((function.instructions.Count + 3) & 0xFFFFFFFC);
                int k = ((function.instructions.Count - 1) >> size) + 1;

                size = v92 + 4 * k;
            }

            // Console.WriteLine(size);

            IList<int> lineinfo = new List<int>(size);

            for (int i = 0; i < size; ++i)
                lineinfo.Add(Reader.ReadByte());

            return lineinfo;
        }

        private IList<int> ReadClosureIndexs()
        {
            int size = Reader.ReadByte();

            IList<int> indexs = new List<int>(size);

            while (indexs.Count < size)
                indexs.Add(Reader.ReadInt32Compressed());

            return indexs;
        }

        private IList<Constant> ReadConstants()
        {
            int size = Reader.ReadInt32Compressed();

            IList<Constant> constants = new List<Constant>(size);

            while (constants.Count < size)
                constants.Add(ReadConstant());

            return constants;
        }

        private Constant ReadConstant()
        {
            Models.Type type = (Models.Type)Reader.ReadByte();

            switch (type)
            {
                case Models.Type.Nil:
                    return new NilConstant();
                case Models.Type.Bool:
                    bool value = Reader.ReadBoolean();

                    return new BoolConstant(value);
                case Models.Type.Number:
                    double value2 = Reader.ReadDouble();

                    return new NumberConstant(value2);
                case Models.Type.String:
                    int constant_idx = Reader.ReadInt32Compressed();

                    return new StringConstant(constant_idx);
                case Models.Type.Global:
                    uint global_index = Reader.ReadUInt32();

                    return new GlobalConstant(global_index);
                default:
                    Console.WriteLine(type);
                    return new GlobalConstant(1);
            }
        }

        private IList<Instruction> ReadInstructions()
        {
            int size = Reader.ReadInt32Compressed();

            IList<Instruction> code = new List<Instruction>(size);

            while (code.Count < size)
            {
                OpCode Op = (OpCode)Reader.ReadByte();
                
                OpProperties Property = OpProperties[Op];

                switch (Property.OpMode)
                {
                    case OpMode.iABC:
                        byte A = Reader.ReadByte();
                        byte B = Reader.ReadByte();
                        byte C = Reader.ReadByte();

                        code.Add(new Instruction(Property, A, B, C));
                        break;
                    case OpMode.iABx:
                        A = Reader.ReadByte();
                        short Bx = Reader.ReadInt16();

                        code.Add(new Instruction(Property, A, Bx));
                        break;
                    case OpMode.iAsBx:
                        A = Reader.ReadByte();
                        ushort sBx = Reader.ReadUInt16();

                        code.Add(new Instruction(Property, A, sBx));
                        break;
                }

                if (Property.HasPsuedo)
                {
                    uint sub = Reader.ReadUInt32();
                    code.Add(new Instruction(sub));
                }
            }

            return code;
        }
    }
}
