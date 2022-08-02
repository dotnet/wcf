using System;


namespace Infrastructure.Common
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
    public class FailFastAfterAttribute : Attribute
    {
        public FailFastAfterAttribute(string durationString)
        {
            FailTime = TimeSpan.Parse(durationString);
        }

        public TimeSpan FailTime { get; }
    }
}
