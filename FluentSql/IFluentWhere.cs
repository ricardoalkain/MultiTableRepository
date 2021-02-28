using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTableRepository.FluentSql
{
    public interface IFluentWhere<T>
    {
        T Where(string conditions);
    }
}
