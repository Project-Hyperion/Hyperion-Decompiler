using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperionDecompilerv2.Decompiler.Lifter.IR
{
    public class Block : Statement
    {
        public IList<Statement> Body;
        public Block(IList<Statement> Body)
        {
            this.Body = Body;
        }
    }
}
