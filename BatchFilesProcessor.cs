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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;

using FB2Fix.Logging;
using FB2Fix.ObjectModel;
using FB2Fix.Sgml;

using ICSharpCode.SharpZipLib.Zip;

namespace FB2Fix
{
    internal class BatchFilesProcessor
    {
        private readonly Fb2FixArguments options;
        private Dictionary<string, bool> excludeList;

        private readonly string outputDirectoryRoot;
        private readonly string outputDirectoryGood;
        private readonly string outputDirectoryBad;
        private readonly string outputDirectoryNonValid;

        private FileNameProvider provider;

        private static readonly Regex bullets;
        private static readonly Regex invalidChars;
        private SgmlDtd fb2Dtd;
        private int? preferedCodepage;

        private XmlSchemaSet xsdSchema; 

        static BatchFilesProcessor()
        {
            bullets = new Regex("[\u0001-\u0008]");
            invalidChars = new Regex("[\u000b\u000c\u000e-\u001f]");
        }

        public BatchFilesProcessor(Fb2FixArguments options)
        {
            this.options = options;

            this.excludeList = new Dictionary<string, bool>(this.options.excludeList.Length, StringComparer.InvariantCultureIgnoreCase);

            foreach (string entry in this.options.excludeList)
            {
                this.excludeList.Add(entry, true);
            }

            if (String.IsNullOrEmpty(this.options.outputDirectory))
            {
                this.outputDirectoryRoot = Environment.CurrentDirectory;
            }
            else
            {
                this.outputDirectoryRoot = Path.GetFullPath(this.options.outputDirectory);
            }

            try
            {
                this.outputDirectoryGood = Path.Combine(this.outputDirectoryRoot, "Good");
                if (!Directory.Exists(this.outputDirectoryGood))
                {
                    Directory.CreateDirectory(this.outputDirectoryGood);
                }

                this.excludeList.Add(this.outputDirectoryGood, true);

                this.outputDirectoryBad = Path.Combine(this.outputDirectoryRoot, "Bad");
                if (!Directory.Exists(this.outputDirectoryBad))
                {
                    Directory.CreateDirectory(this.outputDirectoryBad);
                }

                this.excludeList.Add(this.outputDirectoryBad, true);

                if (options.validate)
                {
                    this.outputDirectoryNonValid = Path.Combine(this.outputDirectoryRoot, "NonValid");
                    if (!Directory.Exists(this.outputDirectoryNonValid))
                    {
                        Directory.CreateDirectory(this.outputDirectoryNonValid);
                    }

                    this.excludeList.Add(this.outputDirectoryNonValid, true);
                }
            }
            catch(IOException exp)
            {
                Logger.WriteLine(TraceEventType.Critical, exp.Message);
                Logger.WriteLine(TraceEventType.Verbose, exp);
                Logger.Flush();

                Environment.Exit(1);
            }
            catch (UnauthorizedAccessException exp)
            {
                Logger.WriteLine(TraceEventType.Critical, exp.Message);
                Logger.WriteLine(TraceEventType.Verbose, exp);
                Logger.Flush();

                Environment.Exit(1);
            }

            try
            {
                if (options.rename)
                {
                    options.pattern = options.pattern.Replace('/', Path.DirectorySeparatorChar);
                    options.pattern = options.pattern.Replace('\\', Path.DirectorySeparatorChar);

                    this.provider = new FileNameProvider(options.pattern);
                }

                if(!String.IsNullOrEmpty(options.encoding))
                {
                    Encoding prefered = Encoding.GetEncoding(options.encoding);
                    this.preferedCodepage = prefered.CodePage;
                }

                if(options.validate && !String.IsNullOrEmpty(options.xsdSchema))
                {
                    this.xsdSchema = new XmlSchemaSet();
                    this.xsdSchema.XmlResolver = new Fb2SchemaResolver(Path.GetDirectoryName(Path.GetFullPath(options.xsdSchema)));

                    using(FileStream stream = File.Open(options.xsdSchema, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        XmlSchema schema = XmlSchema.Read(stream, null);
                        this.xsdSchema.Add(schema);
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.WriteLine(TraceEventType.Critical, exp.Message);
                Logger.WriteLine(TraceEventType.Verbose, exp);
                Logger.Flush();

                Environment.Exit(1);
            }
        }

        private void ProcessElement(FictionBook fictionBook, XmlNode node, List<XmlElement> invalidNodes)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                ProcessElement(fictionBook, childNode, invalidNodes);
            }

            switch (node.NodeType)
            {
                case XmlNodeType.Text:
                    string text = node.InnerText;

                    text = bullets.Replace(text, new MatchEvaluator(delegate(Match match)
                    {
                        fictionBook.ModificationType = ModificationType.Text;
                        return "-"; 
                    }));

                    text = invalidChars.Replace(text, new MatchEvaluator(delegate(Match match)
                    {
                        fictionBook.ModificationType = ModificationType.Text;
                        return " ";
                    }));

                    node.InnerText = text;

                    //node.InnerText = invalidChars.Replace(bullets.Replace(node.InnerText, "-"), " ");
                    break;

                case XmlNodeType.Element:
                    ElementDecl elementDecl = this.fb2Dtd.FindElement(node.LocalName);
                    if(elementDecl == null)
                    {
                        invalidNodes.Add(node as XmlElement);
                    }
                    break;
            }
        }

        private void PostProcessDocument(FictionBook fictionBook)
        {
            List<XmlElement> invalidNodes;

            if (fictionBook == null)
            {
                throw new ArgumentNullException("fictionBook");
            }

            XmlDocument document = fictionBook.Document;

            invalidNodes = new List<XmlElement>(64);
            ProcessElement(fictionBook, document.DocumentElement, invalidNodes);

            //foreach (XmlElement node in invalidNodes)
            //{
            //    if(node == null)
            //    {
            //        continue;
            //    }

            //    XmlElement parent = node.ParentNode as XmlElement;
            //    if (parent != null && parent.NodeType == XmlNodeType.Element)
            //    {
            //        XmlComment comment = parent.OwnerDocument.CreateComment(node.OuterXml);
            //        parent.ReplaceChild(comment, node);
            //    }
            //}

            XmlNodeList nodes = document.SelectNodes("//FictionBook/descendant::p");
            List<XmlElement> paragraphNodes = new List<XmlElement>(nodes.Count);

            foreach (XmlNode node in nodes)
            {
                XmlElement paragraph = node as XmlElement;
                if (paragraph != null)
                {
                    paragraphNodes.Add(paragraph);
                }
            }

            for (int index = paragraphNodes.Count - 1; index >= 0; index--)
            {
                XmlElement paragraphElement = paragraphNodes[index];
                XmlElement parentElement = paragraphElement.ParentNode as XmlElement;
                if (parentElement != null && String.Compare(parentElement.LocalName, "p") == 0)
                {
                    XmlElement precedingElement = parentElement.ParentNode as XmlElement;
                    if (precedingElement != null)
                    {
                        parentElement.RemoveChild(paragraphElement);
                        precedingElement.InsertAfter(paragraphElement, parentElement);
                        fictionBook.ModificationType = ModificationType.Body;
                    }
                }
            }
        }

        private FictionBook ReadFictionBook(TextReader stream)
        {
            SgmlReader reader = new SgmlReader();
            reader.InputStream = stream;

            if (this.fb2Dtd == null)
            {
                reader.SystemLiteral = options.dtdFile;
                this.fb2Dtd = reader.Dtd;
            }
            else
            {
                reader.Dtd = this.fb2Dtd;    
            }
            
            FictionBook fictionBook = ReadFictionBook(reader);
            
            if(reader.MarkupErrorsCount > 0)
            {
                fictionBook.ModificationType = ModificationType.Body;
            }

            return fictionBook;
        }

        private FictionBook ReadFictionBook(XmlReader reader)
        {
            XmlDocument document = new XmlDocument();
            document.Load(reader);

            FictionBook fictionBook = new FictionBook(document);

            if (fictionBook.DocumentStatus == Fb2FixStatus.Passed && this.options.force == false)
            {
                return fictionBook;
            }

            fictionBook.CheckDocumentHeader(options);

            PostProcessDocument(fictionBook);

            fictionBook.DocumentStatus = Fb2FixStatus.Passed;

            return fictionBook;
        }

        private void ChangeDocumentVersion(FictionBook fictionBook)
        {
            if((fictionBook.ModificationType & ModificationType.DocumentInfo) == ModificationType.DocumentInfo)
            {
                fictionBook.Version = 1.0f;
                return;
            }

            if (this.options.incversion)
            {
                float version = 0.0f;

                if ((fictionBook.ModificationType & ModificationType.Description) == ModificationType.Description)
                {
                    version = Math.Max(version, 0.01f);
                }

                if ((fictionBook.ModificationType & ModificationType.Body) == ModificationType.Body)
                {
                    version = Math.Max(version, 0.1f);
                }

                if ((fictionBook.ModificationType & ModificationType.Text) == ModificationType.Text)
                {
                    version = Math.Max(version, 0.5f);
                }

                fictionBook.Version += version;
            }
        }

        private string GetOutputFileName(string path, string filename, string extension)
        {
            string fullFilename;

            if (options.maxLength > 0)
            {
                fullFilename = Path.Combine(path, StringUtils.Truncate(filename, options.maxLength - extension.Length)) + extension;
            }
            else
            {
                fullFilename = Path.Combine(path, filename) + extension;
            }

            int fileIndex = 0;
            while (File.Exists(fullFilename))
            {
                fileIndex++;
                string suffix = fileIndex.ToString(CultureInfo.InvariantCulture);

                if (options.maxLength > 0)
                {
                    fullFilename = Path.Combine(path, StringUtils.Truncate(filename, options.maxLength - extension.Length - suffix.Length)) + suffix + extension;
                }
                else
                {
                    fullFilename = Path.Combine(path, filename) + suffix + extension;
                }
            }

            return fullFilename;
        }

        private string GetFilename(string directory, string filename, FictionBook fictionBook)
        {
            if(this.options.rename)
            {
                filename = this.provider.GetFileName(fictionBook);
            }

            if(this.options.translify)
            {
                filename = StringUtils.Translify(filename);
            }

            filename = StringUtils.Dirify(filename, this.options.strict);

            if (!String.IsNullOrEmpty(this.options.replaceChar))
            {
                filename = Regex.Replace(filename, @"(\s)", this.options.replaceChar);

                if (this.options.replaceChar.Length == 1)
                {
                    filename = StringUtils.Squeeze(filename, this.options.replaceChar[0]);
                }
            }

            if(this.options.uppercaseFilenames)
            {
                filename = filename.ToUpperInvariant();
            }
            else if(this.options.lowercaseFilenames)
            {
                filename = filename.ToLowerInvariant();
            }

            string separator = Path.DirectorySeparatorChar.ToString();
            while(filename.StartsWith(separator))
            {
                filename = filename.Substring(1);
            }

            filename = StringUtils.Squeeze(filename, Path.DirectorySeparatorChar);

            return Path.Combine(directory, filename);
        }

        private static void StreamCopy(Stream source, Stream destination)
        {
            if (source != null && destination != null)
            {
                byte[] buffer = new byte[4096];

                int readed;

                do
                {
                    readed = source.Read(buffer, 0, buffer.Length);

                    if (readed < 0)
                    {
                        throw new EndOfStreamException("Unexpected end of stream");
                    }

                    destination.Write(buffer, 0, readed);
                } while (readed > 0);
            }
        }

        private void SaveFictionBook(string directory, string filename, FictionBook fictionBook, Encoding encoding)
        {
            string outputFilename = String.Empty;
            XmlDocument document = fictionBook.Document;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            try
            {
                if (this.options.compress)
                {
                    outputFilename = GetOutputFileName(directory, filename, ".zip");

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (Fb2TextWriter writer = new Fb2TextWriter(memoryStream, encoding))
                        {
                            writer.IndentHeader = this.options.indentHeader;
                            writer.IndentBody = this.options.indentBody;

                            writer.WriteStartDocument();

                            document.WriteTo(writer);
                            writer.Flush();

                            memoryStream.Capacity = (int) memoryStream.Length;
                            memoryStream.Seek(0, SeekOrigin.Begin);

                            using (ZipFile file = ZipFile.Create(outputFilename))
                            {
                                file.UseZip64 = UseZip64.Off;

                                ZipEntry entry = file.EntryFactory.MakeFileEntry(filename + ".fb2", false);

                                entry.DateTime = fictionBook.ContainerDateTime;

                                file.BeginUpdate();
                                file.Add(new StreamDataSource(memoryStream), entry);
                                file.CommitUpdate();
                            }
                        }
                    }
                }
                else
                {
                    outputFilename = GetOutputFileName(directory, filename, ".fb2");

                    using (Fb2TextWriter writer = new Fb2TextWriter(outputFilename, encoding))
                    {
                        writer.IndentHeader = this.options.indentHeader;
                        writer.IndentBody = this.options.indentBody;

                        writer.WriteStartDocument();

                        document.WriteTo(writer);
                        writer.Flush();
                    }
                }

                if(!String.IsNullOrEmpty(outputFilename))
                {
                    DateTime dt = fictionBook.ContainerDateTime;

                    if (!dt.IsDaylightSavingTime())
                    {
                        dt = dt.AddHours(-1);
                    }

                    File.SetCreationTime(outputFilename, dt);
                    File.SetLastAccessTime(outputFilename, dt);
                    File.SetLastWriteTime(outputFilename, dt);
                }
            }
            catch (Exception)
            {
                if(!String.IsNullOrEmpty(outputFilename))
                {
                    if(File.Exists(outputFilename))
                    {
                        try
                        {
                            File.Delete(outputFilename);
                        }
                        catch (Exception exp)
                        {
                            Logger.WriteLine(TraceEventType.Verbose, exp);
                        }
                    }
                }
                throw;
            }
        }

