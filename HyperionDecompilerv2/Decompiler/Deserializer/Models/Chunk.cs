using System;
using System.Collections.Generic;
using System.Text;

namespace HyperionDecompilerv2.Decompiler.Deserializer.Models
{
    public class Chunk
    {
        public IList<string> strings;
        public IList<Function> closures;
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("p->sizek: " + strings.Count + '\n');

            for (int i = 0; i < strings.Count; ++i)
                builder.Append("p->k[" + i + "] = " + strings[i] + "\n");

            builder.Append("\n\n");

            for (int i = 0; i < closures.Count; ++i)
            {
                Function func = closures[i];

                builder.Append("p->p[" + i + "]->maxstacksize = " + func.maxstacksize + '\n');
                builder.Append("p->p[" + i + "]->numparams = " + func.numparams + '\n');
                builder.Append("p->p[" + i + "]->nups = " + func.upvalues.Count + '\n');
                builder.Append("p->p[" + i + "]->vararg = " + func.isvararg + "\n\n\n");

                builder.Append("p->p[" + i + "]->sizecode = " + func.instructions.Count + "\n\n");

                for (int j = 0; j < func.instructions.Count; ++j)
                {
                    Instruction instruction = func.instructions[j];

                    switch (instruction.Property.OpMode)
                    {
                        case OpMode.iABC:
                            builder.Append(string.Format("{0}\t {1} {2} {3}\n", instruction.Property.OpCode.ToString().ToUpper(), 
                                instruction.A, instruction.B, instruction.C));
                            break;
                        case OpMode.iABx:
                            builder.Append(string.Format("{0}\t {1} {2}\n", instruction.Property.OpCode.ToString().ToUpper(),
                                instruction.A, instruction.Bx));
                            break;
                        case OpMode.iAsBx:
                            builder.Append(string.Format("{0}\t {1} {2}\n", instruction.Property.OpCode.ToString().ToUpper(),
                                instruction.A, instruction.sBx));
                            break;
                    }

                    if (instruction.Property.HasPsuedo)
                        builder.Append(string.Format("   SUBINST {0}\n", (int)func.instructions[++j].sub));
                }
            }
            builder.Append('\n');

            return builder.ToString();
        }
    }
}
