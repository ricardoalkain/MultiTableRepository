using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTableRepository.Fluent
{
    public interface IFluentSqlDelete : IFluentSqlWriter, IFluentWhere<IFluentSqlDelete>, IFluentSqlBase<IFluentSqlDelete>
    {
        long All();
    }
}
