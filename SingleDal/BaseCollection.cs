using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Framework.SingleDal
{
    public class BaseCollection<T> : List<T>
    {
        /// <summary>
        /// Gera uma string Json que representa a coleção
        /// </summary>
        /// <returns>String no formato Json</returns>
        public string ToJsonString()
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(this.GetType());

            MemoryStream stream = new MemoryStream();
            ser.WriteObject(stream, this);
            stream.Position = 0;

            StreamReader reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}