        private void ProcessDocument(Stream stream, string filename, DateTime lastModifiedTime)
        {
            Encoding encoding = null;
            FictionBook document = null;

            Logger.WriteInformation("Processing fb2 document '{0}'.", filename);

            try
            {
                using (HtmlStream htmlStream = new HtmlStream(stream, Encoding.Default))
                {
                    encoding = htmlStream.Encoding;
                    document = ReadFictionBook(htmlStream);

                    ChangeDocumentVersion(document);

                    if(document.ModificationType == ModificationType.None)
                    {
                        document.ContainerDateTime = lastModifiedTime;
                    }
                }
            }
            catch (InvalidOperationException exp)
            {
                throw new InvalidFictionBookFormatException(exp.Message, exp);
            }
            catch (XmlException exp)
            {
                throw new InvalidFictionBookFormatException(exp.Message, exp);
            }

            try
            {
                if(encoding == null)
                {
                    throw new InvalidFictionBookFormatException("Can't detect a character encoding.");
                }

                long threshold = (long) (document.Document.InnerText.Length * 0.25);

                if(this.preferedCodepage != null)
                {
                    encoding = Encoding.GetEncoding((int) this.preferedCodepage, new EncoderCharEntityFallback(threshold), new DecoderExceptionFallback());
                }
                else if (encoding.IsSingleByte)
                {
                    encoding = Encoding.GetEncoding(encoding.CodePage, new EncoderCharEntityFallback(threshold), new DecoderExceptionFallback());
                }

                bool done = false;
                int retryCount = 0;

                do
                {
                    try
                    {
                        if (++retryCount > 2)
                        {
                            break;
                        }

                        if (encoding != null && document != null)
                        {
                            string outputFullPath = GetFilename(this.outputDirectoryGood, filename, document);
                            string outputDirectory = Path.GetDirectoryName(outputFullPath).Trim();
                            string outputFilename = Path.GetFileName(outputFullPath).Trim();

                            if(options.validate)
                            {
                                try
                                {
                                    XmlParserContext context = new XmlParserContext(null, null, "", XmlSpace.None);
                                    XmlTextReader nodeReader = new XmlTextReader(document.Document.InnerXml, XmlNodeType.Document, context);

                                    XmlReaderSettings settings = new XmlReaderSettings();
                                    settings.ValidationType = ValidationType.Schema;
                                    settings.Schemas = this.xsdSchema;

                                    XmlReader reader = XmlReader.Create(nodeReader, settings);

                                    // Parse the XML file.
                                    while (reader.Read()) ;
                                }
                                catch (XmlSchemaValidationException exp)
                                {
                                    Logger.WriteWarning(exp.Message);
                                    Logger.WriteLine(TraceEventType.Verbose, exp);

                                    outputDirectory = this.outputDirectoryNonValid;
                                }
                            }

                            SaveFictionBook(outputDirectory, outputFilename, document, encoding);
                        }

                        done = true;
                    }
                    catch (EncoderFallbackException exp)
                    {
                        if (encoding != null)
                        {
                            Logger.WriteLineIf(false, TraceEventType.Warning, filename);
                            Logger.WriteWarning("Invalid document encoding ({0}) detected, utf-8 is used instead.", encoding.WebName);
                            Logger.WriteLine(TraceEventType.Verbose, exp);
                        }

                        encoding = Encoding.UTF8;
                    }
                }
                while (!done);
            }
            catch (IOException exp)
            {
                Logger.WriteLine(TraceEventType.Critical, exp.Message);
                Logger.WriteLine(TraceEventType.Verbose, exp);
                Logger.Flush();

                Environment.Exit(1);
            }
            catch (UnauthorizedAccessException exp)
            {
                Logger.WriteError(exp.Message);
                Logger.WriteLine(TraceEventType.Verbose, exp);
            }
        }

