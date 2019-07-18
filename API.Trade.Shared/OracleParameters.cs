using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;

namespace API.Trade.Shared
{
    public class OracleDynamicParameters : SqlMapper.IDynamicParameters
    {
        static Dictionary<SqlMapper.Identity, Action<IDbCommand, object>> paramReaderCache = new Dictionary<SqlMapper.Identity, Action<IDbCommand, object>>();

        readonly Dictionary<string, ParamInfo> parameters = new Dictionary<string, ParamInfo>();

        List<object> templates;

        /// <summary>Construct a dynamic parameter bag.</summary>
        public OracleDynamicParameters() { }

        /// <summary>Construct a dynamic parameter bag starting with a template (e.g. <c>new { A, B }</c>).</summary>
        /// <param name="template">Can be an anonymous type or a <see cref="DynamicParameters"/> bag.</param>
        public OracleDynamicParameters(object template)
        {
            AddDynamicParams(template);
        }

        /// <summary>
        ///     Append a whole object full of parameters to the dynamic parameter bag. For example:
        ///     <para>
        ///         <c>AddDynamicParams(new {A = 1, B = 2}); // will add property A and B to the dynamic parameters bag.</c>
        ///     </para>
        /// </summary>
        /// <param name="param"></param>
        public void AddDynamicParams(dynamic param)
        {
            var obj = param as object;
            if (obj != null)
            {
                var subDynamic = obj as OracleDynamicParameters;
                if (subDynamic == null)
                {
                    var dictionary = obj as IEnumerable<KeyValuePair<string, object>>;
                    if (dictionary == null)
                    {
                        templates = templates ?? new List<object>();
                        templates.Add(obj);
                    }
                    else
                    {
                        foreach (var kvp in dictionary)
                        {
                            Add(kvp.Key, kvp.Value);
                        }
                    }
                }
                else
                {
                    if (subDynamic.parameters != null)
                    {
                        foreach (var kvp in subDynamic.parameters)
                        {
                            parameters.Add(kvp.Key, kvp.Value);
                        }
                    }

                    if (subDynamic.templates != null)
                    {
                        templates = templates ?? new List<object>();
                        foreach (var t in subDynamic.templates)
                        {
                            templates.Add(t);
                        }
                    }
                }
            }
        }

        /// <summary>Adds a parameter to this dynamic parameter bag.</summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="dbType">The <see cref="DbType"/> of the parameter.</param>
        /// <param name="direction">The <see cref="ParameterDirection"/>.</param>
        /// <param name="size">The size of the parameter's value.</param>
        public void Add(string name, object value = null, OracleDbType? dbType = null, ParameterDirection? direction = null, int? size = null)
        {
            parameters[CleanParameterName(name)] = new ParamInfo { Name = name, Value = value, ParameterDirection = direction ?? ParameterDirection.Input, DbType = dbType, Size = size };
        }


        /// <summary>Adds a parameter to this dynamic parameter bag.</summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="dbType">The <see cref="DbType"/> of the parameter.</param>
        /// <param name="direction">The <see cref="ParameterDirection"/>.</param>
        /// <param name="size">The size of the parameter's value.</param>
        public void Add(string name, OracleDbType? dbType = null, ParameterDirection? direction = null, object value = null, int? size = null)
        {
            parameters[CleanParameterName(name)] = new ParamInfo { Name = name, Value = value, ParameterDirection = direction ?? ParameterDirection.Input, DbType = dbType, Size = size };
        }

