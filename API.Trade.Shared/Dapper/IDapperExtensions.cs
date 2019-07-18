using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace API.Trade.Shared
{
    public interface IDapperExtensions
    {

        string GetColumnNameFromAttribute(MemberInfo member);


        void AddSqlColumToModelMapping<T1>();
    }
}
