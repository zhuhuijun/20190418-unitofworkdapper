using Ceisen.Core;
using Ceisen.Core.Models;
using Ceisen.IRepository;
using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Transactions;

namespace Ceisen.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public IDbConnection _dbConnection;
        protected DbOpion _dbOpion;
        private Dictionary<object, Action> addEntities;
        private Dictionary<object, Action> updateEntities;
        private Dictionary<object, Action> deleteEntities;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public UnitOfWork(IOptionsSnapshot<DbOpion> options)
        {
            _dbOpion = options.Get("CzarCms");
            if (_dbOpion == null)
            {
                throw new ArgumentNullException(nameof(DbOpion));
            }
            if(_dbOpion.DbType== DatabaseType.MySQL.ToString())
            {
                Dapper.SimpleCRUD.SetDialect(Dapper.SimpleCRUD.Dialect.MySQL);
            }
            addEntities = new Dictionary<object, Action>();
            updateEntities = new Dictionary<object, Action>();
            deleteEntities = new Dictionary<object, Action>();
        }
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            this.addEntities.Add(entity, () =>
            {
                _dbConnection.Insert<TEntity>(entity);
            });
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            this.updateEntities.Add(entity, () =>
            {
                _dbConnection.Update(entity);
            });
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            this.deleteEntities.Add(entity, () =>
            {
                _dbConnection.Delete(entity);
            });
        }


        public int Commit()
        {
            int count = 0;
            using (TransactionScope scope = CreateTransactionScope())
            {
                try
                {
                    using (_dbConnection = ConnectionFactory.CreateConnection(_dbOpion.DbType, _dbOpion.ConnectionString))
                    {
                        foreach (var entity in deleteEntities.Keys)
                        {
                            this.deleteEntities[entity]();
                        }

                        foreach (var entity in updateEntities.Keys)
                        {
                            this.updateEntities[entity]();
                        }

                        foreach (var entity in addEntities.Keys)
                        {
                            this.addEntities[entity]();
                        }
                    }

                    scope.Complete();
                    count = deleteEntities.Count + updateEntities.Count + addEntities.Count;
                    deleteEntities.Clear();
                    updateEntities.Clear();
                    addEntities.Clear();
                    if (_dbConnection != null)
                    {
                        if (_dbConnection.State == ConnectionState.Open)
                        {
                            _dbConnection.Close();
                        }

                        // Conn.Dispose();
                    }
                }
                catch (Exception ex)
                {

                    count = 0;
                    throw ex;
                }
            }
            return count;
        }

        public static TransactionScope CreateTransactionScope()
        {
            var transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            transactionOptions.Timeout = TransactionManager.MaximumTimeout;
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);

        }
    }
}
