using System.Collections.Generic;

namespace Bifrost.Dto.Dto
{
    public class ConsumerDto
    {
        /// <summary>
        /// Secret password associated with this consumer
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Name and identifier for this Consumer
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Has this Consumer been granted super user rights
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Can this consumer edit/create/delete
        /// </summary>
        public bool CanEdit { get; set; }

        /// <summary>
        /// A list of job names that is available for this consumer
        /// </summary>
        public IList<string> AvailableJobNames { get; set; } = new List<string>();
    }

   
}