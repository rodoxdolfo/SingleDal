using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.SingleDal
{
    /// <summary>
    /// Specifies the name of the field in the table that the property maps to 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DBFieldAttribute : Attribute
    {
        private string _fieldName;
        private bool _emptyIfNull;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="fieldName">name of the field that the property will
        /// be mapped to</param>
        public DBFieldAttribute(string fieldName)
        {
            _fieldName = fieldName;
            _emptyIfNull = false;
        }

        public DBFieldAttribute(string fieldName, bool emptyIfNull)
        {
            _fieldName = fieldName;
            _emptyIfNull = emptyIfNull;
        }

        public string FieldName
        {
            get { return _fieldName; }
        }

        public bool EmptyIfNull
        {
            get { return _emptyIfNull; }
        }
    }
}
