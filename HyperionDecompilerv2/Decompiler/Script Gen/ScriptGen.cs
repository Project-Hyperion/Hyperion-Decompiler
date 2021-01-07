using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperionDecompilerv2.Decompiler.Script_Gen
{
    public class ScriptGen
    {
        private Lifter.IR.Function Restricted;
        private StringBuilder builder;
        private static int indentation = 0;

        public ScriptGen(Lifter.IR.Function Restricted)
        {
            this.Restricted = Restricted;

            builder = new StringBuilder();
        }

        public StringBuilder Generate()
        {
            builder.Append("-- Decompiled by Hyperion Decompiler\n\n");

            GenerateBlock(Restricted.body);

            return builder;
        }

        private void GenerateBlock(Lifter.IR.Block block)
        {

            for (int i = 0; i < block.Body.Count; ++i)
            {
                for (int j = 0; j < indentation; ++j)
                    builder.Append("  ");

                if (block.Body[i] is Lifter.IR.StatementExpression)
                {
                    Lifter.IR.Expression expression = ((Lifter.IR.StatementExpression)block.Body[i]).Expression;

                    if (expression is Lifter.IR.Call)
                        GenerateCall((Lifter.IR.Call)expression);
                }

                else if (block.Body[i] is Lifter.IR.LocalAssignment)
                    GenerateLocalAssignment((Lifter.IR.LocalAssignment)block.Body[i]);

                else if (block.Body[i] is Lifter.IR.Assign)
                    GenerateAssign((Lifter.IR.Assign)block.Body[i]);

                builder.Append("\n"); 
            }
        }

        private void GenerateAssign(Lifter.IR.Assign assign)
        {
            Lifter.IR.LocalExpression l = ((Lifter.IR.LocalExpression)assign.value);
            if (l.expression is Lifter.IR.Function)
            {
                builder.Append("function ");

                GenerateExpression(assign.variable);

                builder.Append("(");

                Lifter.IR.Function f = ((Lifter.IR.Function)l.expression);

                for (int i = 0; i < f.args.Count; ++i)
                {
                    GenerateExpression(f.args[i]);

                    if (f.args.Count - 1 != i)
                        builder.Append(", ");
                }

                builder.Append(")\n");

                indentation++;
                GenerateBlock(f.body);
                indentation--;

                builder.Append("end\n");
                return;
            }

            GenerateExpression(assign.variable);

            builder.Append(" = ");

            GenerateExpression(assign.value);
        }

        private void GenerateLocalAssignment(Lifter.IR.LocalAssignment assign)
        {
            builder.Append("local " + assign.variables.ToString() + " = ");

            GenerateExpression(assign.variables.expression);
        }

        private void GenerateCall(Lifter.IR.Call call)
        {
            GenerateExpression(call.Function);

            builder.Append('(');

            GenerateExpressionList(call.Args);

            builder.Append(')');
        }

        private void GenerateLocalExpression(Lifter.IR.LocalExpression local_expr)
        {
            if (local_expr.Referenced > 1)
            {
                builder.Append(local_expr.ToString());
                return;
            }

            GenerateExpression(local_expr.expression);
        }

        private void GenerateExpression(Lifter.IR.Expression expression)
        {
            if (expression is Lifter.IR.Global)
                builder.Append(((Lifter.IR.Global)expression).value);

            else if (expression is Lifter.IR.Constant)
                builder.Append(((Lifter.IR.Constant)expression).ToString());

            else if (expression is Lifter.IR.LocalExpression)
                GenerateLocalExpression((Lifter.IR.LocalExpression)expression);

            else if (expression is Lifter.IR.NameIndex)
                GenerateNameIndex((Lifter.IR.NameIndex)expression);

            else if (expression is Lifter.IR.Call)
                GenerateCall((Lifter.IR.Call)expression);
        }

        private void GenerateNameIndex(Lifter.IR.NameIndex name_idx)
        {
            GenerateExpression(name_idx.expression);
            builder.Append("." + name_idx.index_name);
        }

        private void GenerateExpressionList(IList<Lifter.IR.Expression> list)
        {
            Lifter.IR.Expression expression;

            for (int i = 0; i < list.Count; ++i)
            {
                expression = list[i];

                GenerateExpression(expression);

                if (list.Count - 1 != i)
                    builder.Append(", ");
            }
        }
    }
}
