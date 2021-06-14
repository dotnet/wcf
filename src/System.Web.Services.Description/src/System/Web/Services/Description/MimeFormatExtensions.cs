// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Xml.Serialization;
using System.Collections;
using System.ComponentModel;
using System.Web.Services.Configuration;
using System.Globalization;

namespace System.Web.Services.Description {
    [XmlFormatExtension("content", Namespace, typeof(MimePart), typeof(InputBinding), typeof(OutputBinding))]
    [XmlFormatExtensionPrefix("mime", Namespace)]
    public sealed class MimeContentBinding : ServiceDescriptionFormatExtension {
        private string _type;

        [XmlAttribute("part")]
        public string Part { get; set; }

        [XmlAttribute("type")]
        public string Type {
            get { return _type == null ? string.Empty : _type; }
            set { _type = value; }
        }

        public const string Namespace = "http://schemas.xmlsoap.org/wsdl/mime/";
    }

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class MimePart : ServiceDescriptionFormatExtension {
        private ServiceDescriptionFormatExtensionCollection _extensions;

        [XmlIgnore]
        public ServiceDescriptionFormatExtensionCollection Extensions {
            get { if (_extensions == null) { _extensions = new ServiceDescriptionFormatExtensionCollection(this); } return _extensions; }
        }
    }

    [XmlFormatExtension("multipartRelated", MimeContentBinding.Namespace, typeof(InputBinding), typeof(OutputBinding))]
    public sealed class MimeMultipartRelatedBinding : ServiceDescriptionFormatExtension {
        [XmlElement("part")]
        public MimePartCollection Parts { get; } = new MimePartCollection();
    }

    [XmlFormatExtension("mimeXml", MimeContentBinding.Namespace, typeof(MimePart), typeof(InputBinding), typeof(OutputBinding))]
    public sealed class MimeXmlBinding : ServiceDescriptionFormatExtension {
        [XmlAttribute("part")]
        public string Part { get; set; }
    }

    public sealed class MimePartCollection : CollectionBase {
        public MimePart this[int index] {
            get { return (MimePart)List[index]; }
            set { List[index] = value; }
        }

        public int Add(MimePart mimePart) {
            return List.Add(mimePart);
        }

        public void Insert(int index, MimePart mimePart) {
            List.Insert(index, mimePart);
        }

        public int IndexOf(MimePart mimePart) {
            return List.IndexOf(mimePart);
        }

        public bool Contains(MimePart mimePart) {
            return List.Contains(mimePart);
        }

        public void Remove(MimePart mimePart) {
            List.Remove(mimePart);
        }

        public void CopyTo(MimePart[] array, int index) {
            List.CopyTo(array, index);
        }
    }

    [XmlFormatExtension("text", Namespace, typeof(InputBinding), typeof(OutputBinding), typeof(MimePart))]
    [XmlFormatExtensionPrefix("tm", Namespace)]
    public sealed class MimeTextBinding : ServiceDescriptionFormatExtension {
        public const string Namespace = "http://microsoft.com/wsdl/mime/textMatching/";

        [XmlElement("match", typeof(MimeTextMatch))]
        public MimeTextMatchCollection Matches { get; } = new MimeTextMatchCollection();
    }

    public sealed class MimeTextMatch {
        private string _name;
        private string _type;
        private int _repeats = 1;
        private string _pattern;
        private int _group = 1;
        private int _capture = 0;

        [XmlAttribute("name")]
        public string Name {
            get { return _name == null ? string.Empty : _name; }
            set { _name = value; }
        }

        [XmlAttribute("type")]
        public string Type {
            get { return _type == null ? string.Empty : _type; }
            set { _type = value; }
        }

        [XmlAttribute("group"), DefaultValue(1)]
        public int Group {
            get { return _group; }
            set {
                if (value < 0)
                {
                    throw new ArgumentException(SR.Format(SR.WebNegativeValue, "group"));
                }

                _group = value;
            }
        }

        [XmlAttribute("capture"), DefaultValue(0)]
        public int Capture {
            get { return _capture; }
            set {
                if (value < 0)
                {
                    throw new ArgumentException(SR.Format(SR.WebNegativeValue, "capture"));
                }

                _capture = value;
            }
        }

        [XmlIgnore()]
        public int Repeats {
            get { return _repeats; }
            set {
                if (value < 0)
                {
                    throw new ArgumentException(SR.Format(SR.WebNegativeValue, "repeats"));
                }

                _repeats = value;
            }
        }

        [XmlAttribute("repeats"), DefaultValue("1")]
        public string RepeatsString {
            get { return _repeats == int.MaxValue ? "*" : _repeats.ToString(CultureInfo.InvariantCulture); }
            set {
                if (value == "*")
                {
                    _repeats = int.MaxValue;
                }
                else
                {
                    Repeats = int.Parse(value, CultureInfo.InvariantCulture);  // pass through our setter for arg checking
                }
            }
        }

        [XmlAttribute("pattern")]
        public string Pattern {
            get { return _pattern == null ? string.Empty : _pattern; }
            set { _pattern = value; }
        }

        [XmlAttribute("ignoreCase")]
        public bool IgnoreCase { get; set; } = false;

        [XmlElement("match")]
        public MimeTextMatchCollection Matches { get; } = new MimeTextMatchCollection();
    }

    public sealed class MimeTextMatchCollection : CollectionBase {
        public MimeTextMatch this[int index] {
            get { return (MimeTextMatch)List[index]; }
            set { List[index] = value; }
        }

        public int Add(MimeTextMatch match) {
            return List.Add(match);
        }

        public void Insert(int index, MimeTextMatch match) {
            List.Insert(index, match);
        }

        public int IndexOf(MimeTextMatch match) {
            return List.IndexOf(match);
        }

        public bool Contains(MimeTextMatch match) {
            return List.Contains(match);
        }

        public void Remove(MimeTextMatch match) {
            List.Remove(match);
        }

        public void CopyTo(MimeTextMatch[] array, int index) {
            List.CopyTo(array, index);
        }
    }
}
