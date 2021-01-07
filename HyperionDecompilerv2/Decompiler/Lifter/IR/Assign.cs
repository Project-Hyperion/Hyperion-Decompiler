using System;
using System.Collections.Generic;
using System.Text;

namespace HyperionDecompilerv2.Decompiler.Lifter.IR
{
    public class Assign : Statement
    {
        public Expression variable;
        public Expression value;

        public Assign(Expression variable, Expression value)
        {
            this.variable = variable;
            this.value = value;
        }
    }
}
