using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleFluentSql
{
    public interface IFluentSqlDelete : IFluentSqlWhere<IFluentSqlDelete>, IFluentSqlCommon<IFluentSqlDelete>
    {
        IFluentSqlDelete All();
    }
}
