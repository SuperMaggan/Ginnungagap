using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Bifrost.Common.Core.Domain.Jobs;
using Bifrost.Common.QuartzIntegration.Domain;

namespace Bifrost.Common.QuartzIntegration
{
    /// <summary>
    ///     Currently supports serializing of version 2.0 of Quartz.Net quartz_jobs.xml file
    /// </summary>
    public class XmlFileQuartzJobConfigurator : IQuartzJobConfigurator
    {
        private const string DefaultTriggerGroupName = "DEFAULT";
        private const string DefaultJobGroupName = "DEFAULT";
        private const string ConcurrentJobLimitParameterName = "ConcurrentJobLimit";
        private static readonly XNamespace DefaultNamespace = @"http://quartznet.sourceforge.net/JobSchedulingData";
        private static readonly XNamespace Xsi = @"http://www.w3.org/2001/XMLSchema-instance";
        private readonly string _xmlJobFilePath = "quartz_jobs.xml";

        public IList<TypedJob> GetJobs()
        {
            if (!File.Exists(_xmlJobFilePath))
                throw new FileNotFoundException("Cant find a quartz file at " + _xmlJobFilePath);

            var xDoc = XDocument.Load(_xmlJobFilePath);

            var jobs = ParseJobs(xDoc.Root.Element(DefaultNamespace + "schedule"));
            var triggers = ParseCronTriggers(xDoc.Root.Element(DefaultNamespace + "schedule"));

            foreach (var job in jobs)
            {
                if (triggers.ContainsKey(job.Name))
                    job.TriggerCronSyntax = triggers[job.Name].CronExpression;
            }

            return jobs;
        }

        public void SaveOrUpdateJob(IList<TypedJob> jobs)
        {
            var xRoot = CreateRootElement();
            xRoot.Add(CreateScheduleElement(jobs));

            if (File.Exists(_xmlJobFilePath))
                File.Delete(_xmlJobFilePath);
            using(var stream = File.CreateText(_xmlJobFilePath)) { 
                xRoot.Save(stream);
            }
        }

        public IList<TypedJob> GetJobs(XDocument xDoc)
        {
            if (xDoc == null)
                return new TypedJob[0];

            var jobs = ParseJobs(xDoc.Root.Element(DefaultNamespace + "schedule"));
            var triggers = ParseCronTriggers(xDoc.Root.Element(DefaultNamespace + "schedule"));

            foreach (var job in jobs)
            {
                if (triggers.ContainsKey(job.Name))
                    job.TriggerCronSyntax = triggers[job.Name].CronExpression;
            }

            return jobs;
        }

        /*
         * Deserializing
         */

        private IDictionary<string, JobCronTrigger> ParseCronTriggers(XElement xSchedule)
        {
            if (xSchedule == null)
                return new Dictionary<string, JobCronTrigger>();
            var xTriggerElements = xSchedule.Elements(DefaultNamespace + "trigger");
            var triggers = new Dictionary<string, JobCronTrigger>();

            foreach (var xTrigger in xTriggerElements.Select(x => x.Element(DefaultNamespace + "cron")))
            {
                var xName = xTrigger.Element(DefaultNamespace + "name");
                var xJobName = xTrigger.Element(DefaultNamespace + "job-name");
                var xCronExpression = xTrigger.Element(DefaultNamespace + "cron-expression");
                if (xName != null && xJobName != null && xCronExpression != null)
                    triggers.Add(xJobName.Value,
                        new JobCronTrigger
                        {
                            Name = xName.Value,
                            CronExpression = xCronExpression.Value
                        });
            }
            return triggers;
        }