        static string CleanParameterName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                switch (name[0])
                {
                    case '@':
                    case ':':
                    case '?':
                        return name.Substring(1);
                }
            }
            return name;
        }

        /// <summary>Add all the parameters needed to the command just before it executes.</summary>
        /// <param name="command">The raw command prior to execution.</param>
        /// <param name="identity">Information about the query.</param>
        void SqlMapper.IDynamicParameters.AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            AddParameters(command, identity);
        }

        /// <summary>Add all the parameters needed to the command just before it executes.</summary>
        /// <param name="command">The raw command prior to execution.</param>
        /// <param name="identity">Information about the query.</param>
        protected void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            if (templates != null)
            {
                foreach (var template in templates)
                {
                    var newIdent = identity.ForDynamicParameters(template.GetType());
                    Action<IDbCommand, object> appender;

                    lock (paramReaderCache)
                    {
                        if (!paramReaderCache.TryGetValue(newIdent, out appender))
                        {
                            appender = SqlMapper.CreateParamInfoGenerator(newIdent, false, false);
                            paramReaderCache[newIdent] = appender;
                        }
                    }

                    appender(command, template);
                }
            }

            foreach (var param in parameters.Values)
            {
                string name = CleanParameterName(param.Name);
                bool parameterExists = ((OracleCommand)command).Parameters.Contains(name);
                OracleParameter oracleParameter;
                if (parameterExists)
                {
                    oracleParameter = ((OracleCommand)command).Parameters[name];
                }
                else
                {
                    oracleParameter = ((OracleCommand)command).CreateParameter();
                    oracleParameter.ParameterName = name;
                }

                var val = param.Value;
                oracleParameter.Value = val ?? DBNull.Value;
                oracleParameter.Direction = param.ParameterDirection;

                var s = val as string;
                if (s != null && s.Length <= 4000)
                    oracleParameter.Size = 4000;

                if (param.Size != null)
                    oracleParameter.Size = param.Size.Value;

                if (param.DbType != null)
                    oracleParameter.OracleDbType = param.DbType.Value;

                if (!parameterExists)
                    command.Parameters.Add(oracleParameter);

                param.AttachedParam = oracleParameter;
            }
        }

        /// <summary>All the names of the parameters in the bag; use <see cref="Get{T}(string)"/> to yank them out.</summary>
        public IEnumerable<string> ParameterNames => parameters.Select(p => p.Key);

        /// <summary>Gets the value of a parameter by name.</summary>
        /// <typeparam name="T">The <see cref="Type"/> of the parameter's value.</typeparam>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The parameter's value or <c>null</c> (<see cref="DBNull.Value"/> is never returned).</returns>
        public T Get<T>(string name)
        {
            var paramValue = parameters[CleanParameterName(name)].AttachedParam.Value;
            if (paramValue == DBNull.Value)
            {
#pragma warning disable RECS0017 // Possible compare of value type with 'null'
                if (default(T) != null)
#pragma warning restore RECS0017 // Possible compare of value type with 'null'
                    throw new ApplicationException("Attempting to cast a DBNull to a non nullable type!");

                return default(T);
            }

            var typeOfT = typeof(T);
            if (typeOfT == typeof(int) && paramValue is global::Oracle.ManagedDataAccess.Types.OracleDecimal)
            {
                var oracleDecimal = (global::Oracle.ManagedDataAccess.Types.OracleDecimal)paramValue;
                paramValue = oracleDecimal.ToInt32();
            }
            else if (typeOfT == typeof(string) && paramValue is global::Oracle.ManagedDataAccess.Types.OracleString)
            {
                var oracleString = (global::Oracle.ManagedDataAccess.Types.OracleString)paramValue;
                paramValue = oracleString.IsNull
                            ? null
                            : oracleString.ToString();
            }

            return (T)paramValue;
        }

        /// <summary>Object containing members representing a parameter.</summary>
        class ParamInfo
        {
            /// <summary>Gets or sets the name of the  parameter.</summary>
            public string Name { get; set; }

            /// <summary>Gets or sets the parameter's value.</summary>
            public object Value { get; set; }

            /// <summary>Gets or sets the <see cref="System.Data.ParameterDirection"/> of the parameter.</summary>
            public ParameterDirection ParameterDirection { get; set; }

            /// <summary>Gets or sets the <see cref="OracleDbType"/> of the parameter.</summary>
            public OracleDbType? DbType { get; set; }

            /// <summary>Gets or sets the size of the parameter's value.</summary>
            public int? Size { get; set; }

            /// <summary>Gets or sets the <see cref="IDbDataParameter"/> associated with the parameter.</summary>
            public IDbDataParameter AttachedParam { get; set; }
        }
    }
}
