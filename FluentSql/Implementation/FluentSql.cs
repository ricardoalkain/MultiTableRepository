using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace MultiTableRepository.FluentSql
{
    internal class FluentSql
    {
        public const string SEPARATOR = ", ";

        public static IFluentSql<T> CreateFluentSql<T>(IDbConnection connection, ITableInfo tableInfo) where T: class
        {
            var ctor = typeof(FluentSql<T>).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[]{ typeof(IDbConnection), typeof(ITableInfo) }, null);
            return (FluentSql<T>)(ctor.Invoke(new object[] { connection, tableInfo }));
        }

        public static IFluentSqlAutoExec<T> CreateFluentSql<T>(IDbConnection connection, ITableInfo tableInfo, T entity) where T: class
        {
            var ctor = typeof(FluentSqlAutoExec<T>).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance, null,
                new Type[]{ typeof(IDbConnection), typeof(ITableInfo), typeof(T) }, null);
            return (FluentSqlAutoExec<T>)(ctor.Invoke(new object[] { connection, tableInfo, entity }));
        }
    }

    internal class FluentSql<T> : IFluentSql<T> where T : class
    {
        protected Context Context { get; set; }

        protected FluentSql(IDbConnection connection, ITableInfo tableInfo)
        {
            Context = new Context
            {
                Connection = connection,
                TableInfo = tableInfo,
                TableName = tableInfo.TableName,
                KeyName = tableInfo.KeyProperty?.Name,

                //TODO: Get params from TableInfo.Columns
            };
        }

        public IFluentSqlSelect<T> Select()
        {
            Context.Operation = SqlOperation.SELECT;
            Context.Columns = Context.TableInfo.SqlSelectColumnsText;
            return FLuentSqlSelect<T>.Chain<T>(Context);
        }

        public IFluentSqlInsert<T> Insert()
        {
            Context.Operation = SqlOperation.INSERT;
            Context.Columns = Context.TableInfo.SqlInsertColumnNames;
            Context.ValueClause = Context.TableInfo.SqlInsertColumnValues;
            return FluentSqlInsert<T>.Chain<T>(Context);
        }

        public IFluentSqlUpdate<T> Update()
        {
            Context.Operation = SqlOperation.UPDATE;
            return FluentSqlUpdate<T>.Chain<T>(Context);
        }

        public IFluentSqlDelete Delete()
        {
            Context.Operation = SqlOperation.DELETE;
            return FluentSqlDelete.Chain(Context);
        }
    }

    internal class FluentSqlAutoExec<T> : FluentSql<T>, IFluentSqlAutoExec<T> where T : class
    {
        protected T ContextEntity { get; } //?? Maybe always pass it and include a "AutoExec" flag in Context

        protected FluentSqlAutoExec(IDbConnection connection, ITableInfo tableInfo, T entity) : base(connection, tableInfo)
        {
            ContextEntity = entity;
            if (entity != null)
            {
                Context.Parameters.AddDynamicParams(entity);
            }
        }

        public IEnumerable<T> Select(params string[] conditions)
        {
            Context.Operation = SqlOperation.SELECT;
            Context.Columns = Context.TableInfo.SqlSelectColumnsText;
            Context.Where.AddRange(conditions);
            return FLuentSqlSelect<T>.Chain<T>(Context).Query();
        }

        public bool InsertIt()
        {
            Context.Entity = ContextEntity;
            Context.Operation = SqlOperation.INSERT;
            Context.Columns = Context.TableInfo.SqlInsertColumnNames;
            Context.ValueClause = Context.TableInfo.SqlInsertColumnValues;
            return FluentSqlInsert<T>.Chain<T>(Context).Execute() > 0;
        }

        public long InsertAndGetId()
        {
            Context.Entity = ContextEntity;
            Context.Operation = SqlOperation.INSERT;
            Context.Columns = Context.TableInfo.SqlInsertColumnNames;
            Context.ValueClause = Context.TableInfo.SqlInsertColumnValues;
            return FluentSqlInsert<T>.Chain<T>(Context).ExecuteAndGetId();
        }

        public bool UpdateIt()
        {
            Context.Entity = ContextEntity; //TODO: Check null all through the chain
            Context.Operation = SqlOperation.UPDATE;
            return FluentSqlUpdate<T>.Chain<T>(Context)
                .Only(GetEntityKeyValue(ContextEntity))
                .Execute() > 0; //TODO: Test behavior without Only()
        }

        public bool DeleteIt()
        {
            Context.Entity = ContextEntity;
            Context.Operation = SqlOperation.DELETE;
            return FluentSqlDelete.Chain(Context)
                .Only(GetEntityKeyValue(ContextEntity))
                .Execute() > 0;
        }

        private object GetEntityKeyValue<T>(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            return Context.TableInfo?.KeyProperty?.GetValue(entity);
        }
    }

}