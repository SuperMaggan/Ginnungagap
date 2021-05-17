using System.Data.Common;
using Dapper;
using Bifrost.Common.ApplicationServices.Sql.Common;

namespace Bifrost.Data.Sql.Databases.Bifrost.Objects.StoredProcedures
{
    public class PushVerifyDocumentToQueueProcedure : StoredProcedureBase, IDatabaseObject
    {
        protected override string GetCreateSql()
        {
            return @"
                    -- =============================================
                    -- Author:		<Magnus Månsson>
                    -- Create date: <2015-10-27>
                    -- Updated : <2015-11-03> 
                    -- Description:	<Creates a QueueItem for each DocumentState whose VerifyDate is less than UTCNOW>
                    -- Returns: Number of documents pushed
                    -- =============================================
                                                                        " +
                     $"CREATE PROCEDURE [dbo].[{GetName()}]"+
                    @"          
                        @category nvarchar(256)
                    AS
	                BEGIN
		                DECLARE @nowDate datetime;
		                SET @nowDate = GETUTCDATE();

		                INSERT INTO QueueItems
		                SELECT Category, @nowDate as 'CreateDate', DocumentId, OptionalData FROM DocumentStates 
		                WHERE Category = @category  AND VerifyDate < @nowDate

		                IF(@@ROWCOUNT > 0)
		                BEGIN						
			                UPDATE DocumentStates   
			                SET VerifyDate = (VerifyDate - LastVerified) + VerifyDate , LastVerified = @nowDate
			                WHERE Category=@category AND VerifyDate < @nowDate
		                END
	                END";
        }

        protected override string GetName()
        {
            return GetProcedureName;
        }

        public static string GetProcedureName => "PushVerifyDocumentsToQueue";

        public static int Execute(DbConnection connection, string category)
        {
            return connection.Execute($"EXEC {GetProcedureName} @category = '{category}'");
        }

        public void SetConstraints(DbConnection connection)
        {
            return;

        }
    }
}
