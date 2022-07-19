using System;
namespace SvcutilBootstrap {
    public class Program {
        public static int Main(string[] args) {
            return Microsoft.Tools.ServiceModel.Svcutil.Tool.Main(args);
        }
    }
}