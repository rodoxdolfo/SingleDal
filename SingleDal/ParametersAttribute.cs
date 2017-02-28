using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.SingleDal
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ParameterAttribute : Attribute
    {
        public ParameterAttribute(string parameterName, ProcedureType type, int order)
        {
            ParameterName = parameterName;
            Order = order;
            IsID = false;
            Type = type;
        }
        public ParameterAttribute(string parameterName, ProcedureType type, int order, bool isid)
        {
            ParameterName = parameterName;
            Order = order;
            IsID = isid;
            Type = type;
        }

        public string ParameterName { get; set; }
        public int Order { get; set; }
        public bool IsID { get; set; }
        public ProcedureType Type { get; set; }
    }
}
