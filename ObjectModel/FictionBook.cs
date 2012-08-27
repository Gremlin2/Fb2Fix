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
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using FB2Fix.Sgml;

namespace FB2Fix.ObjectModel
{
    public class FictionBook
    {
        private static Regex librusecIdPattern;

        private readonly XmlDocument document;

        private DocumentInfoNode documentInfo;
        private TitleInfoNode titleInfo;
        private TitleInfoNode srcTitleInfo;
        private PublishInfoNode publishInfo;
        
        private XmlNode documentInfoNode;
        private XmlNode titleInfoNode;
        private XmlNode srcTitleInfoNode;
        private XmlNode publishInfoNode;

        private XmlNode descriptionNode;

        private List<CustomInfoNode> customInfos;

        private Fb2FixStatus documentStatus;
        private ModificationType modificationType;
        private DateTime containerDateTime;

        static FictionBook()
        {
            librusecIdPattern = new Regex(@"^\w{3}\s\w{3}\s\d{1,2}\s\d{2}:\d{2}:\d{2}\s\d{4}$");
        }

        public DocumentInfoNode DocumentInfo
        {
            get
            {
                return this.documentInfo;
            }
        }

        public TitleInfoNode TitleInfo
        {
            get
            {
                return this.titleInfo;
            }
        }

        public TitleInfoNode SrcTitleInfo
        {
            get
            {
                return this.srcTitleInfo;
            }
        }

        public PublishInfoNode PublishInfo
        {
            get
            {
                return this.publishInfo;
            }
        }

        public XmlNode DescriptionNode
        {
            get
            {
                return this.descriptionNode;
            }
        }

        public XmlDocument Document
        {
            get
            {
                return this.document;
            }
        }
        
        public Fb2FixStatus DocumentStatus
        {
            get 
            {
                return this.documentStatus;
            }

            set
            {
                if(this.documentStatus != value)
                {
                    ChangeDocumentStatus(value);
                    this.documentStatus = value;
                }
            }
        }

        public ModificationType ModificationType
        {
            get
            {
                return this.modificationType;
            }

            set
            {
                this.modificationType |= value;
            }
        }

        public bool Modified
        {
            get
            {
                return (this.modificationType != ModificationType.None);
            }
        }

        public float Version
        {
            get
            {
                return this.documentInfo.Version ?? 0.0f;
            }

            set
            {
                if((this.documentInfo.Version ?? 0.0) != value)
                {
                    ChangeDocumentVersion(value);
                    this.documentInfo.Version = value;
                }
            }
        }

        public DateTime ContainerDateTime
        {
            get
            {
                return this.containerDateTime;
            }
            set
            {
                this.containerDateTime = value;
            }
        }

