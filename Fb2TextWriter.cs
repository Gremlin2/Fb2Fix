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
using System.IO;
using System.Text;
using System.Xml;

namespace FB2Fix
{
    internal class Fb2TextWriter : XmlTextWriter
    {
        private enum Fb2TextWriterState
        {
            None,
            StartDocument,
            Root,
            Description,
            Body
        }

        private bool indentHeader;
        private bool indentBody;
        private TextWriter writer;
        private Stack<string> elementStack;
        private Fb2TextWriterState state;

        public Fb2TextWriter(TextWriter w) : base(w)
        {
            this.writer = w;
            this.elementStack = new Stack<string>(16);
        }

        public Fb2TextWriter(Stream w, Encoding encoding) : this(encoding == null ? new StreamWriter(w) : new StreamWriter(w, encoding))
        {
        }

        public Fb2TextWriter(string filename, Encoding encoding) : this(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read), encoding)
        {
        }

        public bool IndentHeader
        {
            get
            {
                return this.indentHeader;
            }
            set
            {
                this.indentHeader = value;
            }
        }

        public bool IndentBody
        {
            get
            {
                return this.indentBody;
            }
            set
            {
                this.indentBody = value;
            }
        }

        public override void WriteStartDocument()
        {
            base.Formatting = Formatting.Indented;
            base.WriteStartDocument();
            state = Fb2TextWriterState.StartDocument;
        }

        public override void WriteStartDocument(bool standalone)
        {
            base.Formatting = Formatting.Indented;
            base.WriteStartDocument(standalone);
            state = Fb2TextWriterState.StartDocument;
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            this.elementStack.Push(localName);
            
            switch(this.state)
            {
                case Fb2TextWriterState.StartDocument:
                    if (String.Compare(localName, "FictionBook") == 0)
                    {
                        this.state = Fb2TextWriterState.Root;
                    }
                    break;

                case Fb2TextWriterState.Root:
                    if (String.Compare(localName, "description") == 0)
                    {
                        this.state = Fb2TextWriterState.Description;
                        if(!this.indentHeader)
                        {
                            base.Formatting = Formatting.None;
                        }
                    }
                    else if (String.Compare(localName, "body") == 0)
                    {
                        base.Formatting = (this.indentBody) ? Formatting.Indented : Formatting.None;
                        this.state = Fb2TextWriterState.Body;
                    }
                    else
                    {
                        base.Formatting = Formatting.Indented;
                    }
                    break;
            }

            base.WriteStartElement(prefix, localName, ns);
        }

        public override void WriteFullEndElement()
        {
            string localName = this.elementStack.Pop();

            base.WriteFullEndElement();

            switch (this.state)
            {
                case Fb2TextWriterState.Description:
                    if (!this.indentHeader)
                    {
                        switch (localName)
                        {
                            case "title-info":
                            case "src-title-info":
                            case "document-info":
                            case "publish-info":
                            case "custom-info":
                            case "output":
                                writer.WriteLine();
                                break;
                        }
                    }

                    if (String.Compare(localName, "description") == 0)
                    {
                        this.state = Fb2TextWriterState.Root;
                        writer.WriteLine();
                    }

                    break;

                case Fb2TextWriterState.Body:
                    if (!this.indentBody)
                    {
                        switch (localName)
                        {
                            case "image":
                            case "title":
                            case "epigraph":
                            case "section":
                            case "p":
                                writer.WriteLine();
                                break;
                        }
                    }

                    if(String.Compare(localName, "body") == 0)
                    {
                        this.state = Fb2TextWriterState.Root;
                    }

                    break;
            }

        }

        public override void WriteEndElement()
        {
            this.elementStack.Pop();
            base.WriteEndElement();
        }
    }
}
