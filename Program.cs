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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using FB2Fix.Logging;
using FB2Fix.CommandLine;

using ICSharpCode.SharpZipLib.Zip;

namespace FB2Fix
{
    public class Fb2FixArguments
    {
        [Argument(ArgumentType.AtMostOnce, HelpText = "Compress output files automatically.", LongName = "compress", ShortName = "c", DefaultValue = true)]
        public bool compress;

        [Argument(ArgumentType.AtMostOnce, HelpText = "This option is obsolete. Use /indentheader and /indentbody options instead.", LongName = "indent", ShortName = "")]
        public bool indent;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Indent output document headers.", LongName = "indentheader", ShortName = "", DefaultValue = true)]
        public bool indentHeader;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Indent output document body.", LongName = "indentbody", ShortName = "", DefaultValue = false)]
        public bool indentBody;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Auto increment a minor version number in output document.", LongName = "incversion", ShortName = "", DefaultValue = true)]
        public bool incversion;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Replace the current document id with a new one.", LongName = "replaceid", ShortName = "", DefaultValue = false)]
        public bool regenerateId;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Specify other SGML dtd file to use.", LongName = "dtd", ShortName = "", DefaultValue = "fb2.dtd")]
        public string dtdFile;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Try to use this encoding rather than the encoding specified in the XML document.", ShortName = "")]
        public string encoding;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Map genres from fb2.0 to fb2.1 format.", LongName = "mapgenres", ShortName = "", DefaultValue = true)]
        public bool mapGenres;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Specify the genres configuration file.", LongName = "genres", ShortName = "", DefaultValue = "genrestransfer.xml")]
        public string genres;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Validate documents against XSD schema.", LongName = "validate", ShortName = "", DefaultValue = false)]
        public bool validate;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Specify a xsd schema for the parser.", LongName = "xsd", ShortName = "", DefaultValue = "FictionBook.xsd")]
        public string xsdSchema;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Automatically rename output files according to pattern given in /pattern option.", LongName = "rename", ShortName = "", DefaultValue = false)]
        public bool rename;
       
