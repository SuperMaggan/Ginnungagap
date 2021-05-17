using System;
using System.Collections.Generic;

namespace Bifrost.Common.Core.Domain
{
    public class Script
    {
        public Script()
        {
            ImportedScripts = new List<Script>();
        }
        public string Name { get; set; }
        public string DomainName { get; set; }
        public string Code { get; set; }

        public string Owner { get; set; }
        public bool IsPublic { get; set; }

        public DateTime LastUpdated { get; set; }
        public IList<Script> ImportedScripts { get; set; }
        
        public string Metadata { get; set; }

    }
}