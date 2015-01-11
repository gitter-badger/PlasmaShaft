using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PlasmaShaft
{
    public class Config
    {
        public XmlDocument doc = new XmlDocument();
        public XmlNode rootNode;

        public Config()
        {
        }

        public Config(string name)
        {
            this.LoadConfig(name);
            this.rootNode = this.doc.GetElementsByTagName("root")[0];
        }

        public string GetValue(string name)
        {
            if (this.doc.GetElementsByTagName(name)[0] != null)
                return this.doc.GetElementsByTagName(name)[0].InnerText;
            else
                return (string)null;
        }

        public string[] GetValues()
        {
            List<string> list = new List<string>();
            IEnumerator enumerator = this.rootNode.ChildNodes.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    XmlNode xmlNode = (XmlNode)enumerator.Current;
                    if (xmlNode.NodeType == XmlNodeType.Element)
                        list.Add(xmlNode.Name);
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = enumerator as IDisposable) != null)
                    disposable.Dispose();
            }
            return list.ToArray();
        }

        public void SetValue(string name, string value)
        {
            if (this.doc == null)
                this.doc = new XmlDocument();
            if (this.doc.DocumentElement.GetElementsByTagName(name)[0] == null)
            {
                this.doc.DocumentElement.AppendChild((XmlNode)this.doc.CreateElement(name));
            }
            this.doc.DocumentElement.GetElementsByTagName(name)[0].InnerText = value;
        }

        public void CreateNewConfig()
        {
            this.doc.AppendChild((XmlNode)this.doc.CreateXmlDeclaration("1.0", "UTF-8", (string)null));
            this.rootNode = (XmlNode)this.doc.CreateElement("root");
            this.doc.AppendChild(this.rootNode);
        }

        public void SaveConfig(string name)
        {
            if (!Directory.Exists("Settings"))
                Directory.CreateDirectory("Settings");
            this.doc.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings/" + name + ".xml"));
        }

        public void LoadConfig(string name)
        {
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings/" + name + ".xml")))
            {
                this.CreateNewConfig();
                this.SaveConfig(name);
            }
            else
                this.doc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings/" + name + ".xml"));
        }
    }
}
