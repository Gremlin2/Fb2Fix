using System;
using System.Xml;

namespace FB2Fix.ObjectModel
{
    public class CustomInfoNode : DocumentNode
    {
        private string text;
        private string infoType;

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }

        public string InfoType
        {
            get
            {
                return infoType;
            }
            set
            {
                infoType = value;
            }
        }

        public XmlNode XmlNode { get; set; }

        public override void Load(XmlElement parentNode)
        {
            if (parentNode == null)
            {
                throw new ArgumentNullException("parentNode");
            }

            if (!String.IsNullOrEmpty(parentNode.InnerText) && parentNode.InnerText.Trim().Length > 0)
            {
                infoType = parentNode.InnerText.Trim();
            }

            XmlAttribute typeAttr = parentNode.Attributes["info-type"];
            if (typeAttr != null && !String.IsNullOrEmpty(typeAttr.Value))
            {
                infoType = typeAttr.Value;
            }
        }

        public override XmlElement Store(XmlDocument document, XmlElement element)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (IsEmpty())
            {
                return null;
            }

            element.InnerXml = text;

            if (!String.IsNullOrEmpty(infoType))
            {
                XmlAttribute attribute = document.CreateAttribute("info-type");
                attribute.Value = infoType;

                element.Attributes.Append(attribute);
            }

            return element;
        }

        public override bool IsEmpty()
        {
            return String.IsNullOrEmpty(text);
        }
    }
}
