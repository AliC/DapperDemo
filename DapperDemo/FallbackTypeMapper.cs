using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dapper;

namespace DapperDemo
{
    public class FallbackTypeMapper : SqlMapper.ITypeMap
    {
        private readonly IEnumerable<SqlMapper.ITypeMap> _mappers;

        public FallbackTypeMapper(IEnumerable<SqlMapper.ITypeMap> mappers)
        {
            _mappers = mappers;
        }

        public SqlMapper.IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
        {
            throw new NotImplementedException();
        }

        public SqlMapper.IMemberMap GetMember(string columnName)
        {
            foreach (SqlMapper.ITypeMap mapper in _mappers)
            {
                try
                {
                    SqlMapper.IMemberMap result = mapper.GetMember(columnName);
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch (NotImplementedException)
                {
                    // the CustomPropertyTypeMap only supports a no-args
                    // constructor and throws a not implemented exception.
                    // to work around that, catch and ignore.
                }
            }
            return null;
        }
        // implement other interface methods similarly

        // required sometime after version 1.13 of dapper
        public ConstructorInfo FindConstructor(string[] names, Type[] types)
        {
            foreach (SqlMapper.ITypeMap mapper in _mappers)
            {
                try
                {
                    ConstructorInfo result = mapper.FindConstructor(names, types);

                    if (result != null)
                    {
                        return result;
                    }
                }
                catch (NotImplementedException)
                {
                    // the CustomPropertyTypeMap only supports a no-args
                    // constructor and throws a not implemented exception.
                    // to work around that, catch and ignore.
                }
            }
            return null;
        }

        public ConstructorInfo FindExplicitConstructor()
        {
            return _mappers
                .Select(mapper => mapper.FindExplicitConstructor())
                .FirstOrDefault(result => result != null);
        }
    }
}