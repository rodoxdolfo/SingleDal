using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.SingleDal
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ProcedureAttribute : Attribute
    {

        public ProcedureAttribute(string procedureName, ProcedureType type)
        {
            ProcedureName = procedureName;
            Type = type;
        }

        public string ProcedureName { get; set; }
        public ProcedureType Type { get; set; }
    }

    public enum ProcedureType
    {
        Select = 1,
        SelectFull = 2,
        SelectID = 4,
        Insert = 8,
        Update = 16,
        Delete = 32
    }
}
