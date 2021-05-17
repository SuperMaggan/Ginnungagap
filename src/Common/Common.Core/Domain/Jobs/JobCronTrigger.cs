namespace Bifrost.Common.Core.Domain.Jobs
{
    public class JobCronTrigger
    {
        public string Name { get; set; }
        public string CronExpression { get; set; }
    }
}