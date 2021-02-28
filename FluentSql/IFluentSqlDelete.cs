using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTableRepositoryTest.Extensions.FluentSql
{
    public interface IFluentSqlDelete : IFluentSqlWriter, IFluentWhere<IFluentSqlDelete>, IFluentSqlBase<IFluentSqlDelete>
    {
        long All();
    }
}
