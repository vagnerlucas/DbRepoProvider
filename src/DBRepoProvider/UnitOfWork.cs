using DBRepoProvider.Core;
using DBRepoProvider.Core.Observer;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DBRepoProvider
{
    public class UnitOfWork<TContext> : IObserver where TContext : class
    {
        private ContextHandler contextHandler;
        private bool autoCommit;

        public bool AutoCommit
        {
            get
            {
                return autoCommit;
            }

            private set
            {
                autoCommit = value;
            }
        }

        public TContext Context
        {
            get { return contextHandler.Context as TContext; }
        }

        public UnitOfWork(DbContext context, bool autoCommit = false)
        {
            this.autoCommit = autoCommit;
            contextHandler = ContextHandler.GetContext(context);
        }

        public GenericRepository<T> GetRepository<T>() where T : class
        {
            var result = new GenericRepository<T>(this.contextHandler.Context);
            result.Attach(this);
            return result;
        }

        public void Save()
        {
            contextHandler.Context.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return contextHandler.Context.SaveChangesAsync();
        }

        public void Update()
        {
            if (this.autoCommit)
                Save();
        }

        public object ExecuteFunction(string functionName, params object[] paramsList)
        {
            var conn = ((EntityConnection)((IObjectContextAdapter)contextHandler.Context).ObjectContext.Connection).StoreConnection;

            conn.Open();

            var sqlParamsList = new Dictionary<string, object>();
            int count = 0;
            foreach (var item in paramsList)
            {
                sqlParamsList.Add($":p{count++}", item);
            }
            var keys = string.Join(",", sqlParamsList.Select(w => w.Key));

            var functionQuery = $"SELECT {functionName} ({keys}) AS FRESULT FROM DUAL";

            using (OracleCommand cmd = conn.CreateCommand() as OracleCommand)
            {
                cmd.CommandText = functionQuery;
                foreach (var item in sqlParamsList)
                {
                    cmd.Parameters.Add(new OracleParameter(item.Key, item.Value));
                }
                var dr = cmd.ExecuteScalar();
                return dr;
            }
        }

        public ObjectResult<TResult> ExecuteProcedure<TResult>(string procedureName, params object[] paramsList) where TResult : class
        {
            int index = 0;

            var procedure = contextHandler.Context.GetType().GetRuntimeMethods().Where(m => m.Name == procedureName).FirstOrDefault();

            if (procedure == null)
                throw new ArgumentException("Procedure not found. Check the full name in your DbContext");

            var procedureParams = procedure.GetParameters();

            List<ObjectParameter> dbParamList = new List<ObjectParameter>();
            foreach (var prop in procedureParams)
            {
                ObjectParameter p;
                if (index < paramsList.Length)
                    p = new ObjectParameter(prop.Name, paramsList[index++] ?? DBNull.Value);
                else
                {

                }
                    p = new ObjectParameter(prop.Name, DBNull.Value);

                dbParamList.Add(p);
            }

            return ((IObjectContextAdapter)contextHandler.Context).ObjectContext.ExecuteFunction<TResult>(procedureName, dbParamList.ToArray());
        }
    }
}
