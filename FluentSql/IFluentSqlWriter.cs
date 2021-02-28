using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTableRepository.FluentSql
{
    public interface IFluentSqlWriter
    {
        //TODO: OUTPUT clause

        long Execute();
    }
}
