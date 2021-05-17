using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bifrost.Core.Connectors.States.SqlDatabase;
using Bifrost.Dto.Dto;
using Bifrost.Common.Core.ApplicationServices.Helpers;
using Bifrost.Common.Core.Domain;
using Bifrost.Common.Core.Domain.Jobs;

namespace Bifrost.ClientAPI.Controllers.Mappers
{
    public static class JobExtensions
    {
        public static JobDto ToDto(this Job job, IJobConfigurationMapper mapper)
        {
            if (job == null)
                return null;
            return new JobDto()
            {
                Name = job.Name,
                Description = job.Description,
                ConcurrentLimit = job.ConcurrentLimit,
                Enabled = job.Enabled,
                LastUpdated = job.LastUpdated,
                TriggerCronSyntax = job.TriggerCronSyntax,
                Configuration = mapper.Serialize(job.Configuration)
            };
        }

        public static Job ToDomain(this JobDto job, IJobConfigurationMapper mapper)
        {
            if (job == null)
                return null;

            return new Job()
            {
                Name = job.Name,
                Description = job.Description,
                ConcurrentLimit = job.ConcurrentLimit,
                Enabled = job.Enabled,
                LastUpdated = job.LastUpdated,
                TriggerCronSyntax = job.TriggerCronSyntax,
                Configuration = mapper.Deserialize(job.Configuration)
            };
        }

        public static JobStateDto ToDto(this State state)
        {
            if (state == null)
                return null;



            var jobState = new JobState(state);

            return new JobStateDto()
            {
                JobName = jobState.Name,
                LastExecutionDate = jobState.LastExecutionDate,
                LastExecutionDurationMs = jobState.LastExecutionTimeMs,
                Message = jobState.Message,
                Status = jobState.Status.ToDto(),
                Error = jobState.GetJobError().ToDto(),
                Progress = jobState.GetProgressDto(),
                Custom = jobState.GetCustomFields().ToDto()
            };
        }


        public static JobProgressDto GetProgressDto(this JobState state)
        {
            //todo: smarter logic
            var dbstate = new DatabaseJobState(state);
            return new JobProgressDto()
            {
                Total = dbstate.LastDiscoverBatchCount,
                Current = dbstate.BatchCount
            };
        }

        public static IList<FieldDto> ToDto(this IList<Field> fields)
        {
            return fields.Select(x => new FieldDto() {Name = x.Name, Value = x.Value}).ToList();
        }

        public static JobStatusDto ToDto(this JobStatus status)
        {
            switch (status)
            {
                case JobStatus.Ok:
                    return JobStatusDto.Ok;
                case JobStatus.Warning:
                    return JobStatusDto.Warning;
                case JobStatus.Error:
                    return JobStatusDto.Error;
                default:
                    return JobStatusDto.Ok;

            }
        }

        public static ErrorDto ToDto(this JobError error)
        {
            if (error == null)
                return new ErrorDto();
            return new ErrorDto()
            {
                Date = error.Date,
                Message = error.Message,
                StackTrace = error.StackTrace
            }
                ;
        }

        public static JobConfigurationTemplateDto ToDto(this IJobConfiguration configuration, IJobConfigurationMapper mapper)
        {
            return new JobConfigurationTemplateDto()
            {
                Description = configuration.Description,
                DisplayName = configuration.DisplayName,
                ExampleConfiguration = mapper.Serialize(configuration.CreateExample())
            };
        }
    }


}
