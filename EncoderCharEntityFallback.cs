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
using System.Text;

namespace FB2Fix
{
    internal sealed class EncoderCharEntityFallback : EncoderFallback
    {
        private readonly long threshold;

        public EncoderCharEntityFallback(long threshold)
        {
            this.threshold = threshold;
        }

        public override EncoderFallbackBuffer CreateFallbackBuffer()
        {
            return new EncoderCharEntityFallbackBuffer(this);
        }

        public long Threshold
        {
            get
            {
                return this.threshold;
            }
        }

        public override int MaxCharCount
        {
            get
            {
                return 9;
            }
        }
    }

    internal sealed class EncoderCharEntityFallbackBuffer : EncoderFallbackBuffer
    {
        private int fallbackCount;
        private int fallbackIndex;

        private long unknownCharCount;
        private long threshold;
        private bool valid;
        
        private StringBuilder buffer;

        public EncoderCharEntityFallbackBuffer(EncoderCharEntityFallback fallback)
        {
            if (fallback == null)
            {
                throw new ArgumentNullException("fallback");
            }

            this.fallbackCount = -1;
            this.fallbackIndex = -1;

            this.unknownCharCount = 0;
            this.threshold = fallback.Threshold;

            this.buffer = new StringBuilder(10);
            this.valid = true;
        }

        public override bool Fallback(char charUnknown, int index)
        {
            if (this.fallbackCount > 0)
            {
                throw new ArgumentException();
            }

            this.unknownCharCount++;

            if (this.unknownCharCount > this.threshold)
            {
                this.fallbackIndex = -1;
                this.fallbackCount = 0;
                this.valid = false;

                throw new EncoderFallbackException();
            }

            if (this.buffer.Length > 0)
            {
                this.buffer.Remove(0, this.buffer.Length);
            }

            this.buffer.Append("&#x");
            this.buffer.Append(((int) charUnknown).ToString("X", CultureInfo.InvariantCulture));
            this.buffer.Append(";");

            this.fallbackIndex = -1;
            this.fallbackCount = this.buffer.Length;

            return true;
        }

        public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
        {
            if(!Char.IsHighSurrogate(charUnknownHigh))
            {
                throw new ArgumentOutOfRangeException("charUnknownHigh");
            }

            if(!Char.IsLowSurrogate(charUnknownLow))
            {
                throw new ArgumentOutOfRangeException("charUnknownLow");
            }

            if(this.fallbackCount > 0)
            {
                throw new ArgumentException();
            }

            this.unknownCharCount++;

            if(this.unknownCharCount > this.threshold)
            {
                this.fallbackIndex = -1;
                this.fallbackCount = 0;
                this.valid = false;

                throw new EncoderFallbackException();
            }

            int charCode = (charUnknownLow - 0xDC00) | (((charUnknownHigh - 0xd800) << 10) + 0x10000);

            if (this.buffer.Length > 0)
            {
                this.buffer.Remove(0, this.buffer.Length);
            }

            this.buffer.Append("&#x");
            this.buffer.Append(charCode.ToString("X", CultureInfo.InvariantCulture));
            this.buffer.Append(";");

            this.fallbackIndex = -1;
            this.fallbackCount = this.buffer.Length;

            return true;
        }

        public override char GetNextChar()
        {
            this.fallbackCount--;
            this.fallbackIndex++;

            if (this.valid)
            {
                if (this.fallbackCount < 0)
                {
                    return '\0';
                }

                return this.buffer[fallbackIndex];
            }
            else
            {
                return '\0';
            }
        }

        public override bool MovePrevious()
        {
            if (this.valid)
            {
                if ((this.fallbackCount >= -1) && (this.fallbackIndex >= 0))
                {
                    this.fallbackIndex--;
                    this.fallbackCount++;

                    return true;
                }
            }            

            return false;
        }

        public override int Remaining
        {
            get
            {
                if (this.valid && this.fallbackCount >= 0)
                {
                    return this.fallbackCount;
                }

                return 0;
            }
        }
    }
}
