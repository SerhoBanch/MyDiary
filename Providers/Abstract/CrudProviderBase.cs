using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using MyDiary.Models.Abstract;

namespace MyDiary.Providers.Abstract
{
    abstract class CrudProviderBase<TEntity, TPrimaryKey> where TEntity : IModel
    {
        private readonly string connectionString = $@"
            Data Source=.\SQLEXPRESS02;
            Initial Catalog=DiaryDb;
            Integrated Security=True;
            Connect Timeout=30;
            Encrypt=False;
            TrustServerCertificate=False;
            ApplicationIntent=ReadWrite;
            MultiSubnetFailover=False;";

        protected SqlConnection GetConnection()
        {
            SqlConnection connection = new(connectionString);
            connection.Open();
            return connection;
        }

        #region Abstract
        public abstract IReadOnlyCollection<TEntity> GetAll();
        public abstract TEntity Get(TPrimaryKey pk);
        public abstract void Add(TEntity entity);
        public abstract void Update(TPrimaryKey pk, TEntity entity);
        public abstract void Delete(TPrimaryKey pk);
        #endregion
    }
}