        private IList<TypedJob> ParseJobs(XElement xSchedule)
        {
            if (xSchedule == null)
                return new List<TypedJob>();
            var xJobElements = xSchedule.Elements(DefaultNamespace + "job");
            var jobs = new List<TypedJob>();

            foreach (var xJobElement in xJobElements)
            {
                var xName = xJobElement.Element(DefaultNamespace + "name");
                var xDescription = xJobElement.Element(DefaultNamespace + "description");
                var xJobType = xJobElement.Element(DefaultNamespace + "job-type");
                var xDataMap = xJobElement.Element(DefaultNamespace + "job-data-map");
                if (xName != null && xDescription != null && xJobType != null)
                    jobs.Add(new TypedJob
                    {
                        Name = xName.Value,
                        JobType = Type.GetType(xJobType.Value),
                        Description = xDescription.Value,
                        ConcurrentLimit = ParseConcurrentLimit(xDataMap)
                    });
            }
            return jobs;
        }

        private int ParseConcurrentLimit(XElement xJobDataMap)
        {
            if (xJobDataMap == null)
                return 0;
            var xEntries = xJobDataMap.Elements(DefaultNamespace + "entry");

            foreach (var entry in xEntries)
            {
                var key = entry.Element(DefaultNamespace + "key");
                var value = entry.Element(DefaultNamespace + "value");

                if (key == null || value == null)
                    continue;
                if (key.Name == ConcurrentJobLimitParameterName)
                    return int.Parse(value.Value);
            }
            return 0;
        }

        /*
         * Serializing
         */

        private XElement CreateRootElement()
        {
            var root = new XElement(DefaultNamespace + "job-scheduling-data");

            var processingElement = new XElement(DefaultNamespace + "processing-directives");
            processingElement.Add(new XElement(DefaultNamespace + "overwrite-existing-data", true));

            root.Add(processingElement);
            root.Add(new XAttribute("version", "2.0"));


            return root;
        }

        private XElement CreateScheduleElement(IList<TypedJob> jobs)
        {
            var xSchedule = new XElement(DefaultNamespace + "schedule",
                jobs.Select(CreateJobElement),
                jobs.Select(CreateTriggerElement)
                );
            return xSchedule;
        }

        private XElement CreateJobElement(TypedJob job)
        {
            var xName = new XElement(DefaultNamespace + "name", job.Name);
            var xGroup = new XElement(DefaultNamespace + "group", DefaultJobGroupName);
            var xDesc = new XElement(DefaultNamespace + "description", job.Description);
            var xJobType = new XElement(DefaultNamespace + "job-type", job.JobType);
            var xDurable = new XElement(DefaultNamespace + "durable", true);
            var xRecover = new XElement(DefaultNamespace + "recover", false);
            var xJobDataMap = CreateJobDataMapElement(job);

            var xJob = new XElement(DefaultNamespace + "job",
                xName, xGroup, xDesc, xJobType, xDurable, xRecover, xJobDataMap);
            return xJob;
        }

        private XElement CreateJobDataMapElement(Job job)
        {
            var xEntries = new XElement(DefaultNamespace + "job-data-map");

            xEntries.Add(new XElement(DefaultNamespace + "entry",
                new XElement(DefaultNamespace + "key", ConcurrentJobLimitParameterName),
                new XElement(DefaultNamespace + "value", job.ConcurrentLimit)));
            return xEntries;
        }

        private XElement CreateTriggerElement(Job job)
        {
            var xName = new XElement(DefaultNamespace + "name", GetTriggerName(job.Name));
            var xGroup = new XElement(DefaultNamespace + "group", DefaultTriggerGroupName);
            var xJobName = new XElement(DefaultNamespace + "job-name", job.Name);
            var xJobGroup = new XElement(DefaultNamespace + "job-group", DefaultJobGroupName);
            var xMissfireInstructions = new XElement(DefaultNamespace + "misfire-instruction", "DoNothing");
            var xCronExpression = new XElement(DefaultNamespace + "cron-expression", job.TriggerCronSyntax);

            var xCronRoot = new XElement(DefaultNamespace + "cron",
                xName, xGroup, xJobName, xJobGroup, xMissfireInstructions, xCronExpression);

            var xTriggerRoot = new XElement(DefaultNamespace + "trigger", xCronRoot);
            return xTriggerRoot;
        }

        private string GetTriggerName(string jobname)
        {
            return $"{jobname}-trigger";
        }
    }
}