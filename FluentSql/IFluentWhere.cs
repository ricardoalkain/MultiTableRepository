using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTableRepositoryTest.Extensions.FluentSql
{
    public interface IFluentWhere<T>
    {
        T Where(string conditions);
    }
}
