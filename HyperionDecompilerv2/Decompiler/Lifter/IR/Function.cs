using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperionDecompilerv2.Decompiler.Lifter.IR
{
    public class Function : Expression
    {
        public IList<LocalExpression> args;
        public bool has_vararg;
        public Block body;

        public Function(IList<LocalExpression> args, bool has_vararg, Block body)
        {
            this.args = args;
            this.has_vararg = has_vararg;
            this.body = body;
        }

        public Function()
        {  }
    }
}
