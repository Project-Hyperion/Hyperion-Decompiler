using System;
using System.IO;
using System.Linq;

namespace HyperionDecompilerv2
{
    class Program
    {
        static void Main(string[] args)
        {
            Decompiler.Decompiler d = new Decompiler.Decompiler(ToByteArray(File.ReadAllText("./bytecode2.txt").Trim()));
            d.Decompile();
        }

        public static byte[] ToByteArray(string Bytecode)
        {
            byte[] bytes = Bytecode.Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
            return bytes;
        }
    }
}
