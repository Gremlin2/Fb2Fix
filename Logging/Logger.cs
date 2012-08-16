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

namespace FB2Fix.Logging
{
    internal static class Logger
    {
        private static readonly object sync = new object();
        private static volatile TraceSource writer;

        private static TraceSource Writer
        {
            get
            {
                if (writer == null)
                {
                    lock (sync)
                    {
                        if (writer == null)
                        {
                            writer = new TraceSource("Fb2Fix.exe", SourceLevels.Verbose);
                            writer.Switch.Level = SourceLevels.Verbose;
                        }
                    }
                }

                return writer;
            }
        }

        public static TraceListenerCollection Listeners
        {
            get
            {
                return Writer.Listeners;
            }
        }

        public static void Flush()
        {
            Writer.Flush();
        }

        [Conditional("TRACE")]
        public static void WriteLineIf(bool condition, TraceEventType eventType, string format, params object[] args)
        {
            if (condition)
            {
                Writer.TraceEvent(eventType, 0, format, args);
            }
        }

        [Conditional("TRACE")]
        public static void WriteLineIf(bool condition, TraceEventType eventType, string message)
        {
            if (condition)
            {
                Writer.TraceEvent(eventType, 0, message);
            }
        }

        [Conditional("TRACE")]
        public static void WriteLineIf(bool condition, TraceEventType eventType, Exception exp)
        {
            if (condition)
            {
                if (exp != null)
                {
                    foreach (string line in exp.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        Writer.TraceEvent(eventType, 0, line);
                    }
                }
            }
        }

        [Conditional("TRACE")]
        public static void WriteLine(TraceEventType eventType, string message)
        {
            Writer.TraceEvent(eventType, 0, message);
        }

        [Conditional("TRACE")]
        public static void WriteLine(TraceEventType eventType, string format, params object[] args)
        {
            Writer.TraceEvent(eventType, 0, format, args);
        }

        [Conditional("TRACE")]
        public static void WriteLine(TraceEventType eventType, Exception exp)
        {
            if (exp != null)
            {
                foreach (string line in exp.ToString().Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries))
                {
                    Writer.TraceEvent(eventType, 0, line);
                }
            }
        }

        [Conditional("TRACE")]
        public static void WriteInformation(string format, params object[] args)
        {
            WriteLine(TraceEventType.Information, format, args);
        }

        [Conditional("TRACE")]
        public static void WriteInformation(string message)
        {
            WriteLine(TraceEventType.Information, message);
        }

        [Conditional("TRACE")]
        public static void WriteInformation(Exception exp)
        {
            WriteLine(TraceEventType.Information, exp);
        }

        [Conditional("TRACE")]
        public static void WriteWarning(string format, params object[] args)
        {
            WriteLine(TraceEventType.Warning, format, args);
        }

        [Conditional("TRACE")]
        public static void WriteWarning(string message)
        {
            WriteLine(TraceEventType.Warning, message);
        }

        [Conditional("TRACE")]
        public static void WriteWarning(Exception exp)
        {
            WriteLine(TraceEventType.Warning, exp);
        }

        [Conditional("TRACE")]
        public static void WriteError(string format, params object[] args)
        {
            WriteLine(TraceEventType.Error, format, args);
        }

        [Conditional("TRACE")]
        public static void WriteError(string message)
        {
            WriteLine(TraceEventType.Error, message);
        }

        [Conditional("TRACE")]
        public static void WriteError(Exception exp)
        {
            WriteLine(TraceEventType.Error, exp);
        }
    }
}
