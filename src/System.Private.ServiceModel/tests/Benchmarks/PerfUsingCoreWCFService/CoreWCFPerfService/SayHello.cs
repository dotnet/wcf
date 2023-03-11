using System.Threading.Tasks;

namespace WCFCorePerfService
{
    public class SayHello : ISayHello
    {
        public Task<string> HelloAsync(string name)
        {
            return Task.FromResult(name);
        }
    }
}
