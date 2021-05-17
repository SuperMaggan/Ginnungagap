using System;
using Bifrost.Common.ApplicationServices.Sql.Common.Extensions;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Common.ApplicationServices.Sql.Models.Mappers
{
    public static class WorkTaskMappingExtensions
    {
        public static WorkTask ToDomain(this WorkTaskModel model)
        {
            if (model == null)
                return null;
            return new WorkTask(model.Owner, model.Id.ToString(), model.InstructionsCsv.FromCsvArray());
        }

        public static WorkTaskModel ToModel(this WorkTask workTask, bool isCheckedOut = false)
        {
            if (workTask == null)
                return null;
            int intId;
            if (!int.TryParse(workTask.Id, out intId))
                throw new FormatException("An worktask Id needs to be an Int");

            return new WorkTaskModel
            {
                Owner = workTask.GroupName,
                Id = intId,
                CheckedOut = isCheckedOut,
                InstructionsCsv = workTask.Instructions.ToCsvString()
            };
        }
    }
}