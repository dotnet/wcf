// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Web.Services.Description
{
    using Microsoft.Xml.Serialization;
    using System.Collections;
    using System.ComponentModel;
    using System.Web.Services.Configuration;
    using System.Globalization;

    /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeContentBinding"]/*' />
    [XmlFormatExtension("content", MimeContentBinding.Namespace, typeof(MimePart), typeof(InputBinding), typeof(OutputBinding))]
    [XmlFormatExtensionPrefix("mime", MimeContentBinding.Namespace)]
    public sealed class MimeContentBinding : ServiceDescriptionFormatExtension
    {
        private string _type;
        private string _part;

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeContentBinding.Part"]/*' />
        [XmlAttribute("part")]
        public string Part
        {
            get { return _part; }
            set { _part = value; }
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeContentBinding.Type"]/*' />
        [XmlAttribute("type")]
        public string Type
        {
            get { return _type == null ? string.Empty : _type; }
            set { _type = value; }
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeContentBinding.Namespace"]/*' />
        public const string Namespace = "http://schemas.xmlsoap.org/wsdl/mime/";
    }

    /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimePart"]/*' />
    [XmlFormatExtensionPoint("Extensions")]
    public sealed class MimePart : ServiceDescriptionFormatExtension
    {
        private ServiceDescriptionFormatExtensionCollection _extensions;

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimePart.Extensions"]/*' />
        [XmlIgnore]
        public ServiceDescriptionFormatExtensionCollection Extensions
        {
            get { if (_extensions == null) _extensions = new ServiceDescriptionFormatExtensionCollection(this); return _extensions; }
        }
    }

    /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeMultipartRelatedBinding"]/*' />
    [XmlFormatExtension("multipartRelated", MimeContentBinding.Namespace, typeof(InputBinding), typeof(OutputBinding))]
    public sealed class MimeMultipartRelatedBinding : ServiceDescriptionFormatExtension
    {
        private MimePartCollection _parts = new MimePartCollection();

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeMultipartRelatedBinding.Parts"]/*' />
        [XmlElement("part")]
        public MimePartCollection Parts
        {
            get { return _parts; }
        }
    }

    /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeXmlBinding"]/*' />
    [XmlFormatExtension("mimeXml", MimeContentBinding.Namespace, typeof(MimePart), typeof(InputBinding), typeof(OutputBinding))]
    public sealed class MimeXmlBinding : ServiceDescriptionFormatExtension
    {
        private string _part;

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeXmlBinding.Part"]/*' />
        [XmlAttribute("part")]
        public string Part
        {
            get { return _part; }
            set { _part = value; }
        }
    }

    /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimePartCollection"]/*' />
    public sealed class MimePartCollection : CollectionBase
    {
        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimePartCollection.this"]/*' />
        public MimePart this[int index]
        {
            get { return (MimePart)List[index]; }
            set { List[index] = value; }
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimePartCollection.Add"]/*' />
        public int Add(MimePart mimePart)
        {
            return List.Add(mimePart);
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimePartCollection.Insert"]/*' />
        public void Insert(int index, MimePart mimePart)
        {
            List.Insert(index, mimePart);
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimePartCollection.IndexOf"]/*' />
        public int IndexOf(MimePart mimePart)
        {
            return List.IndexOf(mimePart);
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimePartCollection.Contains"]/*' />
        public bool Contains(MimePart mimePart)
        {
            return List.Contains(mimePart);
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimePartCollection.Remove"]/*' />
        public void Remove(MimePart mimePart)
        {
            List.Remove(mimePart);
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimePartCollection.CopyTo"]/*' />
        public void CopyTo(MimePart[] array, int index)
        {
            List.CopyTo(array, index);
        }
    }

    /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextBinding"]/*' />
    [XmlFormatExtension("text", MimeTextBinding.Namespace, typeof(InputBinding), typeof(OutputBinding), typeof(MimePart))]
    [XmlFormatExtensionPrefix("tm", MimeTextBinding.Namespace)]
    public sealed class MimeTextBinding : ServiceDescriptionFormatExtension
    {
        private MimeTextMatchCollection _matches = new MimeTextMatchCollection();

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextBinding.Namespace"]/*' />
        public const string Namespace = "http://microsoft.com/wsdl/mime/textMatching/";

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextBinding.Matches"]/*' />
        [XmlElement("match", typeof(MimeTextMatch))]
        public MimeTextMatchCollection Matches
        {
            get { return _matches; }
        }
    }

    /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatch"]/*' />
    public sealed class MimeTextMatch
    {
        private string _name;
        private string _type;
        private int _repeats = 1;
        private string _pattern;
        private int _group = 1;
        private int _capture = 0;
        private bool _ignoreCase = false;
        private MimeTextMatchCollection _matches = new MimeTextMatchCollection();

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatch.Name"]/*' />
        [XmlAttribute("name")]
        public string Name
        {
            get { return _name == null ? string.Empty : _name; }
            set { _name = value; }
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatch.Type"]/*' />
        [XmlAttribute("type")]
        public string Type
        {
            get { return _type == null ? string.Empty : _type; }
            set { _type = value; }
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatch.Group"]/*' />
        [XmlAttribute("group"), DefaultValue(1)]
        public int Group
        {
            get { return _group; }
            set
            {
                if (value < 0) throw new ArgumentException(string.Format(ResWebServices.WebNegativeValue, "group"));
                _group = value;
            }
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatch.Capture"]/*' />
        [XmlAttribute("capture"), DefaultValue(0)]
        public int Capture
        {
            get { return _capture; }
            set
            {
                if (value < 0) throw new ArgumentException(string.Format(ResWebServices.WebNegativeValue, "capture"));
                _capture = value;
            }
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatch.Repeats"]/*' />
        [XmlIgnore()]
        public int Repeats
        {
            get { return _repeats; }
            set
            {
                if (value < 0) throw new ArgumentException(string.Format(ResWebServices.WebNegativeValue, "repeats"));
                _repeats = value;
            }
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatch.RepeatsString"]/*' />
        [XmlAttribute("repeats"), DefaultValue("1")]
        public string RepeatsString
        {
            get { return _repeats == int.MaxValue ? "*" : _repeats.ToString(CultureInfo.InvariantCulture); }
            set
            {
                if (value == "*")
                    _repeats = int.MaxValue;
                else
                    Repeats = int.Parse(value, CultureInfo.InvariantCulture);  // pass through our setter for arg checking
            }
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatch.Pattern"]/*' />
        [XmlAttribute("pattern")]
        public string Pattern
        {
            get { return _pattern == null ? string.Empty : _pattern; }
            set { _pattern = value; }
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatch.IgnoreCase"]/*' />
        [XmlAttribute("ignoreCase")]
        public bool IgnoreCase
        {
            get { return _ignoreCase; }
            set { _ignoreCase = value; }
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatch.Matches"]/*' />   
        [XmlElement("match")]
        public MimeTextMatchCollection Matches
        {
            get { return _matches; }
        }
    }

    /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatchCollection"]/*' />
    public sealed class MimeTextMatchCollection : CollectionBase
    {
        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatchCollection.this"]/*' />
        public MimeTextMatch this[int index]
        {
            get { return (MimeTextMatch)List[index]; }
            set { List[index] = value; }
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatchCollection.Add"]/*' />
        public int Add(MimeTextMatch match)
        {
            return List.Add(match);
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatchCollection.Insert"]/*' />
        public void Insert(int index, MimeTextMatch match)
        {
            List.Insert(index, match);
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatchCollection.IndexOf"]/*' />
        public int IndexOf(MimeTextMatch match)
        {
            return List.IndexOf(match);
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatchCollection.Contains"]/*' />
        public bool Contains(MimeTextMatch match)
        {
            return List.Contains(match);
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatchCollection.Remove"]/*' />
        public void Remove(MimeTextMatch match)
        {
            List.Remove(match);
        }

        /// <include file='doc\MimeFormatExtensions.uex' path='docs/doc[@for="MimeTextMatchCollection.CopyTo"]/*' />
        public void CopyTo(MimeTextMatch[] array, int index)
        {
            List.CopyTo(array, index);
        }
    }
}

