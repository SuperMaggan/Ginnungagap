using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Bifrost.Common.ApplicationServices.Sql.Common;
using Bifrost.Common.ApplicationServices.Sql.Databases;
using Bifrost.Common.ApplicationServices.Sql.Models;
using Bifrost.Common.ApplicationServices.Sql.Models.Mappers.Jobs;
using Bifrost.Common.ApplicationServices.Sql.Tables;
using Bifrost.Common.Core.ApplicationServices;
using Bifrost.Common.Core.ApplicationServices.Helpers;
using Bifrost.Common.Core.Domain.Jobs;
using Serilog;

namespace Bifrost.Common.ApplicationServices.Sql
{
    public class SqlJobService : IJobService
    {
        private readonly DatabaseBase _database;
        private readonly IJobConfigurationMapper _configurationMapper;

        public SqlJobService(JobDatabase database, IJobConfigurationMapper configurationMapper) 
        {
            _database = database;
            _configurationMapper = configurationMapper;
            _database.SetupOnce();
        }

        public IList<Job> Get()
        {
            using (var connection = _database.GetOpenConnection())
            {
                var jobModels = connection.Query<JobModel>("SELECT * FROM " + JobTable.TableName).ToList();
                var jobs = jobModels.Select(model => model.ToDomain(_configurationMapper));
                return jobs.ToList();
            }
        }

        public Job Get(string name)
        {
            using (var connection = _database.GetOpenConnection())
            {
                var jobModel =
                    connection.Query<JobModel>(string.Format("SELECT * FROM {0} WHERE Name=@Name", JobTable.TableName),
                        new {Name = name})
                        .FirstOrDefault();
                return jobModel?.ToDomain(_configurationMapper);
            }
        }

        public void Delete(string name)
        {
            using (var connection = _database.GetOpenConnection())
            {
                connection.Execute(string.Format("DELETE FROM {0} WHERE Name = @Name",
                    JobTable.TableName), new {Name = name});
            }
        }

        public void SaveOrUpdate(Job job)
        {
            job.LastUpdated = DateTime.UtcNow;
            using (var connection = _database.GetOpenConnection())
            {
                var jobModel = job.ToModel(_configurationMapper);
                if (
                    connection.Query<JobModel>(
                        string.Format("SELECT Name FROM {0} WHERE Name=@Name", JobTable.TableName), new {job.Name})
                        .Any())
                    connection.Update(jobModel);
                else
                    connection.Insert(jobModel);
            }
        }
    }
}