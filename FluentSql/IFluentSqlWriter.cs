using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTableRepositoryTest.Extensions.FluentSql
{
    public interface IFluentSqlWriter
    {
        //TODO: OUTPUT clause

        long Execute();
    }
}
