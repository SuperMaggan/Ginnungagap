using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Bifrost.Common.ApplicationServices.Sql.Common;
using Bifrost.Common.ApplicationServices.Sql.Common.Extensions;
using Bifrost.Common.ApplicationServices.Sql.Databases;
using Bifrost.Common.ApplicationServices.Sql.Models;
using Bifrost.Common.ApplicationServices.Sql.Models.Mappers;
using Bifrost.Common.ApplicationServices.Sql.Tables;
using Bifrost.Common.Core.Data;
using Bifrost.Common.Core.Domain;

namespace Bifrost.Common.ApplicationServices.Sql
{
    public class SqlScriptRepository : IScriptRepository
    {
        private readonly DatabaseBase _database;


        public SqlScriptRepository(ScriptDatabase database)
        {
            _database = database;
            _database.SetupOnce();
        }

        public IList<Script> GetScripts()
        {
            using (var conn = _database.GetOpenConnection())
            {
                var models = conn.Query<ScriptModel>(String.Format("SELECT * FROM {0}", ScriptsTable.TableName));
                return models.Select(model =>
                    model.ToDomain(
                        GetScripts(model.ImportedScriptNamesCsv.FromCsvArray<string>())))
                    .ToList();
            }
        }

        public Script GetScriptByDomainName(string domainName)
        {
            using (var connection = _database.GetOpenConnection())
            {
                var model =
                    connection.Query<ScriptModel>(
                        String.Format("SELECT TOP(1) * FROM {0} WHERE [DomainName] = @DomainName", ScriptsTable.TableName),
                        new { DomainName = domainName })
                        .FirstOrDefault();
                return model?.ToDomain(GetScripts(model.ImportedScriptNamesCsv.FromCsvArray<string>()));
            }
        }

        public Script GetScriptByName(string name)
        {
            using (var connection = _database.GetOpenConnection())
            {
                var model =
                    connection.Query<ScriptModel>(
                        String.Format("SELECT TOP(1) * FROM {0} WHERE [Name] = @Name", ScriptsTable.TableName),
                        new { Name = name })
                        .FirstOrDefault();
                return model?.ToDomain(GetScripts(model.ImportedScriptNamesCsv.FromCsvArray<string>()));
            }
        }



        public void SaveOrUpdate(Script script)
        {
            using (var connection = _database.GetOpenConnection())
            using (var transaction = connection.BeginTransaction())
            {
                var existingScript =
                    connection.Query<ScriptModel>(
                        String.Format("SELECT TOP(1) * FROM {0} WHERE [Name] = @name", ScriptsTable.TableName),
                        new { name= script.Name },
                        transaction).FirstOrDefault();
                if (existingScript == null)
                {
                   connection.Insert(script.ToModel(), transaction);
                }
                else
                {
                    connection.Update(script.ToModel(), transaction);
                }
                transaction.Commit();
            }
        }

        public void Delete(string name)
        {
            using (var connection = _database.GetOpenConnection())
            using (var transaction = connection.BeginTransaction())
            {
                connection.Execute(
                    String.Format("DELETE FROM {0} WHERE [Name] = @name", ScriptsTable.TableName),
                    new { name },
                    transaction);
                transaction.Commit();
            }
        }

        private IList<Script> GetScripts(IList<string> names)
        {
            if (!names.Any())
                return new List<Script>();
            using (var connection = _database.GetOpenConnection())
            {
                var models =
                    connection.Query<ScriptModel>(
                        String.Format("SELECT * FROM {0} WHERE [Name] IN {1}",
                            ScriptsTable.TableName,
                            names.ToSqlInConstraint()));
                return models.Select(model =>
                    model.ToDomain(
                        GetScripts(model.ImportedScriptNamesCsv.FromCsvArray<string>()
                            .Where(name => !names.Contains(name))
                            .ToArray()

                            )
                        )).ToArray();
            }
        }
    }
}