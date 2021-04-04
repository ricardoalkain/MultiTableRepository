using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleFluentSql.Engine
{
    enum SqlOperation
    {
        NONE,
        SELECT,
        INSERT,
        UPDATE,
        DELETE,
        //TOD: MERGE, PROCEDURE, FUNCTION, DDL
    }
}
