using System.Collections.Generic;

namespace Bifrost.Core.Domain
{
    public class Consumer
    {
        public string ApiKey { get; set; }
        public string Name { get; set; }

        public bool IsAdmin { get; set; }

        /// <summary>
        /// Can this consumer edit/create/delete
        /// </summary>
        public bool CanEdit {get; set; }

        /// <summary>
        /// A list of jobs that is available for this consumer
        /// </summary>
        public IList<string> Jobs{ get; set; }

    }
}