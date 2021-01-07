
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperionDecompilerv2.Decompiler.Lifter.IR
{
    public class Call : Expression
    {
        public Expression Function;
        public IList<Expression> Args;
        public bool IsSelf;

        public Call(Expression Function, IList<Expression> Args, bool IsSelf)
        {
            this.Function = Function;
            this.Args = Args;
            this.IsSelf = IsSelf;
        }
    }
}
