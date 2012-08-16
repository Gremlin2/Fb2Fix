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
using System.Runtime.Serialization;

namespace FB2Fix
{
    [Serializable]
    public class InvalidFictionBookFormatException : Exception
    {
        public InvalidFictionBookFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public InvalidFictionBookFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidFictionBookFormatException(string message) : base(message)
        {
        }

        public InvalidFictionBookFormatException()
        {
        }
    }
}
