using System;
using System.Collections.Generic;
using System.Text;

namespace HyperionDecompilerv2.Decompiler.Lifter.IR
{
    /// <summary>
    /// Base class for expressions.
    /// </summary>
    public class Expression
    {
    }

    /// <summary>
    /// Base class for statements.
    /// </summary>
    public class Statement
    { 
    }

    public class Constant : Expression
    {
        public enum ConstantType
        { 
            Number, 
            String,
            Boolean,
            Nil,
            Global
        }

        public ConstantType Type;
        public double Number;
        public string String;
        public bool Boolean;

        public Constant(double Number)
        {
            this.Number = Number;
            Type = ConstantType.Number;
        }
        public Constant(string String)
        {
            this.String = String;
            Type = ConstantType.String;
        }
        public Constant(bool Boolean)
        {
            this.Boolean = Boolean;
            Type = ConstantType.Boolean;
        }

        public Constant(ConstantType Type)
        {
            this.Type = Type;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case ConstantType.Number:
                    return Number.ToString();
                case ConstantType.String:
                    return '"' + String + '"';
                case ConstantType.Boolean:
                    return Boolean ? "true" : "false";
                case ConstantType.Nil:
                    return "nil";
                default:
                    return "";
            }
        }
    }

    public class Global : Expression
    {
        public string value;

        public Global(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value;
        }
    }

    public class NameIndex : Expression
    {
        public Expression expression;
        public string index_name;
        public NameIndex(Expression expression, string index_name)
        {
            this.expression = expression;
            this.index_name = index_name;
        }
    }

    public static class Reference
    {
        public static int ID = 0;
        public static int ARGID = 0;
        public static int UniqueID = 0;
    }

    public class LocalExpression : Expression
    {
        public int Referenced;
        private int ID;
        public int UniqueID;
        public Expression expression;
        public bool arg = false;

        public LocalExpression(Expression expression)
        {
            ID = Reference.ID++;
            this.expression = expression;
            UniqueID = Reference.UniqueID++;
        }

        public LocalExpression()
        {
            arg = true;
            ID = Reference.ARGID++;
            UniqueID = Reference.UniqueID++;
        }

        public override string ToString()
        {
            return arg ? "a" + ID : "v" + ID;
        }
    }

    public class Arithmetic : Expression
    {
        public Expression left;
        public Expression right;
        public Operation op;

        public enum Operation
        { 
            Add,    // +
            Sub,    // -
            Mul,    // *
            Div,    // /
            Pow,    // ^
            Mod     // %
        }

        public Arithmetic(Expression left, Operation op, Expression right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }
    }

    public class Unary : Expression
    {
        public Expression expr;
        public Operation op;

        public enum Operation
        { 
            Not,
            Len,
            Minus
        }

        public Unary(Operation op, Expression expr)
        {
            this.op = op;
            this.expr = expr;
        }
    }
}
