using Bifrost.Common.Core.ApplicationServices.Helpers;
using Bifrost.Common.Core.Domain.Jobs;

namespace Bifrost.Common.ApplicationServices.Sql.Models.Mappers.Jobs
{
    public static class JobMappingExtensions
    {
        public static Job ToDomain(this JobModel model, IJobConfigurationMapper configMapper)
        {
            if (model == null)
                return null;
            
            return new Job
            {
                Name = model.Name,
                Description = model.Description,
                ConcurrentLimit = model.ConcurrentLimit,
                Enabled = model.Enabled,
                //JobType = model.JobType,
                TriggerCronSyntax = model.TriggerCronSyntax,
                Configuration = configMapper.Deserialize(model.Configuration),
                LastUpdated = model.LastUpdated
            };
        }

        public static JobModel ToModel(this Job job, IJobConfigurationMapper configMapper)
        {
            if (job == null)
                return null;
            return new JobModel
            {
                Name = job.Name,
                Description = job.Description,
                ConcurrentLimit = job.ConcurrentLimit,
                Enabled = job.Enabled,
                JobType = job.Configuration?.TypeName ?? "",
                //JobType = job.JobType,
                TriggerCronSyntax = job.TriggerCronSyntax,
                Configuration = configMapper.Serialize(job.Configuration),
                LastUpdated = job.LastUpdated
            };
        }
    }
}