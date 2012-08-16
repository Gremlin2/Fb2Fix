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
using System.IO;
using System.Text;
using System.Diagnostics;

namespace FB2Fix.Logging
{
    internal class CustomTraceListener : TextWriterTraceListener
    {
        public CustomTraceListener(string fileName, string name) : base(fileName, name)
        {
        }

        public CustomTraceListener(string fileName) : base(fileName)
        {
        }

        public CustomTraceListener(TextWriter writer, string name) : base(writer, name)
        {
        }

        public CustomTraceListener(TextWriter writer) : base(writer)
        {
        }

        public CustomTraceListener(Stream stream, string name) : base(stream, name)
        {
        }

        public CustomTraceListener(Stream stream) : base(stream)
        {
        }

        public CustomTraceListener()
        {
        }

        public override void Write(string message)
        {
            base.Write(String.Format("{0}:{1}: {2}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fffff"), Enum.GetName(typeof(TraceEventType), TraceEventType.Verbose).PadLeft(11), message));
        }

        public override void WriteLine(string message)
        {
            base.WriteLine(String.Format("{0}:{1}: {2}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fffff"), Enum.GetName(typeof(TraceEventType), TraceEventType.Verbose).PadLeft(11), message));
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
            {
                return;
            }

            string message = String.Empty;
            if(data != null)
            {
                message = data.ToString();
            }

            DateTime eventDateTime = DateTime.Now;
            if(eventCache != null)
            {
                eventDateTime = eventCache.DateTime; 
            }

            base.WriteLine(String.Format("{0}:{1}: {2}", eventDateTime.ToString("dd.MM.yyyy HH:mm:ss.fffff"), Enum.GetName(typeof(TraceEventType), eventType).PadLeft(11), message));
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, data))
            {
                return;
            }

            StringBuilder message = new StringBuilder();
            if(data != null)
            {
                for(int index = 0; index < data.Length; index++)
                {
                    if(index != 0)
                    {
                        message.Append(", ");
                    }

                    if(data[index] != null)
                    {
                        message.Append(data[index].ToString());
                    }
                }
            }

            DateTime eventDateTime = DateTime.Now;
            if(eventCache != null)
            {
                eventDateTime = eventCache.DateTime; 
            }

            base.WriteLine(String.Format("{0}:{1}: {2}", eventDateTime.ToString("dd.MM.yyyy HH:mm:ss.fffff"), Enum.GetName(typeof(TraceEventType), eventType).PadLeft(11), message));
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            if(Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null))
            {
                return;
            }

            DateTime eventDateTime = DateTime.Now;
            if(eventCache != null)
            {
                eventDateTime = eventCache.DateTime; 
            }

            base.WriteLine(String.Format("{0}:{1}: {2}", eventDateTime.ToString("dd.MM.yyyy HH:mm:ss.fffff"), Enum.GetName(typeof(TraceEventType), eventType).PadLeft(11), message));
        }


        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null))
            {
                return;
            }
            
            string message;

            if(args == null)
            {
                message = format;
            }
            else
            {
                message = String.Format(format, args);
            }

            DateTime eventDateTime = DateTime.Now;
            if(eventCache != null)
            {
                eventDateTime = eventCache.DateTime; 
            }

            base.WriteLine(String.Format("{0}:{1}: {2}", eventDateTime.ToString("dd.MM.yyyy HH:mm:ss.fffff"), Enum.GetName(typeof(TraceEventType), eventType).PadLeft(11), message));
        }
    }
}