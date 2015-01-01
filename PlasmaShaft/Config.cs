using System;
using System.Xml;
namespace PlasmaShaft
{
        /// <summary>
        /// Config class that allows you to get and set values of simple XML configuration files
        /// </summary>
        public class Config
        {
            public XmlDocument doc;
            public XmlNode rootNode;
            public Config()
            {
                doc = new XmlDocument();
            }
            public Config (string name)
            {
                doc = new XmlDocument();
                LoadConfig(name);
                rootNode = doc.GetElementsByTagName("root")[0];
            }
            public string GetValue(string name)
            {
                if(doc.GetElementsByTagName(name)[0] != null)
                    return doc.GetElementsByTagName(name)[0].InnerText;
                else
                    return null;
            }
            public string[] GetValues()
            {
                System.Collections.Generic.List<string> values = new System.Collections.Generic.List<string>();
                foreach(XmlNode node in rootNode.ChildNodes)
                    if(node.NodeType == XmlNodeType.Element)
                        values.Add(node.Name);
                return values.ToArray();
            }
            public void SetValue(string name, string value)
            {
                if(doc.GetElementsByTagName(name)[0] == null)
                    rootNode.AppendChild(doc.CreateElement(name));
                doc.GetElementsByTagName(name)[0].InnerText = value;
            }
            public void CreateNewConfig()
            {
                doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
                rootNode = doc.CreateElement("root");
                doc.AppendChild(rootNode);
            }
            public void SaveConfig(string name)
            {
                doc.Save(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings/"+name+".xml"));
            }
            public void LoadConfig(string name)
            {
                if(!System.IO.File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings/"+name+".xml")))
                {
                    CreateNewConfig();
                    SaveConfig(name);
                }
                else
                    doc.Load(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings/"+name+".xml"));
        }
    }
}