        [Argument(ArgumentType.AtMostOnce, HelpText = "Specify folder/files naming pattern.", LongName = "pattern", ShortName = "", DefaultValue = @"[*NLA*\]*NL*[ *NM*] *NF*[ *NN*]\*NL* *BN*[ (*SN* - *SII*)]")]
        public string pattern;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Translify output file names.", LongName = "translify", ShortName = "", DefaultValue = true)]
        public bool translify;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Convert output file names to upper case.", LongName = "upper", ShortName = "", DefaultValue = false)]
        public bool uppercaseFilenames;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Convert output file names to lower case.", LongName = "lower", ShortName = "", DefaultValue = false)]
        public bool lowercaseFilenames;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Specify character to replace each whitespace in an output filename.", LongName = "replacechar", ShortName = "", DefaultValue = "_")]
        public string replaceChar;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Specify the maximum a file name length.", LongName = "maxlength", ShortName = "", DefaultValue = -1)]
        public int maxLength;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Use a strict naming conventions for output files.", LongName = "strict", ShortName = "", DefaultValue = false)]
        public bool strict;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Set error log file name.", LongName = "logfile", ShortName = "")]
        public string logfile;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = LogLevel.Information)]
        public LogLevel loglevel;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Search subdirectories for files to process.", LongName = "recurse", ShortName = "r", DefaultValue = true)]
        public bool recurse;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Force re-process already processed files.", ShortName = "", LongName = "force", DefaultValue = false)]
        public bool force;

        [Argument(ArgumentType.AtMostOnce, HelpText = "Set the output directory for processed files.", LongName = "output", ShortName = "o")]
        public string outputDirectory;

        [Argument(ArgumentType.MultipleUnique, HelpText = "Exclude file or directory from the process.", LongName = "exclude", ShortName = "e")]
        public string[] excludeList;

        //[Argument(ArgumentType.AtMostOnce, HelpText = "Read data from the stdin and write result to stdout.", ShortName = "", LongName = "filter", DefaultValue = false)]
        //public bool filter;

        //[Argument(ArgumentType.AtMostOnce, HelpText = "Instructs the fb2fix not to auto include fb2fix.rsp file.", LongName = "noconfig", ShortName = "", DefaultValue = false)]
        //public bool noconfig;

        [DefaultArgument(ArgumentType.MultipleUnique, HelpText = "Input files or directories to process.", LongName = "file")]
        public string[] files;
    }

    internal class Program
    {
        private static void PrintUsageInfo()
        {
            Console.WriteLine("Fb2Fix Version 1.0.10 Copyright 2007-2010 Gremlin");
            Console.WriteLine("Usage: Fb2Fix.exe [options|@optionsfile] <file ...>");
            Console.WriteLine();
            Console.Write(CommandLineParser.ArgumentsUsage(typeof(Fb2FixArguments)));
        }

        private static void Main(string[] args)
        {
            CustomTraceListener listener = null;

            Fb2FixArguments arguments = new Fb2FixArguments();

            ZipConstants.DefaultCodePage = 866;

            if (CommandLineParser.ParseArguments(args, arguments))
            {
                string workingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                //if(File.Exists(Path.Combine(workingDirectory, "fb2fix.rsp")) && !arguments.noconfig)
                //{
                //    arguments = new Fb2FixArguments();
                //    string[] argList = new string[args.Length + 1];

                //    argList[0] = "@" + Path.Combine(workingDirectory, "fb2fix.rsp");
                //    args.CopyTo(argList, 1);

                //    if(!CommandLineParser.ParseArguments(argList, arguments))
                //    {
                //        Console.WriteLine();
                //        PrintUsageInfo();
                //        Environment.Exit(1);
                //    }
                //}

                if(arguments.files.Length == 0 && arguments.filter == false)
                {
                    PrintUsageInfo();
                    Environment.Exit(1);
                }

                if(String.IsNullOrEmpty(arguments.dtdFile) || String.Compare(arguments.dtdFile, "fb2.dtd") == 0)
                {
                    arguments.dtdFile = Path.Combine(workingDirectory, "fb2.dtd");
                }

                if(String.IsNullOrEmpty(arguments.genres) || String.Compare(arguments.genres, "genrestransfer.xml") == 0)
                {
                    arguments.genres = Path.Combine(workingDirectory, "genrestransfer.xml");
                }

                if (String.IsNullOrEmpty(arguments.xsdSchema) || String.Compare(arguments.xsdSchema, "FictionBook.xsd") == 0)
                {
                    arguments.xsdSchema = Path.Combine(workingDirectory, "FictionBook.xsd");
                }

                CustomConsoleTraceListener infoConsoleListener = new CustomConsoleTraceListener();
                infoConsoleListener.Filter = new TraceEventTypeFilter(TraceEventTypes.Information, arguments.loglevel);
                Logger.Listeners.Add(infoConsoleListener);

                CustomConsoleTraceListener errorConsoleListener = new CustomConsoleTraceListener();
                errorConsoleListener.Filter = new TraceEventTypeFilter(TraceEventTypes.Critical | TraceEventTypes.Error | TraceEventTypes.Warning, arguments.loglevel);
                Logger.Listeners.Add(errorConsoleListener);

                if (!String.IsNullOrEmpty(arguments.logfile))
                {
                    listener = new CustomTraceListener(arguments.logfile);
                    listener.Filter = new EventTypeFilter((SourceLevels) arguments.loglevel);
                    Logger.Listeners.Add(listener);
                }

                try
                {
                    using (FileStream fileStream = File.Open(arguments.genres, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (XmlReader reader = XmlReader.Create(fileStream))
                        {
                            GenreTable.ReadGenreList(reader);
                        }
                    }

                    BatchFilesProcessor batch = new BatchFilesProcessor(arguments);
                    batch.Process(arguments.files);
                }
                catch (Exception exp)
                {
                    Logger.WriteLine(TraceEventType.Critical, exp.Message);
                    Logger.WriteLine(TraceEventType.Verbose, exp.ToString());
                }
                finally
                {
                    Logger.Flush();

                    if (listener != null)
                    {
                        listener.Flush();
                        listener.Close();
                    }
                }
            }
            else
            {
                Console.WriteLine();
                PrintUsageInfo();
                Environment.Exit(1);
            }
        }
    }
}