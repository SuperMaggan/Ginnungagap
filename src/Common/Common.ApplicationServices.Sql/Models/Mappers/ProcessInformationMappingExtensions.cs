using System;
using System.Linq;
using Bifrost.Common.ApplicationServices.Sql.Common.Extensions;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Common.ApplicationServices.Sql.Models.Mappers
{
    public static class ProcessInformationMappingExtensions
    {
        public static ProcessInformation ToDomain(this ProcessInformationModel model)
        {
            if (model == null)
                return null;
            return new ProcessInformation(model.Id)
            {
                ServerName = model.ServerName,
                ProcessType = model.ProcessType,
                LastUpdated = model.LastUpdated,
                Information = model.InformationCsv.FromCsvArray().Select(s => s.ToField()).ToList(),
                UpdateFrequencySec = model.UpdateFrequencySec,
                IsRunning = DateTime.UtcNow.AddSeconds(-(model.UpdateFrequencySec + 5)) < model.LastUpdated
            };
        }

        public static ProcessInformationModel ToModel(this ProcessInformation procInfo)
        {
            if (procInfo == null)
                return null;
            return new ProcessInformationModel
            {
                Id = procInfo.Id,
                ServerName = procInfo.ServerName,
                ProcessType = procInfo.ProcessType,
                LastUpdated = procInfo.LastUpdated,
                UpdateFrequencySec = procInfo.UpdateFrequencySec,
                InformationCsv = procInfo.Information.Select(f => f.ToCsvString()).ToCsvString()
            };
        }
    }
}