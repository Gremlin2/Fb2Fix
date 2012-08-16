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
using System.Xml;

namespace FB2Fix.ObjectModel
{
	public class AuthorInfoNode : DocumentNode
	{
		private string firstName;
		private string middleName;
		private string lastName;
		private string nickName;
		private List<string> homepage;
		private List<string> email;
		private string id;
		
		public AuthorInfoNode()
		{
            this.homepage = new List<string>();
            this.email = new List<string>();
		}
		
		public string FirstName 
		{
			get 
			{ 
				return firstName; 
			}
			set 
			{ 
				firstName = value; 
			}
		}
		
		public string MiddleName 
		{
			get 
			{ 
				return middleName; 
			}
			set 
			{ 
				middleName = value; 
			}
		}
		
		public string LastName 
		{
			get 
			{ 
				return lastName; 
			}
			set 
			{ 
				lastName = value; 
			}
		}
		
		public string NickName 
		{
			get 
			{ 
				return nickName; 
			}
			set 
			{ 
				nickName = value; 
			}
		}
		
		public string Id 
		{
			get 
			{ 
				return id; 
			}
			set 
			{ 
				id = value; 
			}
		}

	    public List<string> Homepage
	    {
	        get
	        {
	            return this.homepage;
	        }
	    }

	    public List<string> Email
	    {
	        get
	        {
	            return this.email;
	        }
	    }

	    public override void Load(XmlElement parentNode)
		{
			if(parentNode == null)
			{
				throw new ArgumentNullException("parentNode");
			}
			
			this.firstName = LoadRequiredElement(parentNode, "./first-name");
            this.middleName = LoadElement(parentNode, "./middle-name");
            this.lastName = LoadRequiredElement(parentNode, "./last-name");
            this.nickName = LoadRequiredElement(parentNode, "./nickname");
			this.id = LoadElement(parentNode, "./id");
			this.homepage = LoadElementsList(parentNode, "./home-page");
			this.email = LoadElementsList(parentNode, "./email");
		}

	    public override XmlElement Store(XmlDocument document, XmlElement element)
	    {
	        XmlElement childElement;

	        if (document == null)
	        {
	            throw new ArgumentNullException("document");
	        }
	        
            if (element == null)
	        {
	            throw new ArgumentNullException("element");
	        }

            if((childElement = StoreRequiredElement(document, "first-name", this.firstName)) != null)
	        {
	            element.AppendChild(childElement);
	        }

            if ((childElement = StoreElement(document, "middle-name", this.middleName)) != null)
            {
                element.AppendChild(childElement);
            }

            if ((childElement = StoreRequiredElement(document, "last-name", this.lastName)) != null)
            {
                element.AppendChild(childElement);
            }

            if ((childElement = StoreRequiredElement(document, "nickname", this.nickName)) != null)
            {
                element.AppendChild(childElement);
            }

            if ((childElement = StoreElement(document, "id", this.id)) != null)
            {
                element.AppendChild(childElement);
            }

	        foreach (string value in this.homepage)
	        {
                if ((childElement = StoreElement(document, "home-page", value)) != null)
                {
                    element.AppendChild(childElement);
                }
            }

            foreach (string value in this.email)
            {
                if ((childElement = StoreElement(document, "email", value)) != null)
                {
                    element.AppendChild(childElement);
                }
            }

            if(element.ChildNodes.Count == 0)
            {
                return null;
            }

	        return element;
	    }

	    public override bool IsEmpty()
	    {
	        return false;
	    }
	}
}
