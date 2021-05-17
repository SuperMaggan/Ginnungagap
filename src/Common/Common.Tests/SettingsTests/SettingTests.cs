using System;
using System.Collections.Generic;
using System.Linq;
using Bifrost.Common.Core.Settings;
using FluentAssertions;
using Xunit;

namespace Bifrost.Common.Tests.SettingsTests
{
    public class SettingTests
    {
        [Fact]
        public void Can_find_app_config()
        {
            var config = TemporaryConnectionManager.GetApplicationConfig();
            config.Should().NotBeNull();
        }

        [Fact]
        public void Can_get_app_setting()
        {
            var testSetting = TemporaryConnectionManager.GetAppSetting("test");
            testSetting.Should().Be("testvalue");
        }

        [Fact]
        public void Can_get_connection_setting()
        {
            var testSetting = TemporaryConnectionManager.GetConnectionString("TestConnection");
            testSetting.ConnectionString.Should().Be("http://blog.cwa.me.uk");
        }

        [Fact]
        public void Can_resolve_automatically()
        {
            var testSetting = new UnitTestSettingsObj();
            testSetting.Test.Should().Be("testvalue");
            testSetting.TestConnection?.ConnectionString.Should().Be("http://blog.cwa.me.uk");

        }

        [Fact]
        public void CanPopulateSettingsFromAppConfig()
        {
            var importantSettings = new ImportantSettings();
            importantSettings.ImportantString.Should().Be("Kite4life");
            importantSettings.ImportantDate.Should().Be(new DateTime(1984, 10, 6));
            importantSettings.ImportantInt.Should().Be(10);
            importantSettings.ImportantFloat.Should().Be(0.1f);
            importantSettings.ImportantEmptyFloat.Should().Be(0);
            importantSettings.TestConnection.ConnectionString.Should()
                .Be("http://blog.cwa.me.uk");
            importantSettings.TestConnection.ProviderName.Should().Be("System.Data.Sql");
        }

        [Fact]
        public void CanGetSetSettingsMessages()
        {
            var settings = new ImportantSettings();
            var messages = settings.GetSetSettingsMessages();
            messages.Count.Should().Be(7);
        }

        [Fact]
        public void CanGetNotSetSettingsMessages()
        {
            var settings = new ImportantSettings();
            var messages = settings.GetNotSetSettingsMessages();
            messages.Count.Should().Be(2);
        }


        [Fact]
        public void Should_not_find_connectionsettings()
        {
            var settings = new ImportantSettings();
            settings.ShouldNotExistConnection.Should().BeNull();
        }

        [Fact]
        public void Can_set_from_environmental_variable()
        {
            Environment.SetEnvironmentVariable("SetFromEnvironmental", "44");
            var settings = new ImportantSettings();
            settings.SetFromEnvironmental.Should().Be(44);
        }


        [Fact]
        public void Can_set_connection_from_environmental_variable()
        {
            Environment.SetEnvironmentVariable("EnvironmentalConnection", "envyMe");
            var settings = new ImportantSettings();
            settings.EnvironmentalConnection.ConnectionString.Should().Be("envyMe");
        }
    }

    internal class UnitTestSettingsObj : ConfigSettingsBase
    {
        
        public string Test { get; set; }
        public ConnectionStringSettings TestConnection { get; set; }
    }

    internal class ImportantSettings : ConfigSettingsBase
    {
        public string ImportantString { get; set; }
        public DateTime? ImportantDate { get; set; }
        public int ImportantInt { get; set; }
        public float ImportantFloat { get; set; }
        public float ImportantEmptyFloat { get; set; }
        public ConnectionStringSettings TestConnection { get; set; }
        public ConnectionStringSettings ShouldNotExistConnection { get; set; }

        public ConnectionStringSettings EnvironmentalConnection { get; set; }

        public int SetFromEnvironmental { get; set; }
        

        public new IList<string> GetSetSettingsMessages()
        {
            return base.GetSetSettingsMessages().ToList();
        }

        public new IList<string> GetNotSetSettingsMessages()
        {
            return base.GetNotSetSettingsMessages().ToList();
        }
    }
}
