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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FB2Fix.ObjectModel;

namespace FB2Fix
{
    internal abstract class FileNamePatternElement
    {
        private readonly string template;

        private FileNamePatternElement()
        {
        }

        protected FileNamePatternElement(string template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            this.template = template;
        }

        public string Template
        {
            get
            {
                return this.template;
            }
        }

        public static string CleanupValue(string value)
        {
            if(String.IsNullOrEmpty(value))
            {
                return value;
            }

            value = value.Trim();
            value = value.Replace(Path.DirectorySeparatorChar.ToString(), "");

            return value;
        }

        public abstract void Parse();
        public abstract string ApplyPattern(string pattern, FictionBook fictionBook);
        public abstract bool HasValue(FictionBook fictionBook);
    }

    internal sealed class AuthorFirstNameElement : FileNamePatternElement
    {
        private readonly Regex replacePattern;

        public AuthorFirstNameElement(string template) : base(template)
        {
            this.replacePattern = new Regex(Regex.Escape(template));
        }

        public override void Parse()
        {
        }

        public override string ApplyPattern(string pattern, FictionBook fictionBook)
        {
            string value = String.Empty;

            if (fictionBook.TitleInfo.Authors != null && fictionBook.TitleInfo.Authors.Count > 0)
            {
                AuthorInfoNode autorInfo = fictionBook.TitleInfo.Authors[0];
                if (!String.IsNullOrEmpty(autorInfo.FirstName))
                {
                    value = CleanupValue(autorInfo.FirstName);
                }
            }

            return replacePattern.Replace(pattern, value);
        }

        public override bool HasValue(FictionBook fictionBook)
        {
            if (fictionBook.TitleInfo.Authors != null && fictionBook.TitleInfo.Authors.Count > 0)
            {
                AuthorInfoNode autorInfo = fictionBook.TitleInfo.Authors[0];
                if(!String.IsNullOrEmpty(autorInfo.FirstName))
                {
                    return true;
                }
            }

            return false;
        }
    }

    internal sealed class AuthorMiddleNameElement : FileNamePatternElement
    {
        private readonly Regex replacePattern;

        public AuthorMiddleNameElement(string template) : base(template)
        {
            this.replacePattern = new Regex(Regex.Escape(template));
        }

        public override void Parse()
        {
        }

        public override string ApplyPattern(string pattern, FictionBook fictionBook)
        {
            string value = String.Empty;

            if (fictionBook.TitleInfo.Authors != null && fictionBook.TitleInfo.Authors.Count > 0)
            {
                AuthorInfoNode autorInfo = fictionBook.TitleInfo.Authors[0];
                if (!String.IsNullOrEmpty(autorInfo.MiddleName))
                {
                    value = CleanupValue(autorInfo.MiddleName);
                }
            }

            return replacePattern.Replace(pattern, value);
        }

        public override bool HasValue(FictionBook fictionBook)
        {
            if (fictionBook.TitleInfo.Authors != null && fictionBook.TitleInfo.Authors.Count > 0)
            {
                AuthorInfoNode autorInfo = fictionBook.TitleInfo.Authors[0];
                if (!String.IsNullOrEmpty(autorInfo.MiddleName))
                {
                    return true;
                }
            }

            return false;
        }
    }

    internal sealed class AuthorLastNameElement : FileNamePatternElement
    {
        private readonly Regex replacePattern;

        public AuthorLastNameElement(string template) : base(template)
        {
            this.replacePattern = new Regex(Regex.Escape(template));
        }

        public override void Parse()
        {
        }

        public override string ApplyPattern(string pattern, FictionBook fictionBook)
        {
            string value = String.Empty;

            if (fictionBook.TitleInfo.Authors != null && fictionBook.TitleInfo.Authors.Count > 0)
            {
                AuthorInfoNode autorInfo = fictionBook.TitleInfo.Authors[0];
                if (!String.IsNullOrEmpty(autorInfo.LastName))
                {
                    value = CleanupValue(autorInfo.LastName);
                }
            }

            return replacePattern.Replace(pattern, value);
        }

        public override bool HasValue(FictionBook fictionBook)
        {
            if (fictionBook.TitleInfo.Authors != null && fictionBook.TitleInfo.Authors.Count > 0)
            {
                AuthorInfoNode autorInfo = fictionBook.TitleInfo.Authors[0];
                if (!String.IsNullOrEmpty(autorInfo.LastName))
                {
                    return true;
                }
            }

            return false;
        }
    }

