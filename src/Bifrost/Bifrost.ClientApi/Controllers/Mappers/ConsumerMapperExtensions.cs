using System;
using Bifrost.Core.Domain;
using Bifrost.Dto.Dto;

namespace Bifrost.ClientAPI.Controllers.Mappers
{
    /// <summary>
    /// Mapping extensions related to Consumers
    /// </summary>
    public static class ConsumerMapperExtensions
    {
        //public static ConsumerDto toDto(this IList<Consumer> consumers)
        //{

        //    return new ConsumersOverviewModel()
        //    {
        //        Consumers = consumers,
        //        AvailableJobNames = availableJobs.Select(x=>x.Name).ToList()
        //    };
        //}

        public static Consumer ToDomain(this ConsumerDto dto)
        {
            if(string.IsNullOrEmpty(dto?.Name))
                throw new Exception("A consumer needs to have a name");
            return new Consumer()
            {
                Name = dto.Name,
                ApiKey = dto.ApiKey,
                IsAdmin = dto.IsAdmin,
                CanEdit = dto.CanEdit,
                Jobs = dto.AvailableJobNames
            };
        }
        public static ConsumerDto ToDto(this Consumer consumer)
        {
            return new ConsumerDto()
            {
                Name = consumer.Name,
                ApiKey = consumer.ApiKey,
                IsAdmin = consumer.IsAdmin,
                CanEdit = consumer.CanEdit,
                AvailableJobNames = consumer.Jobs
            };
        }
    }
}