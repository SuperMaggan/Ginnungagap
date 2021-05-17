using System.Collections.Generic;
using Bifrost.Dto.Apis;
using Bifrost.Dto.Dto;
using Bifrost.RestClient.Internal;

namespace Bifrost.RestClient.Api
{
    public class RestJobApi : RestClientBase, IJobApi
    {
        private readonly string _url;

        public RestJobApi(IAsgardRestClientSettings settings) : base(settings)
        {
            _url = $"{settings.AsgardApiUrl.Trim('/')}/api/v1";
        }

        public IList<JobDto> GetJobs(params string[] jobNames)
        {
         
            return Get<List<JobDto>>($"{_url}/job?{QueryParameter("jobNames", jobNames)}");
        }

        public IList<JobStateDto> GetJobStates(params string[] jobNames)
        {
           
            return Get<List<JobStateDto>>($"{_url}/job/state?{QueryParameter("jobNames", jobNames)}");
        }

        public void EditJob(JobDto job)
        {
            Put($"{_url}/job", job);
        }

        public void DeleteJob(string jobName)
        {
            Delete($"{_url}/job?jobName={jobName}");
        }

        public void EnableJob(params string[] jobNames)
        {
           
            Put($"{_url}/job/enable?{QueryParameter("jobNames", jobNames)}");
        }


        public void DisableJob(params string[] jobNames)
        {
          
            Put($"{_url}/job/disable?{QueryParameter("jobNames", jobNames)}");
        }

        public void Reset(params string[] jobNames)
        {
   
            Put($"{_url}/job/reset?{QueryParameter("jobNames",jobNames)}");
        }

        public IList<JobConfigurationTemplateDto> GetAvailableJobConfigurationTemplates(params string[] jobTypeNames)
        {
            return Get<List<JobConfigurationTemplateDto>>($"{_url}/job/template?{QueryParameter("jobTypeNames", jobTypeNames)}");
        }
    }
}