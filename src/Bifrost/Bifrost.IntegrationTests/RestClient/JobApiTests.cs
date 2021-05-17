using System.Linq;
using Bifrost.Dto.Dto;
using Bifrost.RestClient.Api;
using FluentAssertions;
using Xunit;

namespace Bifrost.IntegrationTests.RestClient
{
 
    public class JobApiTests
    {


        [Fact]
        public void Can_add_and_load_job()
        {
            var client = new RestJobApi(new AutoResolveScyllaRestClientSettings());
            
            var job = new JobDto()
            {
                Name = "TestLoadJob",
                Description = "blalba",
                ConcurrentLimit = 2,
                Enabled = false,
                TriggerCronSyntax = "* */2 * * * ?",
                Configuration = @"<WebConnectorJobConfiguration>  
                                    <StartUrl>http://blog.cwa.me.uk</StartUrl>  
                                    <Depth>3</Depth>  
                                    <NumberOfPagesPerExecution>10</NumberOfPagesPerExecution>  
                                    <LinkFilter>    
                                        <ExcludeAsDefault>false</ExcludeAsDefault>    
                                        <ExcludeExtensions>      
                                            <string>js</string>     
                                        </ExcludeExtensions>    
                                        <ExcludeRegex />    
                                        <IncludeRegex />    
                                        <ExcludePagesWithQueryParameters>true</ExcludePagesWithQueryParameters>    
                                        <StayOnHost>true</StayOnHost>  
                                    </LinkFilter>  
                                    <PageFilter>    
                                        <ExcludeBinaryPages>false</ExcludeBinaryPages>  
                                    </PageFilter>  
                                    <DefaultVerifyFrequency>    
                                        <Days>1</Days>    
                                        <Hours>0</Hours>    
                                        <Minutes>0</Minutes>  
                                    </DefaultVerifyFrequency>
                                </WebConnectorJobConfiguration>"
            };
            client.EditJob(job);

            var fetchedJob = client.GetJobs(job.Name).FirstOrDefault();
            //fetchedJob.ShouldBeEquivalentTo(job, o => o.Excluding(x=>x.LastUpdated).Excluding(x=>x.Configuration));
        }

        [Fact]
        public void Can_add_and_load_and_delete_job()
        {
            var client = new RestJobApi(new AutoResolveScyllaRestClientSettings());
            var job = new JobDto()
            {
                Name = "TestSqlJob",
                Description = "blalba",
                ConcurrentLimit = 2,
                Enabled = false,
                TriggerCronSyntax = "* */2 * * * ?",
                Configuration = @"<SqlDatabaseConnectorJobConfiguration>
                                    <ConnectionString>Server=SECC5399;Database=Asgard;Trusted_Connection=True;Connection Timeout=200</ConnectionString>
                                    <IntegrationType>OracleIntegration</IntegrationType>
                                    <BatchSize>100</BatchSize>
                                    <ResetEveryXHour>-1</ResetEveryXHour>
                                    <MainTable>
                                        <TableName>Documents</TableName>
                                        <PrimaryKeyName>Id</PrimaryKeyName>
                                        <PrimaryKeyIsInteger>false</PrimaryKeyIsInteger>
                                    </MainTable>
                                    <ChangeTables>
                                        <TableDetail>
                                            <TableName>Document</TableName>
                                            <PrimaryKeyName>Id</PrimaryKeyName>
                                            <PrimaryKeyIsInteger>false</PrimaryKeyIsInteger>
                                        </TableDetail>
                                    </ChangeTables>
                                    <EventTables>
                                        <EventTable>
                                            <TableName>Document_Changes</TableName>
                                            <EventSequenceColumnName>SEQ</EventSequenceColumnName>
                                            <MainTableIdColumnName>ID</MainTableIdColumnName>
                                            <EventTypeColumnName>ACTION</EventTypeColumnName>
                                            <DeleteEventTypeValue>DELETE</DeleteEventTypeValue>
                                        </EventTable>
                                    </EventTables>
                                    <ForeignSources />
                                </SqlDatabaseConnectorJobConfiguration>"
            };
            client.EditJob(job);

            var fetchedJob = client.GetJobs(job.Name).FirstOrDefault();
            //fetchedJob.ShouldBeEquivalentTo(job, o => o.Excluding(x => x.LastUpdated).Excluding(x => x.Configuration));

            client.DeleteJob(job.Name);
            var fetchedJobs = client.GetJobs(job.Name);
            fetchedJobs.Should().BeEmpty();
        }

        [Fact]
        public void Can_add_load_enable_disable_job()
        {
            var client = new RestJobApi(new AutoResolveScyllaRestClientSettings());
            var job = new JobDto()
            {
                Name = "TestFileJob",
                Description = "blalba",
                ConcurrentLimit = 12,
                Enabled = false,
                TriggerCronSyntax = "* */2 * * * ?",
                Configuration = @"<FileConnectorJobConfiguration><RootDirectory>D:\\Documents</RootDirectory><CrawlRecursively>true</CrawlRecursively><BatchSizeLimit>100</BatchSizeLimit></FileConnectorJobConfiguration>"
            };
            client.EditJob(job);

            var fetchedJob = client.GetJobs(job.Name).FirstOrDefault();
            //fetchedJob.ShouldBeEquivalentTo(job, o => o.Excluding(x => x.LastUpdated).Excluding(x => x.Configuration));

            client.EnableJob(job.Name);
            fetchedJob = client.GetJobs(job.Name).FirstOrDefault();
            fetchedJob.Enabled.Should().BeTrue();

            client.DisableJob(job.Name);
            fetchedJob = client.GetJobs(job.Name).FirstOrDefault();
            fetchedJob.Enabled.Should().BeFalse();
        }
    }
}
