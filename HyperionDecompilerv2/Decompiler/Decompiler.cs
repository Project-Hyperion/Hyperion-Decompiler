using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperionDecompilerv2.Decompiler
{
    public class Decompiler
    {
        private Deserializer.Deserializer Deserializer;
        public Decompiler(byte[] bytecode) =>
            Deserializer = new Deserializer.Deserializer(bytecode);

        public void Decompile()
        {
            var chunkoooo = Deserializer.Deserialize();

            Console.WriteLine(chunkoooo.ToString() + "\n\n\n\n");

            Lifter.Lifter lifter = new Lifter.Lifter(chunkoooo);
            var blockooo = lifter.Lift();

            Script_Gen.ScriptGen gen = new Script_Gen.ScriptGen(blockooo);
            Console.WriteLine("Decompiled: \n\n" + gen.Generate().ToString());
        }
    }
}
