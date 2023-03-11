using System;
using System.Threading.Tasks;
using CoreWCF.IdentityModel.Selectors;

namespace WCFCorePerfService
{
    public class MyCustomValidator : UserNamePasswordValidator
    {
        public override ValueTask ValidateAsync(string userName, string password)
        {            
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                return default;
            }
            return new ValueTask(Task.FromException(new Exception("username and password cannot be empty")));
        }
    }
}