        public FictionBook(XmlDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            this.document = document;

            this.documentStatus = Fb2FixStatus.None;

            XmlNode statusInfoNode = document.SelectSingleNode("//FictionBook/description/custom-info[@info-type='fb2fix-status']");
            if (statusInfoNode != null && statusInfoNode.NodeType == XmlNodeType.Element)
            {
                if (String.IsNullOrEmpty(statusInfoNode.InnerText))
                {
                    try
                    {
                        this.documentStatus = (Fb2FixStatus)Enum.Parse(typeof(Fb2FixStatus), statusInfoNode.InnerText, true);
                    }
                    catch (ArgumentException)
                    {
                    }
                }
            }

            this.documentInfoNode = document.SelectSingleNode("//FictionBook/description/document-info");
            if (this.documentInfoNode != null && this.documentInfoNode.NodeType == XmlNodeType.Element)
            {
                documentInfo = new DocumentInfoNode();
                documentInfo.Load(this.documentInfoNode as XmlElement);
            }

            this.titleInfoNode = document.SelectSingleNode("//FictionBook/description/title-info");
            if (this.titleInfoNode != null && this.titleInfoNode.NodeType == XmlNodeType.Element)
            {
                titleInfo = new TitleInfoNode();
                titleInfo.Load(this.titleInfoNode as XmlElement);
            }

            if (titleInfo == null)
            {
                throw new InvalidFictionBookFormatException();
            }

            this.srcTitleInfoNode = document.SelectSingleNode("//FictionBook/description/src-title-info");
            if (this.srcTitleInfoNode != null && this.srcTitleInfoNode.NodeType == XmlNodeType.Element)
            {
                srcTitleInfo = new TitleInfoNode();
                srcTitleInfo.Load(this.srcTitleInfoNode as XmlElement);
            }

            this.publishInfoNode = document.SelectSingleNode("//FictionBook/description/publish-info");
            if (this.publishInfoNode != null && this.publishInfoNode.NodeType == XmlNodeType.Element)
            {
                publishInfo = new PublishInfoNode();
                publishInfo.Load(this.publishInfoNode as XmlElement);
            }

            this.descriptionNode = document.SelectSingleNode("//FictionBook/description");

            if (this.descriptionNode == null)
            {
                throw new InvalidFictionBookFormatException();
            }

            XmlNodeList nodes = document.SelectNodes("//FictionBook/description/custom-info");

            customInfos = new List<CustomInfoNode>(nodes.Count);

            foreach (XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    CustomInfoNode item = new CustomInfoNode();
                    item.Load((XmlElement) node);

                    if (!String.IsNullOrEmpty(item.InfoType))
                    {
                        switch (item.InfoType)
                        {
                            case "fb2fix-status":
                            case "librusec-id":
                            case "previous-id":
                                continue;
                        }
                    }

                    item.XmlNode = node;
                    customInfos.Add(item);
                }
            }

            this.modificationType = ModificationType.None;
            this.containerDateTime = DateTime.Now;
        }

        private void ChangeDocumentStatus(Fb2FixStatus status)
        {
            XmlNode statusInfoNode = document.SelectSingleNode("//FictionBook/description/custom-info[@info-type='fb2fix-status']");
            if (statusInfoNode != null && statusInfoNode.NodeType == XmlNodeType.Element)
            {
                statusInfoNode.InnerText = Enum.GetName(typeof(Fb2FixStatus), status);
            }
            else
            {
                XmlElement xmlStatusNode = document.CreateElement("custom-info");
                xmlStatusNode.InnerText = Enum.GetName(typeof(Fb2FixStatus), Fb2FixStatus.Passed);

                XmlAttribute statusAttr = document.CreateAttribute("info-type");
                statusAttr.Value = "fb2fix-status";

                xmlStatusNode.Attributes.Append(statusAttr);

                this.descriptionNode.AppendChild(xmlStatusNode);
            }
        }

        private void ChangeDocumentVersion(float version)
        {
            XmlNode versionInfoNode = document.SelectSingleNode("//FictionBook/description/document-info/version");
            if (versionInfoNode != null && versionInfoNode.NodeType == XmlNodeType.Element)
            {
                versionInfoNode.InnerText = DocumentInfoNode.FormatVersion(version);
            }
            else
            {
                throw new InvalidFictionBookFormatException();
            }
        }

        private static string ComputeDocumentId(string value)
        {
            byte[] hash;
            StringBuilder documentId = new StringBuilder(40);

            MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();
            hash = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(value));

            documentId.Append("fb2-");

            for (int index = 0; index < hash.Length; index++)
            {
                switch (index)
                {
                    case 4:
                    case 6:
                    case 8:
                    case 10:
                        documentId.Append("-");
                        documentId.Append(hash[index].ToString("X2"));
                        break;

                    default:
                        documentId.Append(hash[index].ToString("X2"));
                        break;
                }
            }

