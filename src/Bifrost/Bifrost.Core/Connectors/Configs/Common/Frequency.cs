using System;

namespace Bifrost.Core.Connectors.Configs.Common
{
    public class Frequency
    {
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public TimeSpan ToTimeSpan()
        {
            return new TimeSpan(Days, Hours, Minutes,0);
        }
    }
}
