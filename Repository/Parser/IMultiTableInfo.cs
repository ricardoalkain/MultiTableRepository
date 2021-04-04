using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleFluentSql;

namespace MultiTableRepository.Parser
{
    public interface IMultiTableInfo : ISqlTableInfo
    {
        Type EntityType { get; }

        string TablePrefix { get; }
        string TableSuffix { get; }
        IEnumerable<string> SegmentColumns { get; }

        PropertyInfo KeyProperty { get; }
        IEnumerable<PropertyInfo> WritableProperties { get; }
        IEnumerable<PropertyInfo> AllProperties { get; }
        IEnumerable<PropertyInfo> SegmentProperties { get; }
    }
}
