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
    public class PublishInfoNode : DocumentNode
    {
        private string bookName;
        private string publisher;
        private string city;
        private int? year;
        private string isbn;
        private List<SequenceInfoNode> sequences;

        public PublishInfoNode()
        {
            this.sequences = new List<SequenceInfoNode>();
        }

        public string BookName
        {
            get
            {
                return this.bookName;
            }
            set
            {
                this.bookName = value;
            }
        }

        public string Publisher
        {
            get
            {
                return this.publisher;
            }
            set
            {
                this.publisher = value;
            }
        }

        public string City
        {
            get
            {
                return this.city;
            }
            set
            {
                this.city = value;
            }
        }

        public int? Year
        {
            get
            {
                return this.year;
            }
            set
            {
                this.year = value;
            }
        }

        public string ISBN
        {
            get
            {
                return this.isbn;
            }
            set
            {
                this.isbn = value;
            }
        }

        public List<SequenceInfoNode> Sequences
        {
            get
            {
                return this.sequences;
            }
        }

        public override void Load(XmlElement parentNode)
        {
            if (parentNode == null)
            {
                throw new ArgumentNullException("parentNode");
            }

            this.year = null;

            this.bookName = LoadElement(parentNode, "./book-name");
            this.publisher = LoadElement(parentNode, "./publisher");
            this.city = LoadElement(parentNode, "./city");

            string yearValue = LoadElement(parentNode, "./year");
            if (!String.IsNullOrEmpty(yearValue))
            {
                try
                {
                    this.year = DateTime.ParseExact(yearValue, new string[] {"yyyy", "'+'yyyy", "'-'yyyy", "yyyyzzz", "'+'yyyyzzz", "'-'yyyyzzz"}, CultureInfo.InvariantCulture, DateTimeStyles.None).Year;
                }
                catch (FormatException)
                {
                }
            }

            this.isbn = LoadElement(parentNode, "./isbn");
            this.sequences = LoadObjectList<SequenceInfoNode>(parentNode, "./sequence");
        }

        public override XmlElement Store(XmlDocument document, XmlElement element)
        {
            XmlElement childElement;

            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if ((childElement = StoreElement(document, "book-name", this.bookName)) != null)
            {
                element.AppendChild(childElement);
            }

            if ((childElement = StoreElement(document, "publisher", this.publisher)) != null)
            {
                element.AppendChild(childElement);
            }

            if ((childElement = StoreElement(document, "city", this.city)) != null)
            {
                element.AppendChild(childElement);
            }

            if (this.year != null)
            {
                if ((childElement = StoreElement(document, "year", this.year.Value.ToString(CultureInfo.InvariantCulture))) != null)
                {
                    element.AppendChild(childElement);
                }
            }

            if ((childElement = StoreElement(document, "isbn", this.isbn)) != null)
            {
                element.AppendChild(childElement);
            }

            foreach (SequenceInfoNode sequence in this.sequences)
            {
                childElement = document.CreateElement("sequence");
                if ((childElement = sequence.Store(document, childElement)) != null)
                {
                    element.AppendChild(childElement);
                }
            }

            return element;
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }
}
