using Bifrost.Core.Domain;
using Bifrost.Data.Sql.Databases.Bifrost.Models;

namespace Bifrost.Data.Sql.Databases.Bifrost.Mappers
{
    public static class ConsumerMappingExtensions
    {
        public static Consumer ToDomain(this ConsumerModel model)
        {
            return new Consumer()
            {
                Name = model.Name,
                ApiKey = model.ApiKey,
                CanEdit = model.CanEdit,
                IsAdmin = model.IsAdmin,
                Jobs = model.JobsCsv.ToCsvArray()
            };
        }

        public static ConsumerModel ToModel(this Consumer consumer)
        {
            return new ConsumerModel()
            {
                Name = consumer.Name,
                ApiKey = consumer.ApiKey,
                CanEdit = consumer.CanEdit,
                IsAdmin = consumer.IsAdmin,
                JobsCsv = consumer.Jobs.ToCsvString()
            };
        }
    }
}