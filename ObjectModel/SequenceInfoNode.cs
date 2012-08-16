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
using System.Text;
using System.Xml;

namespace FB2Fix.ObjectModel
{
    public class SequenceInfoNode : DocumentNode
    {
        private string name;
        private int? number;
        private readonly List<SequenceInfoNode> children;

        public SequenceInfoNode()
        {
            children = new List<SequenceInfoNode>();
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public int? Number
        {
            get
            {
                return this.number;
            }
            set
            {
                this.number = value;
            }
        }

        public List<SequenceInfoNode> Children
        {
            get
            {
                return this.children;
            }
        }

        public override void Load(XmlElement parentNode)
        {
            if (parentNode == null)
            {
                throw new ArgumentNullException("parentNode");
            }

            this.name = null;
            this.number = null;

            XmlAttribute nameAttr = parentNode.Attributes["name"];
            if(nameAttr != null)
            {
                if(!String.IsNullOrEmpty(nameAttr.Value))
                {
                    this.name = nameAttr.Value;
                }
            }

            XmlAttribute numberAttr = parentNode.Attributes["number"];
            if(numberAttr != null)
            {
                if(!String.IsNullOrEmpty(numberAttr.Value))
                {
                    try
                    {
                        this.number = Int32.Parse(numberAttr.Value, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                    }
                }
            }

            children.Clear();
            
            XmlNodeList nodes = parentNode.SelectNodes("./sequence");
            if(nodes != null)
            {
                foreach (XmlElement element in nodes)
                {
                    SequenceInfoNode child = new SequenceInfoNode();
                    child.Load(element);

                    children.Add(child);
                }
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

            if(IsEmpty())
            {
                return null;
            }

            XmlAttribute nameAttr = document.CreateAttribute("name");
            nameAttr.Value = this.name;
            element.Attributes.Append(nameAttr);

            if(this.number != null)
            {
                XmlAttribute numberAttr = document.CreateAttribute("number");
                numberAttr.Value = this.number.Value.ToString(CultureInfo.InvariantCulture);
                element.Attributes.Append(numberAttr);
            }

            foreach (SequenceInfoNode node in children)
            {
                XmlElement childElement = document.CreateElement("sequence");
                if ((childElement = node.Store(document, childElement)) != null)
                {
                    element.AppendChild(childElement);
                }
            }

            return element;
        }

        public override bool IsEmpty()
        {
            return String.IsNullOrEmpty(this.name) && children.Count == 0;
        }
    }
}
