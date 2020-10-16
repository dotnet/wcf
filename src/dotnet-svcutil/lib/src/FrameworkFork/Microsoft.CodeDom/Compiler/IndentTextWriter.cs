// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeDom.Compiler
{
    using System.Diagnostics;
    using System;
    using System.IO;
    using System.Text;

    using System.Globalization;

    /// <devdoc>
    ///    <para>Provides a text writer that can indent new lines by a tabString token.</para>
    /// </devdoc>
    public class IndentedTextWriter : TextWriter
    {
        private TextWriter _writer;
        private int _indentLevel;
        private bool _tabsPending;
        private string _tabString;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public const string DefaultTabString = "    ";

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.Compiler.IndentedTextWriter'/> using the specified
        ///       text writer and default tab string.
        ///    </para>
        /// </devdoc>
        public IndentedTextWriter(TextWriter writer) : this(writer, DefaultTabString)
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.Compiler.IndentedTextWriter'/> using the specified
        ///       text writer and tab string.
        ///    </para>
        /// </devdoc>
        public IndentedTextWriter(TextWriter writer, string tabString) : base(CultureInfo.InvariantCulture)
        {
            _writer = writer;
            _tabString = tabString;
            _indentLevel = 0;
            _tabsPending = false;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override Encoding Encoding
        {
            get
            {
                return _writer.Encoding;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the new line character to use.
        ///    </para>
        /// </devdoc>
        public override string NewLine
        {
            get
            {
                return _writer.NewLine;
            }

            set
            {
                _writer.NewLine = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the number of spaces to indent.
        ///    </para>
        /// </devdoc>
        public int Indent
        {
            get
            {
                return _indentLevel;
            }
            set
            {
                Debug.Assert(value >= 0, "Bogus Indent... probably caused by mismatched Indent++ and Indent--");
                if (value < 0)
                {
                    value = 0;
                }
                _indentLevel = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the TextWriter to use.
        ///    </para>
        /// </devdoc>
        public TextWriter InnerWriter
        {
            get
            {
                return _writer;
            }
        }

        internal string TabString
        {
            get { return _tabString; }
        }

        /// <devdoc>
        ///    <para>
        ///       Closes the document being written to.
        ///    </para>
        /// </devdoc>
        //public override void Close() {
        //    writer.Close();
        //}

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override void Flush()
        {
            _writer.Flush();
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected virtual void OutputTabs()
        {
            if (_tabsPending)
            {
                for (int i = 0; i < _indentLevel; i++)
                {
                    _writer.Write(_tabString);
                }
                _tabsPending = false;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Writes a string
        ///       to the text stream.
        ///    </para>
        /// </devdoc>
        public override void Write(string s)
        {
            OutputTabs();
            _writer.Write(s);
        }

        /// <devdoc>
        ///    <para>
        ///       Writes the text representation of a Boolean value to the text stream.
        ///    </para>
        /// </devdoc>
        public override void Write(bool value)
        {
            OutputTabs();
            _writer.Write(value);
        }

        /// <devdoc>
        ///    <para>
        ///       Writes a character to the text stream.
        ///    </para>
        /// </devdoc>
        public override void Write(char value)
        {
            OutputTabs();
            _writer.Write(value);
        }

        /// <devdoc>
        ///    <para>
        ///       Writes a
        ///       character array to the text stream.
        ///    </para>
        /// </devdoc>
        public override void Write(char[] buffer)
        {
            OutputTabs();
            _writer.Write(buffer);
        }

        /// <devdoc>
        ///    <para>
        ///       Writes a subarray
        ///       of characters to the text stream.
        ///    </para>
        /// </devdoc>
        public override void Write(char[] buffer, int index, int count)
        {
            OutputTabs();
            _writer.Write(buffer, index, count);
        }

        /// <devdoc>
        ///    <para>
        ///       Writes the text representation of a Double to the text stream.
        ///    </para>
        /// </devdoc>
        public override void Write(double value)
        {
            OutputTabs();
            _writer.Write(value);
        }

        /// <devdoc>
        ///    <para>
        ///       Writes the text representation of
        ///       a Single to the text
        ///       stream.
        ///    </para>
        /// </devdoc>
        public override void Write(float value)
        {
            OutputTabs();
            _writer.Write(value);
        }

        /// <devdoc>
        ///    <para>
        ///       Writes the text representation of an integer to the text stream.
        ///    </para>
        /// </devdoc>
        public override void Write(int value)
        {
            OutputTabs();
            _writer.Write(value);
        }

        /// <devdoc>
        ///    <para>
        ///       Writes the text representation of an 8-byte integer to the text stream.
        ///    </para>
        /// </devdoc>
        public override void Write(long value)
        {
            OutputTabs();
            _writer.Write(value);
        }

        /// <devdoc>
        ///    <para>
        ///       Writes the text representation of an object
        ///       to the text stream.
        ///    </para>
        /// </devdoc>
        public override void Write(object value)
        {
            OutputTabs();
            _writer.Write(value);
        }

        /// <devdoc>
        ///    <para>
        ///       Writes out a formatted string, using the same semantics as specified.
        ///    </para>
        /// </devdoc>
        public override void Write(string format, object arg0)
        {
            OutputTabs();
            _writer.Write(format, arg0);
        }

        /// <devdoc>
        ///    <para>
        ///       Writes out a formatted string,
        ///       using the same semantics as specified.
        ///    </para>
        /// </devdoc>
        public override void Write(string format, object arg0, object arg1)
        {
            OutputTabs();
            _writer.Write(format, arg0, arg1);
        }

        /// <devdoc>
        ///    <para>
        ///       Writes out a formatted string,
        ///       using the same semantics as specified.
        ///    </para>
        /// </devdoc>
        public override void Write(string format, params object[] arg)
        {
            OutputTabs();
            _writer.Write(format, arg);
        }

        /// <devdoc>
        ///    <para>
        ///       Writes the specified
        ///       string to a line without tabs.
        ///    </para>
        /// </devdoc>
        public void WriteLineNoTabs(string s)
        {
            _writer.WriteLine(s);
        }

        /// <devdoc>
        ///    <para>
        ///       Writes the specified string followed by
        ///       a line terminator to the text stream.
        ///    </para>
        /// </devdoc>
        public override void WriteLine(string s)
        {
            OutputTabs();
            _writer.WriteLine(s);
            _tabsPending = true;
        }

        /// <devdoc>
        ///    <para>
        ///       Writes a line terminator.
        ///    </para>
        /// </devdoc>
        public override void WriteLine()
        {
            OutputTabs();
            _writer.WriteLine();
            _tabsPending = true;
        }

        /// <devdoc>
        ///    <para>
        ///       Writes the text representation of a Boolean followed by a line terminator to
        ///       the text stream.
        ///    </para>
        /// </devdoc>
        public override void WriteLine(bool value)
        {
            OutputTabs();
            _writer.WriteLine(value);
            _tabsPending = true;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override void WriteLine(char value)
        {
            OutputTabs();
            _writer.WriteLine(value);
            _tabsPending = true;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override void WriteLine(char[] buffer)
        {
            OutputTabs();
            _writer.WriteLine(buffer);
            _tabsPending = true;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override void WriteLine(char[] buffer, int index, int count)
        {
            OutputTabs();
            _writer.WriteLine(buffer, index, count);
            _tabsPending = true;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override void WriteLine(double value)
        {
            OutputTabs();
            _writer.WriteLine(value);
            _tabsPending = true;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override void WriteLine(float value)
        {
            OutputTabs();
            _writer.WriteLine(value);
            _tabsPending = true;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override void WriteLine(int value)
        {
            OutputTabs();
            _writer.WriteLine(value);
            _tabsPending = true;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override void WriteLine(long value)
        {
            OutputTabs();
            _writer.WriteLine(value);
            _tabsPending = true;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override void WriteLine(object value)
        {
            OutputTabs();
            _writer.WriteLine(value);
            _tabsPending = true;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override void WriteLine(string format, object arg0)
        {
            OutputTabs();
            _writer.WriteLine(format, arg0);
            _tabsPending = true;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override void WriteLine(string format, object arg0, object arg1)
        {
            OutputTabs();
            _writer.WriteLine(format, arg0, arg1);
            _tabsPending = true;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override void WriteLine(string format, params object[] arg)
        {
            OutputTabs();
            _writer.WriteLine(format, arg);
            _tabsPending = true;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        //// [CLSCompliant(false)]
        public override void WriteLine(UInt32 value)
        {
            OutputTabs();
            _writer.WriteLine(value);
            _tabsPending = true;
        }

        internal void InternalOutputTabs()
        {
            for (int i = 0; i < _indentLevel; i++)
            {
                _writer.Write(_tabString);
            }
        }
    }

    internal class Indentation
    {
        private IndentedTextWriter _writer;
        private int _indent;
        private string _s;

        internal Indentation(IndentedTextWriter writer, int indent)
        {
            _writer = writer;
            _indent = indent;
            _s = null;
        }

        internal string IndentationString
        {
            get
            {
                if (_s == null)
                {
                    string tabString = _writer.TabString;
                    StringBuilder sb = new StringBuilder(_indent * tabString.Length);
                    for (int i = 0; i < _indent; i++)
                    {
                        sb.Append(tabString);
                    }
                    _s = sb.ToString();
                }
                return _s;
            }
        }
    }
}
