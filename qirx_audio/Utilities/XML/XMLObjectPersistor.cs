using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using softsyst.Generic.Logger;
using softsyst.qirx.configuration;

//https://stackoverflow.com/questions/4123590/serialize-an-object-to-xml

namespace softsyst.Generic.XML
{
    public class XMLObjectPersistor <T> where T:class
    {
        logging<XMLObjectPersistor<T>> logger = new logging<XMLObjectPersistor<T>>(logging2.log);

        string _directory;
        public string Path { get; set; }
        string _extension = ".xml";

        /// <summary>
        /// File name without extension
        /// </summary>
        string _bareFileName;

        /// <summary>
        /// The instance to save
        /// </summary>
        T _instance;

        public XMLObjectPersistor(string name, T instance)
        {
            try
            {
                _bareFileName = name;
                configFileProgram cfg = new configFileProgram();
                cfg.readConfiguration();
                _directory = cfg.PersistenceDirectory;
                Path = _directory + _bareFileName + _extension;

                if (!Directory.Exists(_directory))
                {
                    Directory.CreateDirectory(_directory);
                    logger.Info($"Directory {_directory} created.");
                }
                _instance = instance;
            }
            catch (Exception e)
            {
                logger.Error("Error constructing the XML Persistor: " + e.Message);
            }
        }
        public void Save()
        {
            try
            {
                //first serialize the object to memory stream,
                //in case of exception, the original file is not corrupted
                using (MemoryStream ms = new MemoryStream())
                {
                    var writer = new System.IO.StreamWriter(ms);
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, _instance);
                    writer.Flush();

                    //if the serialization succeed, rewrite the file.
                    File.WriteAllBytes(Path, ms.ToArray());
                }
            }
            catch (Exception e)
            {
                logger.Error("Error Saving to the XML Persistor: " + e.Message);
            }
        }

        public T Load()
        {
            if (!File.Exists(Path))
            {
                string s = $"xml Path not existing {Path}";
                logger.Error(s);
                throw new Exception(s);
            }
            using (var stream = System.IO.File.OpenRead(Path))
            {
                var serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(stream) as T;
            }
        }
    }
}
