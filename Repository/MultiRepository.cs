using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using SimpleFluentSql;
using MultiTableRepository.Parser;

namespace MultiTableRepository.Repositories
{
    public /*abstract*/ class MultiRepository<T> : IDisposable /*, IReadOperationsRepository<T>, IWriteOperationsRepository<T>*/ where T : class /*, ICreated*/
    {
        private DbConnection _connection;

        protected IDbConnection Connection
        {
            get
            {
                if (_connection != null && _connection.State == ConnectionState.Open)
                {
                    return _connection;
                }

                _connection = SqlClientFactory.Instance.CreateConnection();
                _connection.ConnectionString = ConnectionString;
                _connection.Open();
                if (_connection.State != ConnectionState.Open)
                {
                    throw new InvalidOperationException("Connection should be open!");
                }

                return _connection;
            }
        }

        protected readonly string ConnectionString;

        protected readonly Type EntityType;



        /*protected*/ public MultiRepository(string connectionStrings)
        {
            ConnectionString = connectionStrings;
            EntityType = typeof(T);
        }





        ////////////////protected IFluentSql<T> For(params string[] segments)
        ////////////////{
        ////////////////    //var info = MultiTableParser.GetTableInfo<T>(segments);
        ////////////////    //return FluentSql.CreateFluentSql<T>(Connection, info);
        ////////////////}

        ////////////////protected IFluentSqlAutoExec<T> For(T entity)
        ////////////////{
        ////////////////    //var info = MultiTableParser.GetTableInfo(entity);
        ////////////////    //return FluentSql.CreateFluentSql<T>(Connection, info, entity);
        ////////////////}

        ////////////////protected IFluentSqlAutoExec<T> ForMany(IEnumerable<T> entities)
        ////////////////{
        ////////////////}








        /// <summary>
        /// Insert the Entity into the database
        /// </summary>
        /// <param name="entity">Entity to be saved</param>
        /// <returns>Return the ID of the Entity.</returns>
        public long Add(T entity)
        {
            return 0; // For(entity).InsertAndGetId(); /////// Carry entity into context
        }

        /// <summary>
        /// Insert the Entity into the database
        /// <remarks>If necessary you might override this method.
        /// Specially if you expect more than 3.000 objects</remarks>
        /// </summary>
        /// <param name="entities">Entity to be saved</param>
        public virtual void Add(IEnumerable<T> entities)
        {
            //TODO: ForMany(entities).InsertAll();


            //foreach (var entity in entities)
            //{
            //    For(entity).InsertIt();
            //}
        }

        /// <summary>
        /// Delete the Entity from the database.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        /// <returns>return true is successful </returns>
        public bool Remove(T entity)
        {
            return false; // For(entity).DeleteIt();
        }

        /// <summary>
        /// Get a Entity using its Id
        /// </summary>
        /// <param name="id">Key of the Entity</param>
        /// <returns>returns <see cref="T"/> or wise null</returns>
        public T Get(int id, params string[] segments)
        {
            return null; // For(segments).Select().Only(id).QueryOne();
        }

        /// <summary>
        /// Get All the Entities from a table
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll(params string[] segments)
        {
            return null; // For(segments).Select().Query();
        }

        /// <summary>
        /// Update the Entity from the database.
        /// </summary>
        /// <param name="entity">Entity to be updated</param>
        /// <returns>return true is successful </returns>
        public bool Update(T entity)
        {
            return false; // For(entity).UpdateIt();
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }

        #region Private Methods

        private IReadOnlyDictionary<string, IEnumerable<T>> SplitBySegments(params string[] segments)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}