        private void ProcessZipFile(Stream stream)
        {
            try
            {
                using (ZipFile archive = new ZipFile(stream))
                {
                    foreach (ZipEntry entry in archive)
                    {
                        if (!entry.IsFile || !entry.CanDecompress || entry.IsCrypted)
                        {
                            continue;
                        }
                        
                        string entryName = StringUtils.CleanupFileName(entry.Name);
                        string extension = Path.GetExtension(entryName);

                        if (String.Compare(extension, ".zip", true) == 0)
                        {
                            using (MemoryStream memoryStream = new MemoryStream((int) entry.Size))
                            {
                                using (Stream entryStream = archive.GetInputStream(entry))
                                {
                                    StreamCopy(entryStream, memoryStream);
                                    ProcessZipFile(memoryStream);
                                }
                            }
                        }
                        else if (String.Compare(extension, ".rar", true) == 0)
                        {
                            string tempFilename = Path.GetTempFileName();

                            try
                            {
                                using (FileStream fs = File.Open(tempFilename, FileMode.Create, FileAccess.Write, FileShare.Read))
                                {
                                    using (Stream entryStream = archive.GetInputStream(entry))
                                    {
                                        StreamCopy(entryStream, fs);
                                    }
                                }

                                ProcessRarFile(tempFilename);
                            }
                            finally
                            {
                                File.Delete(tempFilename);
                            }
                        }
                        else if (String.Compare(extension, ".fb2", true) == 0)
                        {
                            using (Stream entryStream = archive.GetInputStream(entry))
                            {
                                string entryFileName = Path.GetFileNameWithoutExtension(entryName);

                                try
                                {
                                    ProcessDocument(entryStream, entryFileName, entry.DateTime);  
                                }
                                catch (Exception exp)
                                {
                                    Logger.WriteError(exp.Message);
                                    Logger.WriteLine(TraceEventType.Verbose, exp);

                                    try
                                    {
                                        string outputFileName = GetOutputFileName(this.outputDirectoryBad, StringUtils.Dirify(entryFileName), ".fb2");

                                        using (Stream inputStream = archive.GetInputStream(entry))
                                        {
                                            using (FileStream fs = File.Open(outputFileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                                            {
                                                StreamCopy(inputStream, fs);
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.WriteError(e.Message);
                                        Logger.WriteLine(TraceEventType.Verbose, e);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(ZipException exp)
            {
                Logger.WriteError(exp.Message);
                Logger.WriteLine(TraceEventType.Verbose, exp);
            }
            catch (Exception exp)
            {
                Logger.WriteError(exp.Message);
                Logger.WriteLine(TraceEventType.Verbose, exp);
            }
        }

        private void ProcessRarFile(string filename)
        {
            try
            {
                using (Unrar archive = new Unrar(filename))
                {
                    archive.Open();
                    archive.NewVolume += delegate(object sender, NewVolumeEventArgs e)
                    {
                        this.excludeList.Add(e.VolumeName, true);
                        e.ContinueOperation = true;
                    };

                    while (archive.ReadHeader())
                    {
                        if (archive.CurrentFile != null)
                        {
                            if (archive.CurrentFile.ContinuedFromPrevious || archive.CurrentFile.IsDirectory)
                            {
                                continue;
                            }
                            
                            string entryName = StringUtils.CleanupFileName(archive.CurrentFile.FileName);
                            string extension = Path.GetExtension(entryName);

                            if (String.Compare(extension, ".zip", true) == 0)
                            {
                                string tempFile = Path.GetTempFileName();

                                try
                                {
                                    archive.Extract(tempFile);
                                    using (FileStream fileStream = File.Open(tempFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                                    {
                                        ProcessZipFile(fileStream);
                                    }
                                }
                                finally
                                {
                                    File.Delete(tempFile);
                                }
                            }
                            else if (String.Compare(extension, ".rar", true) == 0)
                            {
                                string tempFile = Path.GetTempFileName();

                                try
                                {
                                    archive.Extract(tempFile);
                                    ProcessRarFile(tempFile);
                                }
                                finally
                                {
                                    File.Delete(tempFile);
                                }
                            }
                            else if (String.Compare(extension, ".fb2", true) == 0)
                            {
                                string tempFilename = Path.GetTempFileName();

                                try
                                {
                                    archive.Extract(tempFilename);

                                    using (FileStream fileStream = File.Open(tempFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                                    {
                                        try
                                        {
                                            ProcessDocument(fileStream, Path.GetFileNameWithoutExtension(entryName), archive.CurrentFile.FileTime);
                                        }
                                        catch (Exception exp)
                                        {
                                            Logger.WriteError(exp.Message);
                                            Logger.WriteLine(TraceEventType.Verbose, exp);

                                            try
                                            {
                                                string outputFilename = GetOutputFileName(this.outputDirectoryBad, Path.GetFileNameWithoutExtension(entryName), ".fb2");
                                                File.Copy(filename, outputFilename);
                                            }
                                            catch (Exception e)
                                            {
                                                Logger.WriteError(e.Message);
                                                Logger.WriteLine(TraceEventType.Verbose, e);
                                            }
                                        }
                                    }
                                }
                                finally
                                {
                                    File.Delete(tempFilename);
                                }
                            }
                            else
                            {
                                archive.Skip();
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.WriteError(exp.Message);
                Logger.WriteLine(TraceEventType.Verbose, exp);
            }
        }

        private void ProcessFile(string filename)
        {
            string extension = Path.GetExtension(filename);

            if (String.Compare(extension, ".zip", true) == 0)
            {
                Logger.WriteInformation("Processing archive file '{0}'.", filename);

                using (FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    ProcessZipFile(fileStream);
                }
            }
            else if (String.Compare(extension, ".rar", true) == 0)
            {
                Logger.WriteInformation("Processing archive file '{0}'.", filename);

                ProcessRarFile(filename);
            }
            else if (String.Compare(extension, ".fb2", true) == 0)
            {
                using (FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    try
                    {
                        DateTime now = DateTime.Now;
                        TimeSpan localOffset = now - now.ToUniversalTime();

                        ProcessDocument(fileStream, Path.GetFileNameWithoutExtension(filename), File.GetLastWriteTimeUtc(filename) + localOffset);
                    }
                    catch (Exception exp)
                    {
                        Logger.WriteError(exp.Message);
                        Logger.WriteLine(TraceEventType.Verbose, exp);

                        try
                        {
                            string outputFilename = GetOutputFileName(this.outputDirectoryBad, Path.GetFileNameWithoutExtension(filename), ".fb2");
                            File.Copy(filename, outputFilename);
                        }
                        catch (Exception e)
                        {
                            Logger.WriteError(e.Message);
                            Logger.WriteLine(TraceEventType.Verbose, e);
                        }
                    }
                }
            }
        }


        private void ProcessDirectory(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                if (!this.excludeList.ContainsKey(fileName))
                {
                    ProcessFile(fileName);
                }
            }

            if (this.options.recurse)
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subdirectory in subdirectoryEntries)
                {
                    if (!this.excludeList.ContainsKey(subdirectory))
                    {
                        ProcessDirectory(subdirectory);
                    }
                }
            }
        }

        public void Process(IEnumerable<string> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException("entries");
            }

            foreach (string path in entries)
            {
                if (Directory.Exists(path))
                {
                    ProcessDirectory(path);
                }
                else if (File.Exists(path))
                {
                    ProcessFile(path);
                }
                else
                {
                    Logger.WriteError("{0} is not a valid file or directory.", path);
                }
            }
        }
    }
}
