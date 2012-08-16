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
    public class GenreInfoNode : DocumentNode, IEquatable<GenreInfoNode>
    {
        private string genre;
        private int? match;

        public GenreInfoNode() : this(null, null)
        {
        }

        public GenreInfoNode(string genre) : this(genre, null)
        {
        }

        public GenreInfoNode(string genre, int? match)
        {
            this.genre = genre;
            this.match = match;
        }

        public string Genre
        {
            get
            {
                return this.genre;
            }
            set
            {
                this.genre = value;
            }
        }

        public int? Match
        {
            get
            {
                return this.match;
            }
            set
            {
                this.match = value;
            }
        }

        public override void Load(XmlElement parentNode)
        {
            if (parentNode == null)
            {
                throw new ArgumentNullException("parentNode");
            }

            this.genre = null;
            this.match = null;

            if (!String.IsNullOrEmpty(parentNode.InnerText) && parentNode.InnerText.Trim().Length > 0)
            {
                this.genre = parentNode.InnerText.Trim();
            }
            
            XmlAttribute matchAttr = parentNode.Attributes["match"];
            if(matchAttr != null && !String.IsNullOrEmpty(matchAttr.Value))
            {
                try
                {
                    match = Int32.Parse(matchAttr.Value, CultureInfo.InvariantCulture);
                }
                catch(FormatException)
                {
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

            if (this.IsEmpty())
            {
                return null;
            }

            if (this.genre != null)
            {
                element.InnerXml = this.genre;
            }

            if (this.match != null)
            {
                XmlAttribute attribute = document.CreateAttribute("match");
                attribute.Value = this.match.Value.ToString(CultureInfo.InvariantCulture);
                element.Attributes.Append(attribute);
            }

            return element;
        }

        public override bool IsEmpty()
        {
            return String.IsNullOrEmpty(this.genre);
        }

        public bool Equals(GenreInfoNode genreInfoNode)
        {
            if (genreInfoNode == null)
            {
                return false;
            }

            return Equals(genre, genreInfoNode.genre) && Equals(match, genreInfoNode.match);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            
            return Equals(obj as GenreInfoNode);
        }

        public override int GetHashCode()
        {
            return (genre != null ? genre.GetHashCode() : 0) + 29 * match.GetHashCode();
        }
    }
}