    internal sealed class AuthorNickNameElement : FileNamePatternElement
    {
        private readonly Regex replacePattern;

        public AuthorNickNameElement(string template) : base(template)
        {
            this.replacePattern = new Regex(Regex.Escape(template));
        }

        public override void Parse()
        {
        }

        public override string ApplyPattern(string pattern, FictionBook fictionBook)
        {
            string value = String.Empty;

            if (fictionBook.TitleInfo.Authors != null && fictionBook.TitleInfo.Authors.Count > 0)
            {
                AuthorInfoNode autorInfo = fictionBook.TitleInfo.Authors[0];
                if (!String.IsNullOrEmpty(autorInfo.NickName))
                {
                    value = CleanupValue(autorInfo.NickName);
                }
            }

            return replacePattern.Replace(pattern, value);
        }

        public override bool HasValue(FictionBook fictionBook)
        {
            if (fictionBook.TitleInfo.Authors != null && fictionBook.TitleInfo.Authors.Count > 0)
            {
                AuthorInfoNode autorInfo = fictionBook.TitleInfo.Authors[0];
                if (!String.IsNullOrEmpty(autorInfo.NickName))
                {
                    return true;
                }
            }

            return false;
        }
    }

    internal sealed class FirstAuthorFirstNameCharElement : FileNamePatternElement
    {
        private readonly Regex replacePattern;

        public FirstAuthorFirstNameCharElement(string template) : base(template)
        {
            this.replacePattern = new Regex(Regex.Escape(template));
        }

        public override void Parse()
        {
        }

        public override string ApplyPattern(string pattern, FictionBook fictionBook)
        {
            string value = String.Empty;

            if (fictionBook.TitleInfo.Authors != null && fictionBook.TitleInfo.Authors.Count > 0)
            {
                AuthorInfoNode autorInfo = fictionBook.TitleInfo.Authors[0];

                if (!String.IsNullOrEmpty(autorInfo.FirstName))
                {
                    foreach (char c in autorInfo.FirstName)
                    {
                        if (Char.IsLetterOrDigit(c))
                        {
                            value = c.ToString().ToUpperInvariant();
                            break;
                        }
                    }
                }
            }

            return replacePattern.Replace(pattern, value);
        }

