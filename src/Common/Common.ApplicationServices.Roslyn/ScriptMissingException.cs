using System;

namespace Bifrost.Common.ApplicationServices.Roslyn
{
    public class ScriptMissingException : Exception{
        public ScriptMissingException(string scriptName)
            :base("No script found named " + scriptName)
        {
        }
    }
}