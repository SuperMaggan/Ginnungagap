using System;
using System.Collections.Generic;
using System.Linq;
using Bifrost.ClientAPI.Controllers.Mappers;
using Bifrost.ClientAPI.Controllers.Services;
using Bifrost.Core.ApplicationServices.Interfaces;
using Bifrost.Dto.Apis;
using Bifrost.Dto.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Bifrost.ClientAPI.Controllers
{
    /// <summary>
    /// Provides methods for handling Asgard consumers (accounts)
    /// </summary>

    [Route("api/v1/[controller]")]
    public class ConsumerController : AsgardController, IConsumerApi
    {
        private readonly IConsumerService _consumerService;

        public ConsumerController(IConsumerService consumerService, ISecurityService securityService)
      :base(securityService)
        {
            _consumerService = consumerService;
        }

        

        /// <summary>
        /// Retrieves the consumer with the given name
        /// </summary>
        /// <param name="consumerNames">names of the consumers to retrive. If empty, retrieve all</param>
        /// <returns></returns>
        [HttpGet("")]
        public IList<ConsumerDto> GetConsumers([FromQuery] string[] consumerNames)
        {
          EnsureAdmin();
      if (consumerNames.Length == 1)
            {
                var consumer = _consumerService.GetConsumerByName(consumerNames.FirstOrDefault())
                    ?.ToDto();
                return consumer == null
                    ? new List<ConsumerDto>()
                    : new List<ConsumerDto>() {consumer};
            }
            if (!consumerNames.Any())
                return _consumerService.GetAllConsumers().Select(x => x.ToDto()).ToList();
            return _consumerService.GetAllConsumers().Where(x => consumerNames.Contains(x.Name))
                .Select(x => x.ToDto())
                .ToList();
        }

        /// <summary>
        /// Save or update the given consumer
        /// </summary>
        /// <param name="consumer"></param>
        [HttpPut("")]
        public void EditConsumer([FromBody] ConsumerDto consumer)
        {
          EnsureAdmin();
      _consumerService.SaveOrUpdate(consumer.ToDomain());
        }

        /// <summary>
        /// Deletes the consumer with the given name
        /// </summary>
        /// <param name="consumerName"></param>
        [HttpDelete("")]
        public void DeleteConsumer([FromQuery] string consumerName)
        {
          EnsureAdmin();
            if (string.IsNullOrEmpty(consumerName))
                throw new Exception("Need to supply a consumer name when calling DeleteConsumer(string consumerName)");
            _consumerService.DeleteConsumer(consumerName);
        }
    }
}