using System.Collections.Generic;
using System.Linq;
using Bifrost.ClientAPI.Controllers.Mappers;
using Bifrost.ClientAPI.Controllers.Services;
using Bifrost.Dto.Apis;
using Bifrost.Dto.Dto;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.ApplicationServices.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Bifrost.ClientAPI.Controllers
{
    /// <summary>
    /// Provides methods for handling Asgard jobs and their states
    /// </summary>

    [Route("api/v1/[controller]")]
    public class JobController : AsgardController, IJobApi
    {
        private readonly Common.Core.ApplicationServices.IJobService _jobService;
        private readonly IStateService _stateService;
        private readonly IJobConfigurationMapper _configurationMapper;

        public JobController(Common.Core.ApplicationServices.IJobService jobService, IStateService stateService, IJobConfigurationMapper configurationMapper, ISecurityService securityService) 
      : base(securityService)
        {
            _jobService = jobService;
            _stateService = stateService;
            _configurationMapper = configurationMapper;
        }


        /// <summary>
        /// Retrieve the jobs corresponding to the given names or all if no names are supplies
        /// </summary>
        /// <param name="jobNames"></param>
        /// <returns></returns>
        [HttpGet("")]
        public IList<JobDto> GetJobs([FromQuery] string[] jobNames)
        {

            var jobs = jobNames.Any()
                ? _jobService.Get().Where(j => jobNames.Contains(j.Name))
                : _jobService.Get();
            return jobs
                .Select(x => x.ToDto(_configurationMapper))
                .ToList();
        }


        /// <summary>
        /// Get the states for the given jobs
        /// </summary>
        /// <param name="jobNames"></param>
        /// <returns></returns>
        [HttpGet("state")]
        public IList<JobStateDto> GetJobStates([FromQuery] string[] jobNames)
        {
            var states = jobNames.Any()
                ? jobNames.Select(name => _stateService.LoadState(name))
                : _stateService.LoadStates();
            return states
                .Select(x => x.ToDto())
                .ToList();
        }

        /// <summary>
        /// Save or update the given job
        /// </summary>
        /// <param name="job"></param>
        [HttpPut("")]
        public void EditJob([FromBody] JobDto job)
        {
            _jobService.SaveOrUpdate(job.ToDomain(_configurationMapper));
        }

     
        /// <summary>
        /// Delete job with the given name
        /// </summary>
        /// <param name="jobName"></param>
        [HttpDelete("")]
        public void DeleteJob([FromQuery] string jobName)
        {
            _jobService.Delete(jobName);
            _stateService.DeleteState(jobName);
        }

        /// <summary>
        /// Start the given jobs
        /// </summary>
        /// <param name="jobNames"></param>
        [HttpPut("enable")]
        public void EnableJob([FromQuery] string[] jobNames)
        {
            var jobs = jobNames.Any()
                ? _jobService.Get().Where(j => jobNames.Contains(j.Name))
                : _jobService.Get();
            foreach (var job in jobs)
            {
                job.Enabled = true;
                _jobService.SaveOrUpdate(job);
            }
            
        }

        /// <summary>
        /// Stop the given jobs
        /// </summary>
        /// <param name="jobNames"></param>
        [HttpPut("disable")]
        public void DisableJob([FromQuery] string[] jobNames)
        {
            var jobs = jobNames.Any()
                ? _jobService.Get().Where(j => jobNames.Contains(j.Name))
                : _jobService.Get();
            foreach (var job in jobs)
            {
                job.Enabled = false;
                _jobService.SaveOrUpdate(job);
            }
        }

        /// <summary>
        /// Reset the jobs with the given names, clearing their state
        /// </summary>
        /// <param name="jobNames"></param>
        [HttpPut("reset")]
        public void Reset([FromQuery] string[] jobNames)
        {
      
            var jobs = jobNames.Any()
                ? _jobService.Get().Where(j => jobNames.Contains(j.Name))
                : _jobService.Get();
            foreach (var job in jobs)
            {
                _stateService.DeleteState(job.Name);
            }
        }


        /// <summary>
        /// Get configuration templates for all available jobs, or for the jobs given
        /// </summary>
        /// <param name="jobTypeNames"></param>
        /// <returns></returns>
        [HttpGet("template")]
        public IList<JobConfigurationTemplateDto> GetAvailableJobConfigurationTemplates([FromQuery] string[] jobTypeNames)
        {
            var configs = jobTypeNames.Any()
                ? _configurationMapper.GetAllAvailable().Where(c => jobTypeNames.Contains(c.TypeName))
                : _configurationMapper.GetAllAvailable();
            return configs
                .Select(c => c.ToDto(_configurationMapper))
                .ToList();
        }
    }
}