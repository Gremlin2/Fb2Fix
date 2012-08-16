using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace FB2Fix
{
    [Serializable]
    [XmlType(AnonymousType=true)]
    [XmlRoot(Namespace="", IsNullable=false)]
    public class fbgenrestransfer
    {
        private genre[] genreField;

        [XmlElement("genre")]
        public genre[] genre
        {
            get
            {
                return this.genreField;
            }
            set
            {
                this.genreField = value;
            }
        }
    }

    [Serializable]
    [XmlType(AnonymousType=true)]
    [XmlRoot(Namespace="", IsNullable=false)]
    public class genre
    {
        private rootdescr[] rootdescrField;
        private subgenre[] subgenresField;
        private string valueField;

        [XmlElement("root-descr")]
        public rootdescr[] rootdescr
        {
            get
            {
                return this.rootdescrField;
            }
            set
            {
                this.rootdescrField = value;
            }
        }

        [XmlArrayItem("subgenre", IsNullable=false)]
        public subgenre[] subgenres
        {
            get
            {
                return this.subgenresField;
            }
            set
            {
                this.subgenresField = value;
            }
        }

        [XmlAttribute(DataType="NCName")]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    [Serializable]
    [DebuggerStepThrough]
    [XmlType(AnonymousType=true)]
    [XmlRoot("root-descr", Namespace="", IsNullable=false)]
    public class rootdescr
    {
        private string detailedField;
        private string genretitleField;
        private string langField;

        [XmlAttribute]
        public string detailed
        {
            get
            {
                return this.detailedField;
            }
            set
            {
                this.detailedField = value;
            }
        }

        [XmlAttribute("genre-title")]
        public string genretitle
        {
            get
            {
                return this.genretitleField;
            }
            set
            {
                this.genretitleField = value;
            }
        }

        [XmlAttribute(DataType="NCName")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(AnonymousType=true)]
    [XmlRoot(Namespace="", IsNullable=false)]
    public class subgenres
    {
        private subgenre[] subgenreField;

        [XmlElement("subgenre")]
        public subgenre[] subgenre
        {
            get
            {
                return this.subgenreField;
            }
            set
            {
                this.subgenreField = value;
            }
        }
    }

    [Serializable]
    [DebuggerStepThrough]
    [XmlType(AnonymousType=true)]
    [XmlRoot(Namespace="", IsNullable=false)]
    public class subgenre
    {
        private genredescr[] genredescrField;
        private genrealt[] genrealtField;
        private string valueField;

        [XmlElement("genre-descr")]
        public genredescr[] genredescr
        {
            get
            {
                return this.genredescrField;
            }
            set
            {
                this.genredescrField = value;
            }
        }

        [XmlElement("genre-alt")]
        public genrealt[] genrealt
        {
            get
            {
                return this.genrealtField;
            }
            set
            {
                this.genrealtField = value;
            }
        }

        [XmlAttribute(DataType="NCName")]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    [Serializable]
    [DebuggerStepThrough]
    [XmlType(AnonymousType=true)]
    [XmlRoot("genre-descr", Namespace="", IsNullable=false)]
    public class genredescr
    {
        private string langField;
        private string titleField;

        [XmlAttribute(DataType="NCName")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }

        [XmlAttribute]
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }
    }

    [Serializable]
    [DebuggerStepThrough]
    [XmlType(AnonymousType=true)]
    [XmlRoot("genre-alt", Namespace="", IsNullable=false)]
    public class genrealt
    {
        private string formatField;
        private string valueField;

        [XmlAttribute(DataType="NCName")]
        public string format
        {
            get
            {
                return this.formatField;
            }
            set
            {
                this.formatField = value;
            }
        }

        [XmlAttribute(DataType="NCName")]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
}