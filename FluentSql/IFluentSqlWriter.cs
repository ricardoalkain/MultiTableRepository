using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTableRepository.Fluent
{
    public interface IFluentSqlWriter
    {
        //TODO: OUTPUT clause

        long Execute();
    }
}
