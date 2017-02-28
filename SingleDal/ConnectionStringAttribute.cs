using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.SingleDal
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConnectionStringAttribute: Attribute
    {

        public ConnectionStringAttribute(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        public string ConnectionStringName { get; set; }
    }
}
