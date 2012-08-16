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
    [Flags]
    internal enum TraceEventTypes
    {
        None = 0,
        Critical = 1,
        Error = 2,
        Warning = 4,
        Information = 8,
        Verbose = 16,
        Start = 256,
        Stop = 512,
        Suspend = 1024,
        Resume = 2048,
        Transfer = 4096
    }

    public enum LogLevel
    {
        Off = 0,
        Critical = 1,
        Error = 3,
        Warning = 7,
        Information = 15,
        Verbose = 31
    }

    internal class TraceEventTypeFilter : TraceFilter 
    {
        private readonly TraceEventTypes eventTypes;

        public TraceEventTypeFilter(TraceEventTypes eventTypes)
        {
            this.eventTypes = eventTypes;
        }

        public TraceEventTypeFilter(TraceEventTypes eventTypes, LogLevel logLevel)
        {
            this.eventTypes = (TraceEventTypes) ((int) eventTypes & (int) logLevel);
        }
        
        public TraceEventTypes EventTypes
        {
            get
            {
                return this.eventTypes;
            }
        }

        public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
        {
            return ((int) eventType & (int)this.eventTypes) != 0;
        }
    }
}
