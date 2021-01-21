using HyperionDecompilerv2.Decompiler.Deserializer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperionDecompilerv2.Decompiler.Lifter
{
    public class Lifter
    {
        private IDictionary<int, IR.LocalExpression> RegisterMap;
       
        private Function function;

        private Chunk LuauChunk;
        public Lifter(Chunk LuauChunk) =>
            this.LuauChunk = LuauChunk;

        public IR.Function Lift()
        {    
            return (IR.Function)LiftClosure(LuauChunk.closures[LuauChunk.closures.Count - 1]);
        }
        public IR.Expression LiftClosure(Function function)
        {
            this.function = function;

            IR.Function ret = null;

            RegisterMap = new Dictionary<int, IR.LocalExpression>(function.maxstacksize);

            List<IR.Statement> body = new List<IR.Statement>();

            IList<Constant> constants = function.constants;

            for (int i = 0; i < function.instructions.Count; ++i)
            {
                Instruction instruction = function.instructions[i];

                if (instruction.Property == null)
                    continue;

                switch (instruction.Property.OpCode)
                {
                    case OpCode.InitVarArg:
                        RegisterMap.Clear();

                        ret = new IR.Function();
                        ret.args = new List<IR.LocalExpression>();
                        ret.has_vararg = true;
                        break;
                    case OpCode.Closure:
                        IR.Expression closure = LiftClosure(LuauChunk.closures[instruction.B]);

                        RegisterMap.Clear();

                        /*
                            R(A) = register
                            B = ??
                            C = ??
                        */

                        LoadRegister(instruction.A, closure, body);
                        break;
                    case OpCode.Init:
                        RegisterMap.Clear();
                        RegisterMap = new Dictionary<int, IR.LocalExpression>(function.maxstacksize);

                        int nargs = function.numparams;

                        List<IR.LocalExpression> fargs = new List<IR.LocalExpression>(nargs);

                        for (int j = 0; j < nargs; ++j)
                            fargs.Add(new IR.LocalExpression());

                        ret = new IR.Function();
                        ret.args = fargs;
                        ret.has_vararg = false;

                        /*
                            A = numparams?
                            B = 0
                            C = 0

                            NOTE: Not sure, reverse INIT some more.
                        */

                        for (int j = 0; j < nargs; ++j)
                            LoadRegisterArg(j, fargs[j], body);
                        break;
                    case OpCode.LoadConst:
                        IR.Constant constant = new IR.Constant(GetConstant(constants[instruction.Bx]));

                        /*
                            R(A) = register
                            RK(Bx) = constant
                         */

                        LoadRegister(instruction.A, constant, body);
                        break;
                    case OpCode.LoadBoolean:
                        constant = new IR.Constant(Convert.ToBoolean(instruction.B));

                        /*
                            R(A) = register
                            B = true / false
                            C = skipnext
                         */

                        LoadRegister(instruction.A, constant, body);
                        break;
                    case OpCode.LoadInt16:
                        constant = new IR.Constant(instruction.Bx);

                        /*
                            R(A) = register
                            Bx = value
                            
                            NOTE: LOADI16 may be iAsBx, reverse.
                         */

                        LoadRegister(instruction.A, constant, body);
                        break;
                    case OpCode.LoadNil:
                        constant = new IR.Constant(IR.Constant.ConstantType.Nil);

                        /*
                            R(A) = register
                            
                            NOTE: B, C are throw away registers.
                         */
                        
                        LoadRegister(instruction.A, constant, body);
                        break;
                    case OpCode.GetGlobalOpt:
                        Instruction sub = function.instructions[++i];

                        int[] indexes = Decoding.GlobalIndexes(sub.sub);

                        int index1 = indexes[0];
                        int index2 = indexes[1];

                        /*
                            R(A) = register
                            RK(Bx) = constant      
                            RK(Sub) = encoded

                            NOTE: Luau compiler optimization allows 2 indexes in sub 
                            (calls for name index e.g. table.foreach).
                         */

                        // Console.WriteLine("Index1 {0}, Index2 {1}, Bx {2}", index1, index2, instruction.Bx);

                        IR.Expression expression = new IR.Global(GetConstant(constants[instruction.Bx]));

                        if (index1 > 1 && index2 >= 1)
                        {
                            expression = new IR.Global(GetConstant(constants[index1]));

                            if (index2 >= 1)
                                expression = new IR.NameIndex(expression, GetConstant(constants[index2]));

                            LoadRegister(instruction.A, expression, body);
                            break;
                        }

                        LoadRegister(instruction.A, expression, body);
                        break;
                    case OpCode.GetGlobal:
                        sub = function.instructions[++i];
                        expression = new IR.Global(GetConstant(constants[(int)sub.sub]));

                        /*
                            R(A) = register
                            RK(Bx) = ??      
                            RK(Sub) = constant

                            NOTE: Not 100% confident with this assumption, reverse. 
                            If is iABx figure out what Bx is. Don't need Bx yet.
                         */

                        LoadRegister(instruction.A, expression, body);
                        break;
                    case OpCode.GetTableConst:
                        sub = function.instructions[++i];
                        string name = GetConstant(constants[(int)sub.sub]);
                       
                        expression = new IR.NameIndex(GetRegister(instruction.B), name);
                        FreeRegister(instruction.B, body);
                        
                        /*
                            R(A) = register
                            R(B) = reference to the index expression     
                            C = ??
                            RK(Sub) = constant    

                            NOTE: Not 100% confident with assumption for C, reverse. 
                            C may be a hash of the index string.
                         */

                        LoadRegister(instruction.A, expression, body);
                        break;
                    case OpCode.GetTable:
                        expression = GetRegister(instruction.B);
                        IR.Expression expression1 = GetRegister(instruction.C);

                        FreeRegister(instruction.B, body);
                        FreeRegister(instruction.C, body);

                        if (expression1 is IR.Constant)
                        {
                            constant = (IR.Constant)expression1;

                            expression = new IR.NameIndex(expression, constant.String);
                        }

                        LoadRegister(instruction.A, expression, body);
                        break;
                    case OpCode.SetGlobal:
                        sub = function.instructions[++i];
                        
                        expression = GetRegister(instruction.A);

                        IR.Global global = new IR.Global(GetConstant(constants[(int)sub.sub]));

                        /*
                            R(A) = reference to object
                            B = ??     
                            C = ??
                            RK(Sub) = constant    

                            NOTE: Not 100% confident with assumption for C and B, reverse. 
                         */
                       
                        AddStatement(new IR.Assign(global, expression), body);
                        break;
                    case OpCode.SetTableConst:
                        expression = GetRegister(instruction.A);
                        IR.Expression name_idx = GetRegister(instruction.B);

                        FreeRegister(instruction.A, body);
                        FreeRegister(instruction.B, body);

                        /*
                            R(A) = reference to constant
                            B = reference to name index     
                            C = ??
                            RK(Sub) = constant    

                            NOTE: Not 100% confident with assumption for C, reverse. 
                            C may be a hash of the index string.
                         */

                        AddStatement(new IR.Assign(name_idx, expression), body);
                        break;
                    case OpCode.SetTable:
                        expression = GetRegister(instruction.B);
                        IR.Expression expression2 = GetRegister(instruction.C);

                        FreeRegister(instruction.B, body);
                        FreeRegister(instruction.C, body);

                        AddStatement(new IR.Assign(expression, expression2), body);
                        break;
                    case OpCode.Call:
                        IList<IR.Expression> args = new List<IR.Expression>();

                        IR.Expression func = GetRegister(instruction.A);
                        FreeRegister(instruction.A, body);

                        int nparams = instruction.B - 1;

                        if (nparams > 0)
                        {
                            for (int j = 1; j <= nparams; ++j)
                            {
                                int register = instruction.A + j;
                                 IR.Expression arg = GetRegister(register);


                                args.Add(arg);

                                FreeRegister(register, body);
                            }
                        }
                        else if (nparams == -1)
                        {
                            args.Add(GetRegister(instruction.A + 1));
                            FreeRegister(instruction.A + 1, body);
                        }

                        IR.Call call = new IR.Call(func, args, false);

                        /*
                            R(A) = reference
                            B = args      
                            C = retn
                         */

                        if ((instruction.C - 1) == 0)
                        {
                            AddStatement(call, body);
                            break;
                        }

                        LoadRegister(instruction.A, call, body);
                        break;
                    case OpCode.Move:
                        FreeRegister(instruction.A, body);

                        /*
                            R(A) = reference
                            R(B) = reference copied      
                         */

                        RegisterMap[instruction.B].Referenced++;
                        RegisterMap[instruction.A] = RegisterMap[instruction.B];
                        break;
                    case OpCode.Return:
                        if (ret == null) break;

                        ret.body = new IR.Block(body);
                        break;
                }
            }

            return ret;
        }

        private void LoadRegister(int register, IR.Expression expression, List<IR.Statement> body)
        {
            IR.LocalExpression local = new IR.LocalExpression(expression);

            if (!(expression is IR.Function)) 
                body.Add(new IR.LocalAssignment(local));

            if (RegisterMap.ContainsKey(register))
                FreeRegister(register, body);

            if ((RegisterMap.Count - 1) == register)
            {
                RegisterMap[register] = local;
                return;
            }

            RegisterMap.Add(register, local);
        }

        private void LoadRegisterArg(int register, IR.LocalExpression arg, IList<IR.Statement> body)
        {
            if ((RegisterMap.Count - 1) == register)
            {
                RegisterMap[register] = arg;
                return;
            }

            RegisterMap.Add(register, arg);
        }

        private void FreeRegister(int register, List<IR.Statement> body)
        {
            // if (RegisterMap.Count - 1 < register) return;

            IR.LocalExpression local = RegisterMap[register];

            if (local.Referenced > 1 || local.arg)
            {
                RegisterMap.Remove(register);
                return;
            }

            int index = body.FindIndex(x => x is IR.LocalAssignment && ((IR.LocalAssignment)x).ID == local.UniqueID);
     
            if (index > -1)
                body.RemoveAt(index);

            RegisterMap.Remove(register);

            IR.Reference.ID--;
        }

        private IR.Expression GetRegister(int register)
        {
            RegisterMap[register].Referenced++;

            return RegisterMap[register];
        }

        private void AddStatement(IR.Expression expression, IList<IR.Statement> body)
        {
            body.Add(new IR.StatementExpression(expression));
        }

        private void AddStatement(IR.Statement statement, IList<IR.Statement> body)
        {
            body.Add(statement);
        }

        private string GetConstant(Constant constant)
        { 
            if (constant is StringConstant)
            {
                int index = ((StringConstant)constant).Value;

                return LuauChunk.strings[index - 1];
            }

            if (constant is GlobalConstant)
            {
                int index = Decoding.GlobalIndex(((GlobalConstant)constant).Value);

                return GetConstant(function.constants[index]);
            }

            return constant.ToString();
        }
    }
}
