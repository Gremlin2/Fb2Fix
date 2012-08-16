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
using System.Xml;

namespace FB2Fix
{
    internal class Fb2SchemaResolver : XmlUrlResolver
    {
        private readonly string xsdSchemaLoacation;

        public Fb2SchemaResolver(string xsdSchemaLoacation)
        {
            if (xsdSchemaLoacation == null)
            {
                throw new ArgumentNullException("xsdSchemaLoacation");
            }

            this.xsdSchemaLoacation = xsdSchemaLoacation;
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri == null)
            {
                throw new ArgumentNullException("absoluteUri");
            }

            if(absoluteUri.IsFile)
            {
                string xsdSchemaFileName = Path.Combine(this.xsdSchemaLoacation, Path.GetFileName(absoluteUri.AbsolutePath));

                if (!String.IsNullOrEmpty(xsdSchemaFileName) && File.Exists(xsdSchemaFileName))
                {
                    return File.Open(xsdSchemaFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
            }

            return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }
    }
}