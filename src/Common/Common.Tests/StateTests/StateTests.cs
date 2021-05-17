using System;
using System.Diagnostics;
using Bifrost.Common.Core.Domain;
using Bifrost.Common.Core.Domain.Jobs;
using FluentAssertions;
using Xunit;

namespace Bifrost.Common.Tests.StateTests
{
    public class StateTests
    {
        [Fact]
        public void CanSerializeReflectionState()
        {
            var state = new DatabaseJobState
            {
                Name = "Test1",
                State = DatabaseJobState.JobState.InitialCrawling,
                BatchCount = 2,
                DiscoverCount = 4,
                ErrorCount = 11,
                LastWorkingState = DatabaseJobState.JobState.Discovering,
                Message = "Hej",
                RecentErrorMessage = "Hej hej heje hej",
                RecentErrorStackTrace = 
                    "alskdjaslk jalskjd alksjd alsjd lkasjdlka jsdlkjaslkdj alskjd lakjsd lakjsd kalsjdaslkdj  aslkjdas lkjasdlkjas dlkjas dlkasdjas dlkj",
                RecentErrorDate = new DateTime?(new DateTime(2000,1,1)),
                IsActive = true
            };
            state.SetValue("custom_field", "");
            var fields = state.Fields;
               
            fields.Should().HaveCount(14);
            

            var state2 = new DatabaseJobState
            {
                Name = state.Name,
                Fields = fields
            };
            state.Should().Be(state2);

            var error = state.GetJobError();
            error.Should().NotBeNull();
            error.Date.Should().Be(state.RecentErrorDate.Value);
            error.Message.Should().Be(state.RecentErrorMessage);

            state.GetCustomFields().Count.Should().Be(1);
        }

        [Fact]
        public void CanSerializeState()
        {
            var state = new DatabaseJobState
            {
                Name = "Test1",
                State = DatabaseJobState.JobState.Discovering
            };
            state.State = DatabaseJobState.JobState.InitialCrawling;
            state.BatchCount = 3;
            state.DiscoverCount = 4;
            state.SetErrorState(new Exception("sadsd"));
            state.Message = "hejaasd jlkasdlkjas";
            var fields = state.Fields;
            fields.Should().HaveCount(12);

            state.RecentErrorDate =
                state.RecentErrorDate.Value.AddTicks(-(state.RecentErrorDate.Value.Ticks % TimeSpan.TicksPerSecond));
            var state2 = new DatabaseJobState(state);
            state.Should().Be(state2);
        }

        [Fact]
        public void CanSetFieldsThatAreNotAProperty()
        {
            var state = new DatabaseJobState
            {
                Name = "Test1",
                State = DatabaseJobState.JobState.Discovering
            };

            var fieldName = "capella";
            var fieldValue = "first";
            state.SetValue(fieldName, fieldValue);

            var setField = state.GetValue(fieldName);
            setField.Should().Be(fieldValue);

            var fields = state.Fields;
            fields.Should().Contain(x => x.Name == fieldName && x.Value == fieldValue);
        }

        [Fact]
        public void CustomFieldsArePersistedWhenCopyingToANewState()
        {
            var state1 = new DatabaseJobState
            {
                Name = "Test1",
                State = DatabaseJobState.JobState.Discovering
            };

            var fieldName = "capella";
            var fieldValue = "first";
            state1.SetValue(fieldName, fieldValue);

            var setField = state1.GetValue(fieldName);
            setField.Should().Be(fieldValue);

            var fields = state1.Fields;
            fields.Should().Contain(x => x.Name == fieldName && x.Value == fieldValue);

            var state2 = new DatabaseJobState(state1);
            state2.Fields.Should().AllBeEquivalentTo(state1.Fields);
        }

        [Fact(Skip = "Only used for measuring")]
        public void CompareStateSpeed()
        {
            var iterations = 100;

            Console.WriteLine("Measuring state with reflection using " + iterations);
            var withReflection = MeasureTime(CanSerializeReflectionState, iterations);
            Console.WriteLine("Measuring state without reflection using " + iterations);
            var withoutReflection = MeasureTime(CanSerializeState, iterations);
            Console.WriteLine("With reflection: " + withReflection.TotalMilliseconds + " ms");
            Console.WriteLine("Without reflection: " + withoutReflection.TotalMilliseconds + " ms");
        }

        private TimeSpan MeasureTime(Action action, int numIterations)
        {
            var timer = new Stopwatch();
            timer.Start();
            for (var i = 0; i < numIterations; i++)
            {
                action.Invoke();
            }
            timer.Stop();
            return timer.Elapsed;
        }

        internal class DatabaseJobState : JobState
        {
            public enum JobState
            {
                NotDefined,
                Error,
                Discovering,
                InitialCrawling,
                IncrementalCrawling,
                ForeignTable,
                Paused
            }

            public DatabaseJobState()
            {
            }

            public DatabaseJobState(State state)
            {
                Name = state.Name;
                Fields = state.Fields;
            }

            public JobState State { get; set; }
            public int BatchCount { get; set; }
            public JobState LastWorkingState { get; set; }
            public int DiscoverCount { get; set; }
            public DateTime? LastFullCrawlDate { get; set; }
            public DateTime? LastFetchDate { get; set; }
            public DateTime? LastDiscoverDate { get; set; }
            public bool IsActive { get; set; }
            //Number of batches that was found the last discover round
            public int LastDiscoverBatchCount { get; set; }
        }
    }
}