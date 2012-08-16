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
using System.Globalization;
using System.Xml;

namespace FB2Fix.ObjectModel
{
    public class DateTimeNode : DocumentNode
    {
        private static readonly string[] formats = new string[] { "yyyy-MM-dd", "'+'yyyy-MM-dd", "'-'yyyy-MM-dd", "yyyy-MM-ddzzz", "'+'yyyy-MM-ddzzz", "'-'yyyy-MM-ddzzz" };

        private DateTime? dateValue;
        private string dateString;

        public DateTime? Value
        {
            get
            {
                return this.dateValue;
            }
            set
            {
                this.dateValue = value;

                if(this.dateValue != null)
                {
                    this.dateString = this.dateValue.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                else
                {
                    this.dateString = null;
                }
            }
        }

        public string DateString
        {
            get
            {
                return this.dateString;
            }
        }

        public override void Load(XmlElement parentNode)
        {
            if (parentNode == null)
            {
                throw new ArgumentNullException("parentNode");
            }

            XmlAttribute attr = parentNode.Attributes["value"];
            if (attr != null)
            {
                this.dateValue = null;
                try
                {
                    dateValue = DateTime.ParseExact(attr.Value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                }
                catch (FormatException)
                {
                }
            }

            if (!String.IsNullOrEmpty(parentNode.InnerText))
            {
                this.dateString = parentNode.InnerText;
            }
            else if(this.dateValue != null)
            {
                this.dateString = this.dateValue.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
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

            if (this.dateValue != null)
            {
                XmlAttribute nameAttr = document.CreateAttribute("value");
                nameAttr.Value = this.dateValue.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                element.Attributes.Append(nameAttr);
            }

            if(!String.IsNullOrEmpty(this.dateString))
            {
                element.InnerText = this.dateString;
            }

            return element;
        }

        public override bool IsEmpty()
        {
            return (this.dateValue == null && String.IsNullOrEmpty(dateString));
        }
    }
}
