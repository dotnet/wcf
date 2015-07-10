using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bridge.Tests.Commands
{
    public class TestCommand
    {
        public const string Value = "Test";
        public string Execute() {
            return Value;
        }
    }
}