        public override bool HasValue(FictionBook fictionBook)
        {
            if (fictionBook.TitleInfo.Authors != null && fictionBook.TitleInfo.Authors.Count > 0)
            {
                AuthorInfoNode autorInfo = fictionBook.TitleInfo.Authors[0];

                if (!String.IsNullOrEmpty(autorInfo.FirstName))
                {
                    foreach (char c in autorInfo.FirstName)
                    {
                        if (Char.IsLetterOrDigit(c))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    internal sealed class FirstAuthorMiddleNameCharElement : FileNamePatternElement
    {
        private readonly Regex replacePattern;

        public FirstAuthorMiddleNameCharElement(string template) : base(template)
        {
            this.replacePattern = new Regex(Regex.Escape(template));
        }

        public override void Parse()
        {
        }

        public override string ApplyPattern(string pattern, FictionBook fictionBook)
        {
            string value = String.Empty;

            if (fictionBook.TitleInfo.Authors != null && fictionBook.TitleInfo.Authors.Count > 0)
            {
                AuthorInfoNode autorInfo = fictionBook.TitleInfo.Authors[0];

                if (!String.IsNullOrEmpty(autorInfo.MiddleName))
                {
                    foreach (char c in autorInfo.MiddleName)
                    {
                        if (Char.IsLetterOrDigit(c))
                        {
                            value = c.ToString().ToUpperInvariant();
                            break;
                        }
                    }
                }
            }

            return replacePattern.Replace(pattern, value);
        }

        public override bool HasValue(FictionBook fictionBook)
        {
            if (fictionBook.TitleInfo.Authors != null && fictionBook.TitleInfo.Authors.Count > 0)
            {
                AuthorInfoNode autorInfo = fictionBook.TitleInfo.Authors[0];

                if (!String.IsNullOrEmpty(autorInfo.MiddleName))
                {
                    foreach (char c in autorInfo.MiddleName)
                    {
                        if (Char.IsLetterOrDigit(c))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    internal sealed class FirstAuthorLastNameCharElement : FileNamePatternElement
    {
        private readonly Regex replacePattern;

        public FirstAuthorLastNameCharElement(string template) : base(template)
        {
            this.replacePattern = new Regex(Regex.Escape(template));
        }

        public override void Parse()
        {
        }

        public override string ApplyPattern(string pattern, FictionBook fictionBook)
        {
            string value = String.Empty;

            if (fictionBook.TitleInfo.Authors != null && fictionBook.TitleInfo.Authors.Count > 0)
            {
                AuthorInfoNode autorInfo = fictionBook.TitleInfo.Authors[0];

                if (!String.IsNullOrEmpty(autorInfo.LastName))
                {
                    foreach (char c in autorInfo.LastName)
                    {
                        if (Char.IsLetterOrDigit(c))
                        {
                            value = c.ToString().ToUpperInvariant();
                            break;
                        }
                    }
                }
            }

            return replacePattern.Replace(pattern, value);
        }

        public override bool HasValue(FictionBook fictionBook)
        {
            if (fictionBook.TitleInfo.Authors != null && fictionBook.TitleInfo.Authors.Count > 0)
            {
                AuthorInfoNode autorInfo = fictionBook.TitleInfo.Authors[0];

                if (!String.IsNullOrEmpty(autorInfo.LastName))
                {
                    foreach (char c in autorInfo.LastName)
                    {
                        if (Char.IsLetterOrDigit(c))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    internal sealed class SequenceNameElement : FileNamePatternElement
    {
        private readonly Regex replacePattern;

        public SequenceNameElement(string template) : base(template)
        {
            this.replacePattern = new Regex(Regex.Escape(template));
        }

        public override void Parse()
        {
        }

        public override string ApplyPattern(string pattern, FictionBook fictionBook)
        {
            string value = String.Empty;

            if (fictionBook.TitleInfo.Sequences != null && fictionBook.TitleInfo.Sequences.Count > 0)
            {
                SequenceInfoNode sequenceInfoNode = fictionBook.TitleInfo.Sequences[0];
                if (!String.IsNullOrEmpty(sequenceInfoNode.Name))
                {
                    value = CleanupValue(sequenceInfoNode.Name);
                }
            }

            return replacePattern.Replace(pattern, value);
        }

        public override bool HasValue(FictionBook fictionBook)
        {
            if (fictionBook.TitleInfo.Sequences != null && fictionBook.TitleInfo.Sequences.Count > 0)
            {
                SequenceInfoNode sequenceInfoNode = fictionBook.TitleInfo.Sequences[0];
                if (!String.IsNullOrEmpty(sequenceInfoNode.Name))
                {
                    return true;
                }
            }

            return false;
        }
    }

    internal sealed class SequenceNumberElement : FileNamePatternElement
    {
        private readonly Regex replacePattern;
        private readonly string numberFormat;

        public SequenceNumberElement(string template) : base(template)
        {
            this.replacePattern = new Regex(Regex.Escape(template));
            this.numberFormat = String.Empty;

            for(int index = 0; index < template.Length; index++)
            {
                if(template[index] == 'I')
                {
                    this.numberFormat += "0";
                }
            }

            if(this.numberFormat.Length == 1)
            {
                this.numberFormat = String.Empty;
            }
        }

        public override void Parse()
        {
        }

        public override string ApplyPattern(string pattern, FictionBook fictionBook)
        {
            string value = String.Empty;

            if (fictionBook.TitleInfo.Sequences != null && fictionBook.TitleInfo.Sequences.Count > 0)
            {
                SequenceInfoNode sequenceInfoNode = fictionBook.TitleInfo.Sequences[0];
                if (sequenceInfoNode.Number != null)
                {
                    value = sequenceInfoNode.Number.Value.ToString(this.numberFormat, CultureInfo.InvariantCulture);
                }
            }

            return replacePattern.Replace(pattern, value);
        }

        public override bool HasValue(FictionBook fictionBook)
        {
            if (fictionBook.TitleInfo.Sequences != null && fictionBook.TitleInfo.Sequences.Count > 0)
            {
                SequenceInfoNode sequenceInfoNode = fictionBook.TitleInfo.Sequences[0];
                if (sequenceInfoNode.Number != null)
                {
                    return true;
                }
            }

            return false;
        }
    }

    internal sealed class BookTitleElement : FileNamePatternElement
    {
        private readonly Regex replacePattern;

        public BookTitleElement(string template) : base(template)
        {
            this.replacePattern = new Regex(Regex.Escape(template));
        }

        public override void Parse()
        {
        }

        public override string ApplyPattern(string pattern, FictionBook fictionBook)
        {
            string value = String.Empty;

            if (!String.IsNullOrEmpty(fictionBook.TitleInfo.BookTitle))
            {
                value = CleanupValue(fictionBook.TitleInfo.BookTitle);
            }

            return replacePattern.Replace(pattern, value);
        }

        public override bool HasValue(FictionBook fictionBook)
        {
            if (fictionBook.TitleInfo != null)
            {
                if (!String.IsNullOrEmpty(fictionBook.TitleInfo.BookTitle))
                {
                    return true;
                }
            }

            return false;
        }
    }

    internal sealed class GenreNameElement : FileNamePatternElement
    {
        private readonly Regex replacePattern;

        public GenreNameElement(string template) : base(template)
        {
            this.replacePattern = new Regex(Regex.Escape(template));
        }

        public override void Parse()
        {
        }

        public override string ApplyPattern(string pattern, FictionBook fictionBook)
        {
            string value = String.Empty;

            if (fictionBook.TitleInfo.Genres != null && fictionBook.TitleInfo.Genres.Count > 0)
            {
                GenreInfoNode genreInfoNode = fictionBook.TitleInfo.Genres[0];
                if (!String.IsNullOrEmpty(genreInfoNode.Genre))
                {
                    value = CleanupValue(genreInfoNode.Genre);
                }
            }

            return replacePattern.Replace(pattern, value);
        }

        public override bool HasValue(FictionBook fictionBook)
        {
            if (fictionBook.TitleInfo.Genres != null && fictionBook.TitleInfo.Genres.Count > 0)
            {
                GenreInfoNode genreInfoNode = fictionBook.TitleInfo.Genres[0];
                if (!String.IsNullOrEmpty(genreInfoNode.Genre))
                {
                    return true;
                }
            }

            return false;
        }
    }

    internal sealed class GenreDescriptionElement : FileNamePatternElement
    {
        private readonly Regex replacePattern;
        private readonly string lang;

        public GenreDescriptionElement(string template) : base(template)
        {
            this.replacePattern = new Regex(Regex.Escape(template));

            switch(template)
            {
                case "*GNR*":
                    lang = "ru";
                    break;

                case "*GNE*":
                    lang = "en";
                    break;
            }
        }

        public override void Parse()
        {
        }

        public override string ApplyPattern(string pattern, FictionBook fictionBook)
        {
            string value = String.Empty;

            if (fictionBook.TitleInfo.Genres != null && fictionBook.TitleInfo.Genres.Count > 0)
            {
                GenreInfoNode genreInfoNode = fictionBook.TitleInfo.Genres[0];
                if (!String.IsNullOrEmpty(genreInfoNode.Genre))
                {
                    Genre genre = GenreTable.Table[genreInfoNode.Genre];
                    if(genre != null)
                    {
                        value = CleanupValue(genre.GetDescription(this.lang));
                    }
                }
            }

            return replacePattern.Replace(pattern, value);
        }

        public override bool HasValue(FictionBook fictionBook)
        {
            if (fictionBook.TitleInfo.Genres != null && fictionBook.TitleInfo.Genres.Count > 0)
            {
                GenreInfoNode genreInfoNode = fictionBook.TitleInfo.Genres[0];
                if (!String.IsNullOrEmpty(genreInfoNode.Genre))
                {
                    Genre genre = GenreTable.Table[genreInfoNode.Genre];
                    if (genre != null)
                    {
                        if(!String.IsNullOrEmpty(genre.GetDescription(this.lang)))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    internal sealed class GroupPatternElement : FileNamePattern
    {
        private readonly Regex replaceRegex;
        
        public GroupPatternElement(string template) : base(template)
        {
            this.replaceRegex = new Regex(@"\[" + Regex.Escape(template) + @"\]");
        }

        public override string ApplyPattern(string pattern, FictionBook fictionBook)
        {
            string result = Template;
            bool hasContent = false;

            foreach (FileNamePatternElement element in Elements)
            {
                if(element.HasValue(fictionBook))
                {
                    hasContent = true;
                }

                result = element.ApplyPattern(result, fictionBook);
            }

            if(!hasContent)
            {
                return replaceRegex.Replace(pattern, String.Empty);
            }

            return replaceRegex.Replace(pattern, result);
        }
    }

    internal sealed class OptionPatternElement : FileNamePatternElement
    {
        private readonly Regex replaceRegex;

        public OptionPatternElement(string template) : base(template)
        {
            this.replaceRegex = new Regex(Regex.Escape(template));
        }

        public override void Parse()
        {
        }

        public override string ApplyPattern(string pattern, FictionBook fictionBook)
        {
            return replaceRegex.Replace(pattern, String.Empty);
        }

        public override bool HasValue(FictionBook fictionBook)
        {
            return false;
        }
    }

    internal class FileNamePattern : FileNamePatternElement
    {
        private static readonly Dictionary<string, Type> knownPatterns;
        private readonly Regex patternParts;
        private readonly List<FileNamePatternElement> elements;

        static FileNamePattern()
        {
            knownPatterns = new Dictionary<string, Type>(14);
            knownPatterns.Add("*NF*", typeof(AuthorFirstNameElement));
            knownPatterns.Add("*NM*", typeof(AuthorMiddleNameElement));
            knownPatterns.Add("*NL*", typeof(AuthorLastNameElement));
            knownPatterns.Add("*NN*", typeof(AuthorNickNameElement));
            knownPatterns.Add("*NFA*", typeof(FirstAuthorFirstNameCharElement));
            knownPatterns.Add("*NMA*", typeof(FirstAuthorMiddleNameCharElement));
            knownPatterns.Add("*NLA*", typeof(FirstAuthorLastNameCharElement));
            knownPatterns.Add("*NA*", typeof(FirstAuthorLastNameCharElement));
            knownPatterns.Add("*SN*", typeof(SequenceNameElement));
            knownPatterns.Add("*SI*", typeof(SequenceNumberElement));
            knownPatterns.Add("*SII*", typeof(SequenceNumberElement));
            knownPatterns.Add("*SIII*", typeof(SequenceNumberElement));
            knownPatterns.Add("*BN*", typeof(BookTitleElement));
            knownPatterns.Add("*GN*", typeof(GenreNameElement));
            knownPatterns.Add("*GNR*", typeof(GenreDescriptionElement));
            knownPatterns.Add("*GNE*", typeof(GenreDescriptionElement));
        }

        public FileNamePattern(string template) : base(template)
        {
            this.patternParts = new Regex(@"(\[(?<group>.+?)\]|(?<pattern>\*\w+\*)|(?<option>\{.??\}))");
            this.elements = new List<FileNamePatternElement>();
        }

        public ReadOnlyCollection<FileNamePatternElement> Elements
        {
            get
            {
                return this.elements.AsReadOnly();
            }
        }

        public override void Parse()
        {
            foreach(Match match in this.patternParts.Matches(Template))
            {
                Group groupPattern = match.Groups["group"];
                Group namePattern = match.Groups["pattern"];
                Group optionPattern = match.Groups["option"];

                if(namePattern.Success)
                {
                    if(knownPatterns.ContainsKey(namePattern.Value))
                    {
                        Type patternType = knownPatterns[namePattern.Value];
                        
                        FileNamePatternElement patternElement = (FileNamePatternElement) Activator.CreateInstance(patternType, namePattern.Value);
                        patternElement.Parse();

                        this.elements.Add(patternElement);
                    }
                }
                else if(groupPattern.Success)
                {
                    GroupPatternElement groupElement = new GroupPatternElement(groupPattern.Value);
                    groupElement.Parse();

                    this.elements.Add(groupElement);
                }
                else if(optionPattern.Success)
                {
                    OptionPatternElement optionElement = new OptionPatternElement(optionPattern.Value);
                    optionElement.Parse();

                    this.elements.Add(optionElement);
                }
            }
        }

        public override string ApplyPattern(string pattern, FictionBook fictionBook)
        {
            string result = Template;

            foreach (FileNamePatternElement element in elements)
            {
                result = element.ApplyPattern(result, fictionBook);    
            }

            return result;
        }

        public override bool HasValue(FictionBook fictionBook)
        {
            return true;
        }
    }

    public class FileNameProvider
    {
        private readonly string template;
        private readonly FileNamePattern pattern;

        public FileNameProvider(string template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            this.template = template;

            this.pattern = new FileNamePattern(template);
            this.pattern.Parse();
        }

        public string GetFileName(FictionBook fictionbook)
        {
            if (fictionbook == null)
            {
                throw new ArgumentNullException("fictionbook");
            }

            return this.pattern.ApplyPattern(this.template, fictionbook);
        }
    }
}
