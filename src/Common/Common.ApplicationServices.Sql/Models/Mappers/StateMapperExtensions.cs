using System.Collections.Generic;
using System.Linq;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Common.ApplicationServices.Sql.Models.Mappers
{
    public static class StateMapperExtensions
    {
        public static IList<StateEntryModel> ToModels(this State sourceObject)
        {
            return sourceObject.Fields.Select(entry => new StateEntryModel
            {
                StateName = sourceObject.Name,
                ParameterName = entry.Name,
                ParameterValue = entry.Value
            }).ToList();
        }

        public static State ToDomain(this IEnumerable<StateEntryModel> sourceObject, string stateName)
        {
            //var name = sourceObject.FirstOrDefault()?.StateName;
            return new State
            {
                Name = stateName,
                Fields = sourceObject.Select(x => new Field(x.ParameterName, x.ParameterValue)).ToList()
            };
        }

        public static IList<State> ToDomain(this IEnumerable<StateEntryModel> sourceObject)
        {
            return sourceObject.GroupBy(x => x.StateName)
                .Select(x => new State()
                {
                    Name = x.Key,
                    Fields = x.Select(y => new Field(y.ParameterName, y.ParameterValue)).ToList()
                }).ToList();
        }
    }
}