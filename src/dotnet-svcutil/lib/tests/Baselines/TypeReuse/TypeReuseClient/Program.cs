using System;

namespace TypeReuseClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var binLibrary = new BinLib.BinLibrary();
            var typesLibCT = new TypesLib.TypeReuseCompositeType();

            Console.WriteLine("Hello World!");
            Console.WriteLine($"{binLibrary}, {typesLibCT}");

            //var client = new WcfServiceTypeClient();
            //Console.WriteLine(client.WcfServiceTypeAsync(client));
        }
    }
}
