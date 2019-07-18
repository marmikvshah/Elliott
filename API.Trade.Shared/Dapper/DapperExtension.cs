using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;
using System.Linq;

namespace API.Trade.Shared
{
    public class DapperExtensions : IDapperExtensions
    {

        public string GetColumnNameFromAttribute(MemberInfo member)
        {
            if (member == null) return null;

            var attrib = (ColumnAttribute)Attribute.GetCustomAttribute(member, typeof(ColumnAttribute), false);
            return attrib?.Name;
        }

        
        public void AddSqlColumToModelMapping<T1>()
        {
            var userModelMap = new CustomPropertyTypeMap(typeof(T1),
                                                (type, columnName) =>
                                                type.GetProperties().FirstOrDefault(prop => GetColumnNameFromAttribute(prop) == columnName));
            SqlMapper.SetTypeMap(typeof(T1), userModelMap);
        }
    }
}
