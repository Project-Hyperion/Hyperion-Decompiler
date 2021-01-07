using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperionDecompilerv2.Decompiler.Lifter.IR
{
    public class StatementExpression : Statement
    {
        public Expression Expression;

        public StatementExpression(Expression Expression)
        {
            this.Expression = Expression;
        }
    }
}
