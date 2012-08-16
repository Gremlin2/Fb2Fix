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
using System.Text;
using System.Xml;

namespace FB2Fix.ObjectModel
{
    public class TitleInfoNode : DocumentNode
    {
        private List<GenreInfoNode> genres;
        private List<AuthorInfoNode> authors;
        private string bookTitle;
        private AnnotationInfoNode annotation;
        private string keywords;
        private DateTimeNode date;
        private XmlElement coverpage;
        private string lang;
        private string sourceLang;
        private List<AuthorInfoNode> translators;
        private List<SequenceInfoNode> sequences;

        public TitleInfoNode()
        {
            this.genres = new List<GenreInfoNode>();
            this.authors = new List<AuthorInfoNode>();
            this.translators = new List<AuthorInfoNode>();
            this.sequences = new List<SequenceInfoNode>();
        }

        public List<GenreInfoNode> Genres
        {
            get
            {
                return this.genres;
            }
        }

        public List<AuthorInfoNode> Authors
        {
            get
            {
                return this.authors;
            }
        }

        public string BookTitle
        {
            get
            {
                return this.bookTitle;
            }
            set
            {
                this.bookTitle = value;
            }
        }

        public AnnotationInfoNode Annotation
        {
            get
            {
                return this.annotation;
            }
            set
            {
                this.annotation = value;
            }
        }

        public string Keywords
        {
            get
            {
                return this.keywords;
            }
            set
            {
                this.keywords = value;
            }
        }

        public DateTime? Date
        {
            get
            {
                if (this.date != null)
                {
                    if (this.date.Value != null)
                    {
                        return this.date.Value;
                    }
                    else
                    {
                        return DateTime.MinValue;
                    }
                }

                return null;
            }
            set
            {
                if(this.date == null)
                {
                    this.date = new DateTimeNode();
                }

                this.date.Value = value;
            }
        }

        public XmlElement Coverpage
        {
            get
            {
                return this.coverpage;
            }
            set
            {
                this.coverpage = value;
            }
        }

        public string Lang
        {
            get
            {
                return this.lang;
            }
            set
            {
                this.lang = value;
            }
        }

        public string SourceLang
        {
            get
            {
                return this.sourceLang;
            }
            set
            {
                this.sourceLang = value;
            }
        }

        public List<AuthorInfoNode> Translators
        {
            get
            {
                return this.translators;
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

            this.genres = LoadObjectList<GenreInfoNode>(parentNode, "./genre");
            this.authors = LoadObjectList<AuthorInfoNode>(parentNode, "./author");
            this.bookTitle = LoadRequiredElement(parentNode, "./book-title");
            this.annotation = LoadObject<AnnotationInfoNode>(parentNode, "./annotation");
            this.keywords = LoadElement(parentNode, "./keywords");
            this.date = LoadObject<DateTimeNode>(parentNode, "./date");
            this.coverpage = LoadElementXml(parentNode, "./coverpage");
            this.lang = LoadRequiredElement(parentNode, "./lang");
            this.sourceLang = LoadElement(parentNode, "src-lang");
            this.translators = LoadObjectList<AuthorInfoNode>(parentNode, "./translator");
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

            foreach (GenreInfoNode genre in this.genres)
            {
                childElement = document.CreateElement("genre");
                if ((childElement = genre.Store(document, childElement)) != null)
                {
                    element.AppendChild(childElement);
                }
            }

            foreach (AuthorInfoNode authorInfoNode in authors)
            {
                childElement = document.CreateElement("author");
                if ((childElement = authorInfoNode.Store(document, childElement)) != null)
                {
                    element.AppendChild(childElement);
                }
            }

            if ((childElement = StoreRequiredElement(document, "book-title", this.bookTitle)) != null)
            {
                element.AppendChild(childElement);
            }

            if(this.annotation != null)
            {
                childElement = document.CreateElement("annotation");
                if ((childElement = annotation.Store(document, childElement)) != null)
                {
                    element.AppendChild(childElement);
                }
            }

            if ((childElement = StoreElement(document, "keywords", this.keywords)) != null)
            {
                element.AppendChild(childElement);
            }

            if(this.date != null)
            {
                childElement = document.CreateElement("date");
                if ((childElement = this.date.Store(document, childElement)) != null)
                {
                    element.AppendChild(childElement);
                }
            }

            if ((childElement = StoreXmlElement(document, "coverpage", this.coverpage)) != null)
            {
                element.AppendChild(childElement);
            }

            if ((childElement = StoreRequiredElement(document, "lang", this.lang)) != null)
            {
                element.AppendChild(childElement);
            }

            if ((childElement = StoreElement(document, "src-lang", this.sourceLang)) != null)
            {
                element.AppendChild(childElement);
            }

            foreach (AuthorInfoNode translator in this.translators)
            {
                childElement = document.CreateElement("translator");
                if ((childElement = translator.Store(document, childElement)) != null)
                {
                    element.AppendChild(childElement);
                }
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