            return documentId.ToString();
        }

        private void CheckAuthorInfo(IEnumerable<AuthorInfoNode> authors)
        {
            foreach (AuthorInfoNode author in authors)
            {
                if (author.FirstName == null && author.LastName != null)
                {
                    author.FirstName = String.Empty;
                    this.modificationType |= ModificationType.Description;
                }
                else if (author.FirstName != null && author.LastName == null)
                {
                    author.LastName = String.Empty;
                    this.modificationType |= ModificationType.Description;
                }
                else if ((String.IsNullOrEmpty(author.FirstName) && String.IsNullOrEmpty(author.LastName)) && String.IsNullOrEmpty(author.NickName))
                {
                    author.NickName = "FB2Fix";
                    this.modificationType |= ModificationType.Description;
                }
            }
        }

        public void CheckDocumentHeader(Fb2FixArguments options)
        {
            foreach (TitleInfoNode infoNode in new TitleInfoNode[] { titleInfo, srcTitleInfo })
            {
                if (infoNode == null)
                {
                    continue;
                }

                if (infoNode.Genres.Count == 0)
                {
                    infoNode.Genres.Add(new GenreInfoNode("nonfiction"));
                    this.modificationType |= ModificationType.Description;
                }

                CheckAuthorInfo(infoNode.Authors);

                if (infoNode.BookTitle == null)
                {
                    if (publishInfo != null && !String.IsNullOrEmpty(publishInfo.BookName))
                    {
                        infoNode.BookTitle = publishInfo.BookName;
                        this.modificationType |= ModificationType.Description;
                    }
                    else if(titleInfo != null && !String.IsNullOrEmpty(titleInfo.BookTitle))
                    {
                        infoNode.BookTitle = titleInfo.BookTitle;
                        this.modificationType |= ModificationType.Description;
                    }
                    else
                    {
                        throw new InvalidFictionBookFormatException();
                    }
                }

                if (infoNode.Lang == null)
                {
                    infoNode.Lang = "ru";
                    this.modificationType |= ModificationType.Description;
                }

                CheckAuthorInfo(infoNode.Translators);
            }

            if (options.mapGenres)
            {
                foreach (TitleInfoNode infoNode in new TitleInfoNode[] { titleInfo, srcTitleInfo })
                {
                    if (infoNode == null)
                    {
                        continue;
                    }

                    foreach (GenreInfoNode genre in infoNode.Genres)
                    {
                        if(!genre.IsEmpty())
                        {
                            if(GenreTable.Table.MapTable.ContainsKey(genre.Genre))
                            {
                                genre.Genre = GenreTable.Table.MapTable[genre.Genre];
                                this.modificationType |= ModificationType.Description;
                            }
                        }
                    }

                    Set<GenreInfoNode> genres = new Set<GenreInfoNode>();
                    foreach (GenreInfoNode genre in infoNode.Genres)
                    {
                        genres.Add(genre);
                    }

                    infoNode.Genres.Clear();
                    infoNode.Genres.AddRange(genres);
                }
            }

            if (documentInfo == null)
            {
                documentInfo = new DocumentInfoNode();

                AuthorInfoNode documentAuthor = new AuthorInfoNode();
                documentAuthor.NickName = "FB2Fix";

                documentInfo.Authors.Add(documentAuthor);
                documentInfo.Id = ComputeDocumentId(document.DocumentElement.InnerText);
                documentInfo.Date = DateTime.Now;
                documentInfo.ProgramUsed = "FB2Fix";
                documentInfo.Version = 0.0f;

                this.modificationType |= ModificationType.DocumentInfo;
            }
            else
            {
                CheckAuthorInfo(documentInfo.Authors);

                if (documentInfo.Date == null)
                {
                    documentInfo.Date = DateTime.Now;
                    this.modificationType |= ModificationType.Description;
                }

                if (documentInfo.Version == null)
                {
                    documentInfo.Version = 0.0f;
                    this.modificationType |= ModificationType.DocumentInfo;
                }

                if (!options.regenerateId)
                {
                    string programUsed = this.documentInfo.ProgramUsed ?? String.Empty;
                    //if (String.Compare(documentInfo.ProgramUsed, "LibRusEc kit", true, CultureInfo.InvariantCulture) == 0)
                    if (programUsed.IndexOf("LibRusEc kit", StringComparison.InvariantCultureIgnoreCase) >= 0 && !String.IsNullOrEmpty(documentInfo.Id))
                    {
                        //@"^\w{3}\s\w{3}\s\d{1,2}\s\d{2}:\d{2}:\d{2}\s\d{4}$"
                        if (librusecIdPattern.Match(documentInfo.Id).Success)
                        {
                            if (document.SelectSingleNode("//FictionBook/description/custom-info[@info-type='librusec-id']") == null)
                            {
                                XmlElement xmlLibRusEcId = document.CreateElement("custom-info");
                                xmlLibRusEcId.InnerText = documentInfo.Id;

                                XmlAttribute attr = document.CreateAttribute("info-type");
                                attr.Value = "librusec-id";

                                xmlLibRusEcId.Attributes.Append(attr);
                                this.descriptionNode.AppendChild(xmlLibRusEcId);

                                documentInfo.Id = ComputeDocumentId(document.DocumentElement.InnerText);
                                this.modificationType |= ModificationType.Description;
                            }
                        }
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(documentInfo.Id))
                    {
                        XmlElement xmlPreviousId = this.document.SelectSingleNode("//FictionBook/description/custom-info[@info-type='previous-id']") as XmlElement;

                        if (xmlPreviousId == null)
                        {
                            xmlPreviousId = document.CreateElement("custom-info");

                            XmlAttribute attr = document.CreateAttribute("info-type");
                            attr.Value = "previous-id";

                            xmlPreviousId.Attributes.Append(attr);
                            this.descriptionNode.AppendChild(xmlPreviousId);
                        }

                        xmlPreviousId.InnerText = documentInfo.Id;
                    }

                    documentInfo.Id = ComputeDocumentId(document.DocumentElement.InnerText);
                    this.modificationType |= ModificationType.Description;
                }

                if (String.IsNullOrEmpty(documentInfo.Id))
                {
                    documentInfo.Id = ComputeDocumentId(document.DocumentElement.InnerText);
                    this.modificationType |= ModificationType.Description;
                }

                CheckAuthorInfo(documentInfo.Publishers);
            }

            XmlElement xmlNewTitleInfo = document.CreateElement("title-info");
            xmlNewTitleInfo = titleInfo.Store(document, xmlNewTitleInfo);
            this.descriptionNode.ReplaceChild(xmlNewTitleInfo, titleInfoNode);
            titleInfoNode = xmlNewTitleInfo;

            if (srcTitleInfo != null)
            {
                XmlElement xmlNewSrcTitleInfo = document.CreateElement("src-title-info");
                xmlNewSrcTitleInfo = srcTitleInfo.Store(document, xmlNewSrcTitleInfo);
                this.descriptionNode.ReplaceChild(xmlNewSrcTitleInfo, srcTitleInfoNode);
                srcTitleInfoNode = xmlNewSrcTitleInfo;
            }

            XmlElement xmlNewDocumentInfo = document.CreateElement("document-info");
            xmlNewDocumentInfo = documentInfo.Store(document, xmlNewDocumentInfo);
            if (documentInfoNode == null)
            {
                if (srcTitleInfoNode == null)
                {
                    this.descriptionNode.InsertAfter(xmlNewDocumentInfo, titleInfoNode);
                }
                else
                {
                    this.descriptionNode.InsertAfter(xmlNewDocumentInfo, srcTitleInfoNode);
                }
            }
            else
            {
                this.descriptionNode.ReplaceChild(xmlNewDocumentInfo, documentInfoNode);
            }

            if (publishInfo != null)
            {
                XmlElement xmlNewPublishInfo = document.CreateElement("publish-info");
                xmlNewPublishInfo = publishInfo.Store(document, xmlNewPublishInfo);

                if (xmlNewPublishInfo != null)
                {
                    this.descriptionNode.ReplaceChild(xmlNewPublishInfo, publishInfoNode);
                }
                else
                {
                    this.descriptionNode.RemoveChild(publishInfoNode);
                }
            }

            foreach (CustomInfoNode customInfoNode in customInfos)
            {
                XmlElement element = document.CreateElement("custom-info");
                element = customInfoNode.Store(document, element);

                if (element != null)
                {
                    this.descriptionNode.ReplaceChild(customInfoNode.XmlNode, element);
                }
                else
                {
                    this.descriptionNode.RemoveChild(customInfoNode.XmlNode);
                }
            }
        }
    }
}
