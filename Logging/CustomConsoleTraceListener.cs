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
using System.Text;

namespace FB2Fix.Logging
{
    internal class CustomConsoleTraceListener : TextWriterTraceListener
    {
        public override void Write(string message)
        {
            Console.Out.Write(message);
        }

        public override void WriteLine(string message)
        {
            Console.Out.WriteLine(message);
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
            {
                return;
            }

            string message = String.Empty;
            if (data != null)
            {
                message = data.ToString();
            }

            switch(eventType)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                case TraceEventType.Warning:
                    Console.Error.WriteLine(String.Format("{0}: {1}", Enum.GetName(typeof(TraceEventType), eventType), message));
                    break;

                default:
                    Console.Out.WriteLine(message);
                    break;
            }
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, data))
            {
                return;
            }

            StringBuilder message = new StringBuilder();
            if (data != null)
            {
                for (int index = 0; index < data.Length; index++)
                {
                    if (index != 0)
                    {
                        message.Append(", ");
                    }

                    if (data[index] != null)
                    {
                        message.Append(data[index].ToString());
                    }
                }
            }

            switch(eventType)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                case TraceEventType.Warning:
                    Console.Error.WriteLine(String.Format("{0}: {1}", Enum.GetName(typeof(TraceEventType), eventType), message));
                    break;

                default:
                    Console.Out.WriteLine(message);
                    break;
            }
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null))
            {
                return;
            }

            switch (eventType)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                case TraceEventType.Warning:
                    Console.Error.WriteLine(String.Format("{0}: {1}", Enum.GetName(typeof(TraceEventType), eventType), message));
                    break;

                default:
                    Console.Out.WriteLine(message);
                    break;
            }
        }


        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null))
            {
                return;
            }

            string message;

            if (args == null)
            {
                message = format;
            }
            else
            {
                message = String.Format(format, args);
            }

            switch (eventType)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                case TraceEventType.Warning:
                    Console.Error.WriteLine(String.Format("{0}: {1}", Enum.GetName(typeof(TraceEventType), eventType), message));
                    break;

                default:
                    Console.Out.WriteLine(message);
                    break;
            }
        }
    }
}
