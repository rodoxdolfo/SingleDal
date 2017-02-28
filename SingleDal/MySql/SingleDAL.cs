using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Framework.SingleDal
{
    public static class SingleDAL<T>
    {
        public static BaseCollection<T> Buscar()
        {
            string currentfield = "";
            ProcedureAttribute select = null;
            var procedures = Attribute.GetCustomAttributes(typeof(T), typeof(ProcedureAttribute));
            foreach (Attribute procedure in procedures)
            {
                if ((procedure as ProcedureAttribute).Type == ProcedureType.SelectFull)
                    select = (procedure as ProcedureAttribute);
            }
            if (select == null)
                throw new Exception("Procedure de Select não configurada");
            BaseCollection<T> retorno = new BaseCollection<T>();


            var conn = Attribute.GetCustomAttributes(typeof(T), typeof(ConnectionStringAttribute))[0];
            if (conn == null)
                throw new Exception("Connection String não configurada");

            using (MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings[(conn as ConnectionStringAttribute).ConnectionStringName].ConnectionString))
            {
                try
                {
                    cn.Open();

                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = cn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = (select as ProcedureAttribute).ProcedureName;

                        MySqlDataReader reader = command.ExecuteReader();

                        PropertyInfo[] properties = typeof(T).GetProperties();
                        while (reader.Read())
                        {
                            var obj = Activator.CreateInstance<T>();
                            foreach (PropertyInfo property in properties)
                            {
                                var DBField = Attribute.GetCustomAttributes(property, typeof(DBFieldAttribute));
                                if (DBField.Length > 0) {
                                    currentfield = (DBField[0] as DBFieldAttribute).FieldName;
                                    if(reader[(DBField[0] as DBFieldAttribute).FieldName] != DBNull.Value)
                                        // work arround para tratamento de campo tipo data se null
                                        if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                                            property.SetValue(obj, Convert.ToDateTime(reader[(DBField[0] as DBFieldAttribute).FieldName]), null);
                                        else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                                            property.SetValue(obj, Convert.ToBoolean(reader[(DBField[0] as DBFieldAttribute).FieldName]), null);
                                        else
                                            property.SetValue(obj, reader[(DBField[0] as DBFieldAttribute).FieldName], null);
                                    else
                                        property.SetValue(obj, null, null);

                                }
                                else
                                    property.SetValue(obj, null, null);
                            }
                            retorno.Add(obj);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    cn.Close();
                    cn.Dispose();
                }
            }

            return retorno;
        }
        public static BaseCollection<T> Buscar(T parameters)
        {
            ProcedureAttribute select = null;
            string currentfield = "";
            var procedures = Attribute.GetCustomAttributes(typeof(T), typeof(ProcedureAttribute));
            foreach (Attribute procedure in procedures)
            {
                if ((procedure as ProcedureAttribute).Type == ProcedureType.Select)
                    select = (procedure as ProcedureAttribute);
            }
            if (select == null)
                throw new Exception("Procedure de Select não configurada");
            BaseCollection<T> retorno = new BaseCollection<T>();

            var conn = Attribute.GetCustomAttributes(typeof(T), typeof(ConnectionStringAttribute))[0];
            if (conn == null)
                throw new Exception("Connection String não configurada");

            using (MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings[(conn as ConnectionStringAttribute).ConnectionStringName].ConnectionString))
            {
                try
                {
                    cn.Open();

                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = cn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = (select as ProcedureAttribute).ProcedureName;

                        PropertyInfo[] properties = typeof(T).GetProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            var parameter = Attribute.GetCustomAttributes(property, typeof(ParameterAttribute));
                            if (parameter.Length > 0)
                            {
                                if ((parameter[0] as ParameterAttribute).Type.HasFlag(ProcedureType.Select))
                                {
                                    command.Parameters.Insert(command.Parameters.Count < (parameter[0] as ParameterAttribute).Order ? command.Parameters.Count : (parameter[0] as ParameterAttribute).Order, new MySqlParameter((parameter[0] as ParameterAttribute).ParameterName, GetDbType(property.PropertyType)));
                                    command.Parameters[(parameter[0] as ParameterAttribute).ParameterName].Value = property.GetValue(parameters, null);
                                }
                            }
                        }
                        MySqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            var obj = Activator.CreateInstance<T>();
                            foreach (PropertyInfo property in properties)
                            {
                                var DBField = Attribute.GetCustomAttributes(property, typeof(DBFieldAttribute));
                                if (DBField.Length > 0)
                                {
                                    currentfield = (DBField[0] as DBFieldAttribute).FieldName;
                                    if (reader[(DBField[0] as DBFieldAttribute).FieldName] != DBNull.Value)
                                        if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                                            property.SetValue(obj, Convert.ToDateTime(reader[(DBField[0] as DBFieldAttribute).FieldName]), null);
                                        else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                                            property.SetValue(obj, Convert.ToBoolean(reader[(DBField[0] as DBFieldAttribute).FieldName]), null);
                                        else
                                            property.SetValue(obj, reader[(DBField[0] as DBFieldAttribute).FieldName], null);
                                    else
                                        property.SetValue(obj, null, null);
                                }
                                else
                                    property.SetValue(obj, null, null);
                            }
                            retorno.Add(obj);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    cn.Close();
                    cn.Dispose();
                }
            }

            return retorno;
        }

        public static BaseCollection<T> BuscarInverso(T parameters)
        {
            ProcedureAttribute select = null;
            string currentfield = "";
            var procedures = Attribute.GetCustomAttributes(typeof(T), typeof(ProcedureAttribute));
            foreach (Attribute procedure in procedures)
            {
                if ((procedure as ProcedureAttribute).Type == ProcedureType.Select)
                    select = (procedure as ProcedureAttribute);
            }
            if (select == null)
                throw new Exception("Procedure de Select não configurada");
            BaseCollection<T> retorno = new BaseCollection<T>();

            var conn = Attribute.GetCustomAttributes(typeof(T), typeof(ConnectionStringAttribute))[0];
            if (conn == null)
                throw new Exception("Connection String não configurada");

            using (MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings[(conn as ConnectionStringAttribute).ConnectionStringName].ConnectionString))
            {
                try
                {
                    cn.Open();

                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = cn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = (select as ProcedureAttribute).ProcedureName;

                        PropertyInfo[] properties = typeof(T).GetProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            var parameter = Attribute.GetCustomAttributes(property, typeof(ParameterAttribute));
                            if (parameter.Length > 0)
                            {
                                if ((parameter[0] as ParameterAttribute).Type.HasFlag(ProcedureType.Select))
                                {
                                    command.Parameters.Insert(command.Parameters.Count < (parameter[0] as ParameterAttribute).Order ? command.Parameters.Count : (parameter[0] as ParameterAttribute).Order, new MySqlParameter((parameter[0] as ParameterAttribute).ParameterName, GetDbType(property.PropertyType)));
                                    command.Parameters[(parameter[0] as ParameterAttribute).ParameterName].Value = property.GetValue(parameters, null);
                                }
                            }
                        }
                        MySqlDataReader reader = command.ExecuteReader();
                        DataTable table = reader.GetSchemaTable();
                        int total = table.Rows.Count;

                        while (reader.Read())
                        {
                            var obj = Activator.CreateInstance<T>();

                            for (int i = 0; i < total; i++)
                            {
                                PropertyInfo property = getProperty(properties, table.Rows[i]["ColumnName"].ToString());
                                if (property != null)
                                {
                                    var DBField = Attribute.GetCustomAttributes(property, typeof(DBFieldAttribute));
                                    currentfield = (DBField[0] as DBFieldAttribute).FieldName;

                                    if (reader[currentfield] != DBNull.Value)
                                        if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                                            property.SetValue(obj, Convert.ToDateTime(reader[currentfield]), null);
                                        else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                                            property.SetValue(obj, Convert.ToBoolean(reader[currentfield]), null);
                                        else
                                            property.SetValue(obj, reader[currentfield], null);
                                    else
                                        property.SetValue(obj, null, null);
                                }
                            }
                            retorno.Add(obj);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    cn.Close();
                    cn.Dispose();
                }
            }

            return retorno;
        }

        static PropertyInfo getProperty(PropertyInfo[] obj, string propertyName)
        {
           return obj.SingleOrDefault(
                    p => p.Name.Equals(propertyName)
                );
        }

        public static BaseCollection<T> BuscarID(T parameters)
        {
            ProcedureAttribute select = null;
            string currentfield = "";
            var procedures = Attribute.GetCustomAttributes(typeof(T), typeof(ProcedureAttribute));
            foreach (Attribute procedure in procedures)
            {
                if ((procedure as ProcedureAttribute).Type == ProcedureType.SelectID)
                    select = (procedure as ProcedureAttribute);
            }
            if (select == null)
                throw new Exception("Procedure de Select não configurada");
            BaseCollection<T> retorno = new BaseCollection<T>();


            var conn = Attribute.GetCustomAttributes(typeof(T), typeof(ConnectionStringAttribute))[0];
            if (conn == null)
                throw new Exception("Connection String não configurada");

            using (MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings[(conn as ConnectionStringAttribute).ConnectionStringName].ConnectionString))
            {
                try
                {
                    cn.Open();

                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = cn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = (select as ProcedureAttribute).ProcedureName;

                        PropertyInfo[] properties = typeof(T).GetProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            var parameter = Attribute.GetCustomAttributes(property, typeof(ParameterAttribute));
                            if (parameter.Length > 0)
                            {
                                if ((parameter[0] as ParameterAttribute).Type.HasFlag(ProcedureType.SelectID))
                                {
                                    command.Parameters.Insert(command.Parameters.Count < (parameter[0] as ParameterAttribute).Order ? command.Parameters.Count : (parameter[0] as ParameterAttribute).Order, new MySqlParameter((parameter[0] as ParameterAttribute).ParameterName, GetDbType(property.PropertyType)));
                                    command.Parameters[(parameter[0] as ParameterAttribute).ParameterName].Value = property.GetValue(parameters, null);
                                }
                            }
                        }

                        MySqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            var obj = Activator.CreateInstance<T>();
                            foreach (PropertyInfo property in properties)
                            {

                                var DBField = Attribute.GetCustomAttributes(property, typeof(DBFieldAttribute));
                                if (DBField.Length > 0)
                                {
                                    currentfield = (DBField[0] as DBFieldAttribute).FieldName;
                                    if (reader[(DBField[0] as DBFieldAttribute).FieldName] != DBNull.Value)
                                        if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                                            property.SetValue(obj, Convert.ToDateTime(reader[(DBField[0] as DBFieldAttribute).FieldName]), null);
                                        else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                                            property.SetValue(obj, Convert.ToBoolean(reader[(DBField[0] as DBFieldAttribute).FieldName]), null);
                                        else
                                            property.SetValue(obj, reader[(DBField[0] as DBFieldAttribute).FieldName], null);
                                    else
                                        property.SetValue(obj, null, null);
                                }
                                else
                                    property.SetValue(obj, null, null);
                            }
                            retorno.Add(obj);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    cn.Close();
                    cn.Dispose();
                }
            }

            return retorno;
        }

        public static BaseCollection<T> BuscarInversoID(T parameters)
        {
            ProcedureAttribute select = null;
            string currentfield = "";
            var procedures = Attribute.GetCustomAttributes(typeof(T), typeof(ProcedureAttribute));
            foreach (Attribute procedure in procedures)
            {
                if ((procedure as ProcedureAttribute).Type == ProcedureType.SelectID)
                    select = (procedure as ProcedureAttribute);
            }
            if (select == null)
                throw new Exception("Procedure de Select não configurada");
            BaseCollection<T> retorno = new BaseCollection<T>();

            var conn = Attribute.GetCustomAttributes(typeof(T), typeof(ConnectionStringAttribute))[0];
            if (conn == null)
                throw new Exception("Connection String não configurada");

            using (MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings[(conn as ConnectionStringAttribute).ConnectionStringName].ConnectionString))
            {
                try
                {
                    cn.Open();

                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = cn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = (select as ProcedureAttribute).ProcedureName;

                        PropertyInfo[] properties = typeof(T).GetProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            var parameter = Attribute.GetCustomAttributes(property, typeof(ParameterAttribute));
                            if (parameter.Length > 0)
                            {
                                if ((parameter[0] as ParameterAttribute).Type.HasFlag(ProcedureType.SelectID))
                                {
                                    command.Parameters.Insert(command.Parameters.Count < (parameter[0] as ParameterAttribute).Order ? command.Parameters.Count : (parameter[0] as ParameterAttribute).Order, new MySqlParameter((parameter[0] as ParameterAttribute).ParameterName, GetDbType(property.PropertyType)));
                                    command.Parameters[(parameter[0] as ParameterAttribute).ParameterName].Value = property.GetValue(parameters, null);
                                }
                            }
                        }

                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            var obj = Activator.CreateInstance<T>();
                            DataTable table = reader.GetSchemaTable();
                            int total = table.Rows.Count;

                            for (int i = 0; i < total; i++)
                            {
                                PropertyInfo property = getProperty(properties, table.Rows[i]["ColumnName"].ToString());
                                if (property != null)
                                {
                                    var DBField = Attribute.GetCustomAttributes(property, typeof(DBFieldAttribute));
                                    currentfield = (DBField[0] as DBFieldAttribute).FieldName;

                                    if (reader[currentfield] != DBNull.Value)
                                        if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                                            property.SetValue(obj, Convert.ToDateTime(reader[currentfield]), null);
                                        else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                                            property.SetValue(obj, Convert.ToBoolean(reader[currentfield]), null);
                                        else
                                            property.SetValue(obj, reader[currentfield], null);
                                    else
                                        property.SetValue(obj, null, null);
                                }
                            }
                            retorno.Add(obj);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    cn.Close();
                    cn.Dispose();
                }
            }

            return retorno;
        }

        public static object Inserir(T parameters)
        {
            ProcedureAttribute select = null;
            object result = null;
            var procedures = Attribute.GetCustomAttributes(typeof(T), typeof(ProcedureAttribute));
            foreach (Attribute procedure in procedures)
            {
                if ((procedure as ProcedureAttribute).Type == ProcedureType.Insert)
                    select = (procedure as ProcedureAttribute);
            }
            if (select == null)
                throw new Exception("Procedure de Select não configurada");
            BaseCollection<T> retorno = new BaseCollection<T>();

            var conn = Attribute.GetCustomAttributes(typeof(T), typeof(ConnectionStringAttribute))[0];
            if (conn == null)
                throw new Exception("Connection String não configurada");

            using (MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings[(conn as ConnectionStringAttribute).ConnectionStringName].ConnectionString))
            {
                try
                {
                    cn.Open();

                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = cn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = (select as ProcedureAttribute).ProcedureName;

                        PropertyInfo[] properties = typeof(T).GetProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            var parameter = Attribute.GetCustomAttributes(property, typeof(ParameterAttribute));
                            if (parameter.Length > 0)
                            {
                                if ((parameter[0] as ParameterAttribute).Type.HasFlag(ProcedureType.Insert))
                                {
                                    command.Parameters.Insert(command.Parameters.Count < (parameter[0] as ParameterAttribute).Order ? command.Parameters.Count : (parameter[0] as ParameterAttribute).Order, new MySqlParameter((parameter[0] as ParameterAttribute).ParameterName, GetDbType(property.PropertyType)));
                                    command.Parameters[(parameter[0] as ParameterAttribute).ParameterName].Value = property.GetValue(parameters, null);
                                }
                            }
                        }

                        result = command.ExecuteScalar();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    cn.Close();
                    cn.Dispose();
                }
            }

            return result;
        }
        public static bool Atualizar(T parameters)
        {
            ProcedureAttribute select = null;
            var procedures = Attribute.GetCustomAttributes(typeof(T), typeof(ProcedureAttribute));
            foreach (Attribute procedure in procedures)
            {
                if ((procedure as ProcedureAttribute).Type == ProcedureType.Update)
                    select = (procedure as ProcedureAttribute);
            }
            if (select == null)
                throw new Exception("Procedure de Select não configurada");
            BaseCollection<T> retorno = new BaseCollection<T>();


            var conn = Attribute.GetCustomAttributes(typeof(T), typeof(ConnectionStringAttribute))[0];
            if (conn == null)
                throw new Exception("Connection String não configurada");

            using (MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings[(conn as ConnectionStringAttribute).ConnectionStringName].ConnectionString))
            {
                try
                {
                    cn.Open();

                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = cn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = (select as ProcedureAttribute).ProcedureName;

                        PropertyInfo[] properties = typeof(T).GetProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            var parameter = Attribute.GetCustomAttributes(property, typeof(ParameterAttribute));
                            if (parameter.Length > 0)
                            {
                                if ((parameter[0] as ParameterAttribute).Type.HasFlag(ProcedureType.Update))
                                {
                                    command.Parameters.Insert(command.Parameters.Count < (parameter[0] as ParameterAttribute).Order ? command.Parameters.Count : (parameter[0] as ParameterAttribute).Order, new MySqlParameter((parameter[0] as ParameterAttribute).ParameterName, GetDbType(property.PropertyType)));
                                    command.Parameters[(parameter[0] as ParameterAttribute).ParameterName].Value = property.GetValue(parameters, null);
                                }
                            }
                        }

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    cn.Close();
                    cn.Dispose();
                }
            }

            return true;
        }
        public static bool Apagar(T parameters)
        {
            ProcedureAttribute select = null;
            var procedures = Attribute.GetCustomAttributes(typeof(T), typeof(ProcedureAttribute));
            foreach (Attribute procedure in procedures)
            {
                if ((procedure as ProcedureAttribute).Type == ProcedureType.Delete)
                    select = (procedure as ProcedureAttribute);
            }
            if (select == null)
                throw new Exception("Procedure de Delete não configurada");
            BaseCollection<T> retorno = new BaseCollection<T>();

            var conn = Attribute.GetCustomAttributes(typeof(T), typeof(ConnectionStringAttribute))[0];
            if (conn == null)
                throw new Exception("Connection String não configurada");

            using (MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings[(conn as ConnectionStringAttribute).ConnectionStringName].ConnectionString))
            {
                try
                {
                    cn.Open();

                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = cn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = (select as ProcedureAttribute).ProcedureName;

                        PropertyInfo[] properties = typeof(T).GetProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            var parameter = Attribute.GetCustomAttributes(property, typeof(ParameterAttribute));
                            if (parameter.Length > 0)
                            {
                                if ((parameter[0] as ParameterAttribute).Type.HasFlag(ProcedureType.Delete))
                                {
                                    command.Parameters.Insert(command.Parameters.Count < (parameter[0] as ParameterAttribute).Order? command.Parameters.Count : (parameter[0] as ParameterAttribute).Order, new MySqlParameter((parameter[0] as ParameterAttribute).ParameterName, GetDbType(property.PropertyType)));
                                    command.Parameters[(parameter[0] as ParameterAttribute).ParameterName].Value = property.GetValue(parameters, null);
                                }
                            }
                        }

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    cn.Close();
                    cn.Dispose();
                }
            }
            return true;
        }

        private static MySqlDbType GetDbType(Type o)
        {
            if (o == typeof(string)) return MySqlDbType.VarChar;
            if (o == typeof(DateTime)) return MySqlDbType.Date;
            if (o == typeof(DateTime?)) return MySqlDbType.Date;
            if (o == typeof(Int64)) return MySqlDbType.Int64;
            if (o == typeof(Int32)) return MySqlDbType.Int32;
            if (o == typeof(Int16)) return MySqlDbType.Int16;
            if (o == typeof(sbyte)) return MySqlDbType.Int16;
            if (o == typeof(byte)) return MySqlDbType.Int16;
            if (o == typeof(bool)) return MySqlDbType.Bit;
            if (o == typeof(bool?)) return MySqlDbType.Bit;
            if (o == typeof(decimal)) return MySqlDbType.Decimal;
            if (o == typeof(float)) return MySqlDbType.Float;
            if (o == typeof(double)) return MySqlDbType.Double;

            return MySqlDbType.VarChar;
        }
    }
}
