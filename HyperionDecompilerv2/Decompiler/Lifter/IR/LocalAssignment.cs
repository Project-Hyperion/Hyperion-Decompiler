using System;
using System.Collections.Generic;
using System.Text;

namespace HyperionDecompilerv2.Decompiler.Lifter.IR
{
    public class LocalAssignment : Statement
    {
        public int ID;

        public LocalExpression variables;
        public LocalAssignment(LocalExpression variables)
        {
            this.variables = variables;
            
            ID = variables.UniqueID;
        }
    }
}
