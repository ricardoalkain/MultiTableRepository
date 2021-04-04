using System.Collections.Generic;

namespace SimpleFluentSql
{
    public interface IFluentSqlSelect : IFluentSqlCommon<IFluentSqlSelect>, IFluentSqlWhere<IFluentSqlSelect>
    {
        IFluentSqlSelect Distinct();

        IFluentSqlSelect Columns(params string[] customColumns);

        IFluentSqlSelect WithNoLock();

        IFluentSqlSelect OrderBy(params string[] columns);

        IFluentSqlSelect Page(int pageNumber);
        IFluentSqlSelect Page(int pageNumber, int pageSize);
        IFluentSqlSelect Page(int pageNumber, int pageSize, params string[] orderByColumns);
        IFluentSqlSelect Page(int pageNumber, params string[] orderByColumns);
    }

}