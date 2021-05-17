using System.Collections.Generic;
using Bifrost.Dto.Dto;

namespace Bifrost.Dto.Apis
{
    /// <summary>
    /// Provides and interface for handling different type of jobs
    /// </summary>
    public interface IJobApi
    {
        /// <summary>
        /// Retrieve the jobs corresponding to the given names
        /// </summary>
        /// <param name="jobNames"></param>
        /// <returns></returns>
        IList<JobDto> GetJobs(string[] jobNames);

     
        /// <summary>
        /// Get the states for the given jobs
        /// </summary>
        /// <param name="jobNames"></param>
        /// <returns></returns>
        IList<JobStateDto> GetJobStates(string[] jobNames);

        /// <summary>
        /// Save or update the given job
        /// </summary>
        /// <param name="job"></param>
        void EditJob(JobDto job);

        /// <summary>
        /// Delete job with the given name
        /// </summary>
        /// <param name="jobName"></param>
        void DeleteJob(string jobName);

        /// <summary>
        /// Start the given jobs
        /// </summary>
        /// <param name="jobNames"></param>
        void EnableJob(string[] jobNames);

        /// <summary>
        /// Stop the given jobs
        /// </summary>
        /// <param name="jobNames"></param>
        void DisableJob(string[] jobNames);

        /// <summary>
        /// Reset the jobs with the given names, clearing their state
        /// </summary>
        /// <param name="jobNames"></param>
        void Reset(string[] jobNames);

        /// <summary>
        /// Get configuration templates for all available jobs, or for the jobs given
        /// </summary>
        /// <param name="jobTypeNames"></param>
        /// <returns></returns>
        IList<JobConfigurationTemplateDto> GetAvailableJobConfigurationTemplates(string[] jobTypeNames);
    }
}