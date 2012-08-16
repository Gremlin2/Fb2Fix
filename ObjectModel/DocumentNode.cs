// Copyright (c) 2007-2012 Andrej Repin aka Gremlin2
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace FB2Fix.ObjectModel
{
	public abstract class DocumentNode
	{
        private static readonly string[] formats = new string[] { "yyyy-MM-dd", "'+'yyyy-MM-dd", "'-'yyyy-MM-dd", "yyyy-MM-ddzzz", "'+'yyyy-MM-ddzzz", "'-'yyyy-MM-ddzzz" };

	    protected DocumentNode()
	    {
	    }

        protected DateTime? LoadDateElement(XmlElement parentElement, string expression)
        {
            XmlNodeList nodes = parentElement.SelectNodes(expression);
            foreach (XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    XmlAttribute attr = node.Attributes["value"];
                    if(attr != null)
                    {
                        DateTime? value = null;
                        try
                        {
                            value = DateTime.ParseExact(attr.Value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                        }
                        catch (FormatException)
                        {
                        }

                        if(value != null)
                        {
                            return value;
                        }
                    }

                    if (!String.IsNullOrEmpty(node.InnerText))
                    {
                        try
                        {
                            return DateTime.Parse(node.InnerText);
                        }
                        catch (FormatException)
                        {
                            return null;
                        }
                    }
                }
            }

            return null;
        }

        protected string LoadElement(XmlElement parentElement, string expression)
        {
            XmlNodeList nodes = parentElement.SelectNodes(expression);
            foreach (XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    if (!String.IsNullOrEmpty(node.InnerText) && node.InnerText.Trim().Length > 0)
                    {
                        return node.InnerText.Trim();
                    }
                }
            }

            return null;
        }

        protected string LoadRequiredElement(XmlElement parentElement, string expression)
        {
            XmlNodeList nodes = parentElement.SelectNodes(expression);
            foreach (XmlNode node in nodes)
            {
                XmlElement element = node as XmlElement;
                if (element != null)
                {
                    if(element.InnerText == null || element.IsEmpty)
                    {
                        return null;
                    }

                    //if(element.InnerText.Trim().Length == 0)
                    //{
                    //    return null;
                    //}

                    return node.InnerText.Trim();
                }
            }

            return null;
        }

        protected XmlElement LoadElementXml(XmlElement parentElement, string expression)
        {
            XmlNodeList nodes = parentElement.SelectNodes(expression);
            foreach (XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    if (!String.IsNullOrEmpty(node.InnerXml))
                    {
                        return node as XmlElement;
                    }
                }
            }

            return null;
        }

		protected List<string> LoadElementsList(XmlElement parentElement, string expression)
		{
			XmlNodeList nodes = parentElement.SelectNodes(expression);
			List<string> list = new List<string>(nodes.Count);

			foreach(XmlNode node in nodes)
			{
				if(node.NodeType == XmlNodeType.Element)
				{
					if(!String.IsNullOrEmpty(node.InnerText) && node.InnerText.Trim().Length > 0)
					{
						list.Add(node.InnerText.Trim());
					}
				}
			}
			
			return list;
		}

        protected XmlElement StoreElement(XmlDocument document, string name, string value)
        {
            if(!String.IsNullOrEmpty(value))
            {
                XmlElement element = document.CreateElement(name);
                element.InnerText = value;
                return element;
            }

            return null;
        }

        protected XmlElement StoreRequiredElement(XmlDocument document, string name, string value)
        {
            if (value != null)
            {
                if (value.Length > 0)
                {
                    XmlElement element = document.CreateElement(name);
                    element.InnerText = value;
                    return element;
                }
                else
                {
                    XmlElement element = document.CreateElement(name);
                    element.IsEmpty = true;
                    return element;
                }
            }

            return null;
        }

        protected XmlElement StoreXmlElement(XmlDocument document, string name, XmlElement value)
        {
            if (value != null)
            {
                return value;
            }

            return null;
        }

        protected XmlElement StoreDateElement(XmlDocument document, string name, DateTime? value)
        {
            if(value != null)
            {
                XmlElement element = document.CreateElement(name);
                element.InnerText = value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                XmlAttribute attr = document.CreateAttribute("value");
                attr.Value = element.InnerText;

                element.Attributes.Append(attr);

                return element;
            }

            return null;
        }

	    public static List<T> LoadObjectList<T>(XmlElement parentElement, string expression) where T : DocumentNode, new()
        {
            XmlNodeList nodes = parentElement.SelectNodes(expression);
            List<T> list = new List<T>(nodes.Count);

            foreach(XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    T item = new T();
                    item.Load(node as XmlElement);
                    if (!item.IsEmpty())
                    {
                        list.Add(item);
                    }
                }
            }

            return list;
        }

        public static T LoadObject<T>(XmlElement parentElement, string xpath) where T : DocumentNode, new()
        {
            XmlNode node = parentElement.SelectSingleNode(xpath);
            if (node != null && node.NodeType == XmlNodeType.Element)
            {
                T item = new T();
                item.Load(node as XmlElement);
                return item;
            }

            return null;
        }

		public abstract void Load(XmlElement parentNode);
		public abstract XmlElement Store(XmlDocument document, XmlElement element);
	    public abstract bool IsEmpty();
	}
}
