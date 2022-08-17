using CoreWCF.IdentityModel.Selectors;
using System;
using System.Threading.Tasks;

namespace WCFCorePerfService
{
    public class MyCustomValidator : UserNamePasswordValidator
    {
        public override ValueTask ValidateAsync(string userName, string password)
        {            
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                return new ValueTask(Task.CompletedTask);
            }
            return new ValueTask(Task.FromException(new Exception("username and password cannot be empty")));
        }
    }
}
