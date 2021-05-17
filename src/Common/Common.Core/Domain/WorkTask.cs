using System.Collections.Generic;
using System.Linq;

namespace Bifrost.Common.Core.Domain
{
    /// <summary>
    ///     Represents a batch of information
    /// </summary>
    public class WorkTask
    {
        public WorkTask(string groupName, string id, State instructions)
        {
            Id = id;
            GroupName = groupName;
            Instructions = instructions.Fields.Select(f => f.Value).ToList();
        }

        public WorkTask(string groupName, string id, IList<string> instructions)
        {
            Id = id;
            GroupName = groupName;
            Instructions = instructions;
        }

        public string GroupName { get; private set; }
        public string Id { get; private set; }
        public IList<string> Instructions { get; private set; }
    }
}