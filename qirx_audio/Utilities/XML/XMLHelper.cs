using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using log4net;
using softsyst.Generic.Logger;
using System.Threading;

namespace softsyst.Generic.XML
{
    public sealed class XMLHelper
    {
        static ILog logger = LogManager.GetLogger(typeof(XMLHelper));

        /// <summary>
        /// Useful static xml functions
        /// </summary>
        /// <summary>
        /// reads the node attribute according to a xpath string
        /// </summary>
        /// <param name="path">Complete path of the xml file</param>
        /// <param name="nodeName">Xml node name</param>
        /// <param name="attribName">Xml attribute name</param>
        /// <returns>The attribute value</returns>
        public static XmlNodeList readXMLNodeList(string path, string xPath)
        {
            try
            {
                if (!File.Exists(path))
                    return null;
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlElement root = doc.DocumentElement;
                XmlNodeList xlist = root.SelectNodes(xPath);
                return xlist;
            }
            catch(Exception)
            {
                logging2.logError(logger, string.Format("Reading Configuration failed. Path = {0}, xPath = {1}",
                   path, xPath));
                return null;
            }
        }

        /// <summary>
        /// Useful static xml functions
        /// </summary>
        /// <summary>
        /// reads the node attribute according to a xpath string
        /// </summary>
        /// <param name="path">Complete path of the xml file</param>
        /// <param name="nodeName">Xml node name</param>
        /// <param name="attribName">Xml attribute name</param>
        /// <returns>The attribute value</returns>
        public static string readSingleXMLNodeAttrib(string path, string nodeName, string attribName)
        {
            try
            {
                if (!File.Exists(path))
                    return string.Empty;
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlElement root = doc.DocumentElement;
                XmlNode node = root.SelectSingleNode(nodeName);
                string s = node.Attributes.GetNamedItem(attribName).Value;
                return s;
            }
            catch(Exception e)
            {
                logging2.logError(logger, string.Format("Reading Configuration failed. Path = {0}, Node name = {1}, Attribute Name = {2}, Error = {3}",
                   path, nodeName, attribName, e.Message));
                return string.Empty;
            }
        }

        static object xmlWriteLock = new object();
        public static void writeSingleXMLNodeAttrib(string path, string node_xPath, string attributeName, string attributeValue)
        {
            Monitor.Enter(xmlWriteLock);
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlElement root = doc.DocumentElement;
                XmlNode node = root.SelectSingleNode(node_xPath);
                node.Attributes.GetNamedItem(attributeName).Value = attributeValue;
                doc.Save(path);
            }
            catch (Exception e)
            {
                logging2.logError(logger, string.Format("Writing Configuration failed. Path = {0}, Node name = {1}, Attribute Name = {2}, Attribute Value = {3}, Error = {4}",
                   path, node_xPath, attributeName, attributeValue, e.Message));
            }
            finally
            {
                Monitor.Exit(xmlWriteLock);
            }
        }
    }
}
