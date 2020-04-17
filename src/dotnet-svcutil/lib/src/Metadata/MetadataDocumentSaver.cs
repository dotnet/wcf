// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel.Description;
#if PRIVATE_RTLIB
using Microsoft.Xml.Schema;
using XmlNS = Microsoft.Xml;
#else
using System.Xml.Schema;
using XmlNS = System.Xml;
#endif
using WsdlNS = System.Web.Services.Description;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    public class MetadataDocumentSaver
    {
        private const string defaultPolicyFileName = "policy";
        private const string defaultMetadataFileName = "metadata";
        internal const bool DefaultOverwrite = false;
        internal const MetadataFileNamingConvention DefaultNamingConvention = MetadataFileNamingConvention.Namespace;

        private MetadataFileNameManager FileNameMgr { get; set; }
        private List<MetadataFileInfo> MetadataFiles { get; set; }
        private string DirectoryPath { get; set; }
        private MetadataFileNamingConvention NamingConvention { get; set; }
        private List<UnresolvedUri> UnresolvedReferences { get; set; }

        private MetadataDocumentSaver(string directoryPath, IEnumerable<MetadataSection> documents, MetadataFileNamingConvention namingConvention)
        {
            this.DirectoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
            this.MetadataFiles = new List<MetadataFileInfo>();
            this.NamingConvention = namingConvention;
            this.FileNameMgr = new MetadataFileNameManager();
            this.UnresolvedReferences = new List<UnresolvedUri>();

            AddMetadataFiles(documents);
        }

        public static async Task<SaveResult> SaveMetadataAsync(string directoryPath, IEnumerable<MetadataSection> documents, CancellationToken cancellationToken)
        {
            return await SaveMetadataAsync(directoryPath, documents, DefaultNamingConvention, DefaultOverwrite, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<SaveResult> SaveMetadataAsync(string directoryPath, IEnumerable<MetadataSection> documents, MetadataFileNamingConvention namingConvention, bool overwrite, CancellationToken cancellationToken)
        {
            var metadataDocumentSaver = new MetadataDocumentSaver(directoryPath, documents, namingConvention);
            var mainWsdl = await AsyncHelper.RunAsync(() => metadataDocumentSaver.SaveMetadata(overwrite), cancellationToken).ConfigureAwait(false);

            return new SaveResult
            {
                WsdlFilePath = mainWsdl?.FilePath,
                MetadataFiles = metadataDocumentSaver.MetadataFiles.Select(mf => mf.FilePath),
                DocumentSaveErrors = metadataDocumentSaver.UnresolvedReferences.Distinct().OrderBy(ur => ur.Uri).Select(ur => string.Format(CultureInfo.CurrentCulture, MetadataResources.ErrUnableToResolveSchemaReferenceFormat, ur.Uri))
            };
        }

        private MetadataFileInfo SaveMetadata(bool overwrite)
        {
            if (!overwrite)
            {
                var fileInfo = this.MetadataFiles.FirstOrDefault(fi => File.Exists(fi.FilePath));
                if (fileInfo != null)
                {
                    throw new IOException(string.Format(CultureInfo.CurrentCulture, MetadataResources.ErrFileAlreadyExistsFormat, fileInfo.FilePath));
                }
            }

            foreach (var mfi in this.MetadataFiles)
            {
                using (XmlNS.XmlWriter xWriter = CreateXmlFile(mfi.FilePath))
                {
                    if (mfi.Write != null)
                    {
                        mfi.Write(xWriter);
                        xWriter.Flush();
                    }
                }
            }

            return GetMainWsdl();
        }

        private void AddMetadataFiles(IEnumerable<MetadataSection> documents)
        {
            if (documents == null)
            {
                throw new ArgumentNullException(nameof(documents));
            }

            // prepopulate schema/wsdl includes/imports so references can be resolved/updated when resolving document paths.
            foreach (var doc in documents)
            {
                if (!AddUnresolvedSchemaRefs(doc.Metadata as XmlNS.Schema.XmlSchema))
                {
                    AddUnresolvedWsdlRefs(doc.Metadata as WsdlNS.ServiceDescription);
                }
            }

            // compute document paths.
            foreach (var doc in documents)
            {
                if (AddWsdl(doc.Metadata as WsdlNS.ServiceDescription) == null)
                {
                    if (AddSchema(doc.Metadata as XmlNS.Schema.XmlSchema) == null)
                    {
                        if (AddXmlDocument(doc.Metadata as XmlNS.XmlElement, doc.Dialect) == null)
                        {
#if DEBUG
                            string typeName = doc.Metadata.GetType().ToString();
                            Debug.Fail("Unknown metadata found: " + typeName);
#endif
                        }
                    }
                }
            }

            for (int idx = UnresolvedReferences.Count - 1; idx >= 0; idx--)
            {
                var unresolvedRef = UnresolvedReferences[idx];

                if (unresolvedRef.Namespace != null)
                {
                    // remove namespace-only schema references as they are still valid
                    UnresolvedReferences.RemoveAt(idx);
                }
                else
                {
                    // remove schema references for which multiple files are resolved (wildcards).
                    var location = unresolvedRef.WsdlImport != null ? unresolvedRef.WsdlImport.Location : unresolvedRef.SchemaExternal?.SchemaLocation;

                    if (MetadataFileNameManager.TryCreateUri(location, out Uri locationUri) && MetadataFileNameManager.TryResolveFiles(locationUri.LocalPath, out var files))
                    {
                        var missingRefs = files.Where(file => !this.MetadataFiles.Any(metaFile => MetadataFileNameManager.UriEqual(file.FullName, metaFile.SourceUri)));
                        if (missingRefs.Count() == 0)
                        {
                            var updatedLocation = Path.Combine(this.DirectoryPath, Path.GetFileName(location));
                            if (unresolvedRef.WsdlImport != null)
                            {
                                unresolvedRef.WsdlImport.Location = updatedLocation;
                            }
                            else
                            {
                                unresolvedRef.SchemaExternal.SchemaLocation = updatedLocation;
                            }
                            UnresolvedReferences.Remove(unresolvedRef);
                        }
                    }
                }
            }
        }

        private bool AddUnresolvedWsdlRefs(WsdlNS.ServiceDescription wsdl)
        {
            if (wsdl != null)
            {
                foreach (WsdlNS.Import import in wsdl.Imports)
                {
                    if (!string.IsNullOrEmpty(import.Location) && !this.UnresolvedReferences.Any(r => r.WsdlImport == import))
                    {
                        import.Location = MetadataFileNameManager.GetComposedUri(wsdl.RetrievalUrl, import.Location);
                        UnresolvedReferences.Add(new UnresolvedUri { WsdlImport = import, Wsdl = wsdl });
                    }
                }

                foreach (XmlNS.Schema.XmlSchema schema in wsdl.Types.Schemas)
                {
                    AddUnresolvedSchemaRefs(schema);
                }
                return true;
            }
            return false;
        }

        private bool AddUnresolvedSchemaRefs(XmlNS.Schema.XmlSchema schema)
        {
            if (schema != null)
            {
                foreach (XmlNS.Schema.XmlSchemaExternal schemaExternal in schema.Includes)
                {
                    if (!this.UnresolvedReferences.Any(r => r.SchemaExternal == schemaExternal))
                    {
                        if (!string.IsNullOrEmpty(schemaExternal.SchemaLocation))
                        {
                            schemaExternal.SchemaLocation = MetadataFileNameManager.GetComposedUri(schema.SourceUri, schemaExternal.SchemaLocation);
                            UnresolvedReferences.Add(new UnresolvedUri { Schema = schema, SchemaExternal = schemaExternal });
                        }
                        else if (schemaExternal.Schema == null)
                        {
                            // the MetadataExchangeClient when using MEX protocol downloads wsdl-embedded schemas separately,
                            // need to gather namespace-only imports (which are valid w/o any schema) to be able to connect 
                            // the docs if it is the case.
                            var schemaImport = schemaExternal as XmlNS.Schema.XmlSchemaImport;
                            if (schemaImport != null && !string.IsNullOrEmpty(schemaImport.Namespace))
                            {
                                UnresolvedReferences.Add(new UnresolvedUri { Schema = schema, SchemaExternal = schemaExternal, Namespace = schemaImport.Namespace });
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private MetadataFileInfo AddWsdl(WsdlNS.ServiceDescription wsdl)
        {
            MetadataFileInfo metadataFileInfo = null;
            if (wsdl != null && !this.MetadataFiles.Any(mi => mi.Metadata == wsdl))
            {
                var sourceUrl = wsdl.RetrievalUrl;
                var filePath = AddFilePath(wsdl.RetrievalUrl, wsdl.TargetNamespace, ".wsdl");

                wsdl.RetrievalUrl = Path.GetFileName(filePath);
                metadataFileInfo = new WsdlFileInfo(wsdl, filePath, sourceUrl, wsdl.Write);
                this.MetadataFiles.Add(metadataFileInfo);

                var unresolvedRefs = UnresolvedReferences.Where(u => MetadataFileNameManager.UriEqual(u.WsdlImport?.Location, sourceUrl)).ToList();
                foreach (var unresolvedRef in unresolvedRefs)
                {
                    unresolvedRef.WsdlImport.Location = wsdl.RetrievalUrl;
                    UnresolvedReferences.Remove(unresolvedRef);
                }
            }
            return metadataFileInfo;
        }

        private MetadataFileInfo AddSchema(XmlSchema schema)
        {
            MetadataFileInfo metadataFileInfo = null;
            if (schema != null && !this.MetadataFiles.Any(mi => mi.Metadata == schema) /*&& schema.Items.Count > 0*/)
            {
                var sourceUrl = schema.SourceUri;
                var filePath = AddFilePath(schema.SourceUri, schema.TargetNamespace, ".xsd");

                schema.SourceUri = Path.GetFileName(filePath);
                metadataFileInfo = new SchemaFileInfo(schema, filePath, sourceUrl, schema.Write);
                this.MetadataFiles.Add(metadataFileInfo);

                var unresolvedRefs = UnresolvedReferences.Where(u =>
                       (MetadataFileNameManager.UriEqual(u.SchemaExternal?.SchemaLocation, sourceUrl) ||
                       (!string.IsNullOrEmpty(u.Namespace) && u.Namespace == schema.TargetNamespace))).ToList();

                foreach (var unresolvedRef in unresolvedRefs)
                {
                    unresolvedRef.SchemaExternal.SchemaLocation = schema.SourceUri;
                    UnresolvedReferences.Remove(unresolvedRef);
                }
            }
            return metadataFileInfo;
        }

        private MetadataFileInfo AddXmlDocument(XmlNS.XmlElement document, string dialect)
        {
            MetadataFileInfo metadataFileInfo = null;
            if (document != null && !this.MetadataFiles.Any(mi => mi.Metadata == document))
            {
                var fileName = GetXmlElementFilename(document, dialect);
                var filePath = FileNameMgr.AddFileName(this.DirectoryPath, fileName, ".xml");
                metadataFileInfo = new MetadataFileInfo(document, filePath, null, document.WriteTo);
                this.MetadataFiles.Add(metadataFileInfo);
            }
            return metadataFileInfo;
        }

        private WsdlFileInfo GetMainWsdl()
        {
            var importedWsdl = new List<string>();
            var wsdlFiles = this.MetadataFiles.OfType<WsdlFileInfo>();

            // record imported wsld files to be able to identify the core wsdl file.
            foreach (var wsdl in wsdlFiles.Select(f => f.Wsdl))
            {
                foreach (WsdlNS.Import import in wsdl.Imports)
                {
                    var filePath = Path.Combine(this.DirectoryPath, import.Location);
                    importedWsdl.Add(filePath);
                }
            }

            var mainWsdlFile = wsdlFiles.Where(w => !importedWsdl.Any(i => MetadataFileNameManager.UriEqual(i, w.FilePath))).FirstOrDefault();
            if (mainWsdlFile == null)
            {
                // this may be the case of docs with circular dependencies, this is ok as they are not imported multiple times, select the first one (if any).
                mainWsdlFile = wsdlFiles.FirstOrDefault();
            }

            return mainWsdlFile;
        }

        private string AddFilePath(string location, string targetNamespace, string extension)
        {
            Uri.TryCreate(location, UriKind.Absolute, out Uri uri);

            string filePath = this.NamingConvention == MetadataFileNamingConvention.Namespace || uri == null ?
                this.FileNameMgr.AddFromNamespace(this.DirectoryPath, targetNamespace, extension) :
                this.FileNameMgr.AddFileName(this.DirectoryPath, Path.GetFileName(uri.LocalPath), extension);

            return filePath;
        }

        private XmlNS.XmlWriter CreateXmlFile(string filePath)
        {
            var dirPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var xmlWriterSettings = new XmlNS.XmlWriterSettings()
            {
                Indent = true,
                CheckCharacters = false
            };

            return XmlNS.XmlWriter.Create(filePath, xmlWriterSettings);
        }

        private static string GetXmlElementFilename(XmlNS.XmlElement doc, string dialect)
        {
            string filename;

            if (dialect == MetadataSection.PolicyDialect)
            {
                filename = GetPolicyFilename(doc);
            }
            else
            {
                filename = defaultMetadataFileName;
            }

            return filename;
        }

        private static string GetPolicyFilename(XmlNS.XmlElement policyElement)
        {
            string id = null;

            if (policyElement.NamespaceURI == MetadataConstants.WSPolicy.NamespaceUri && policyElement.LocalName == MetadataConstants.WSPolicy.Elements.Policy)
            {
                id = policyElement.GetAttribute(MetadataConstants.Wsu.Attributes.Id, MetadataConstants.Wsu.NamespaceUri);
                if (id == null)
                {
                    id = policyElement.GetAttribute(MetadataConstants.Xml.Attributes.Id, MetadataConstants.Xml.NamespaceUri);
                }
                if (!string.IsNullOrEmpty(id))
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}", id);
                }
            }

            return defaultPolicyFileName;
        }

        #region Nested types

        public class SaveResult
        {
            public string WsdlFilePath { get; internal set; }
            public IEnumerable<string> MetadataFiles { get; internal set; }
            public IEnumerable<string> DocumentSaveErrors { get; internal set; }
        }

        private class UnresolvedUri
        {
            public XmlNS.Schema.XmlSchema Schema;
            public XmlNS.Schema.XmlSchemaExternal SchemaExternal;
            public WsdlNS.ServiceDescription Wsdl;
            public WsdlNS.Import WsdlImport;
            public string Namespace;

            public string Uri
            {
                get
                {
                    if (SchemaExternal != null)
                    {
                        return SchemaExternal.SchemaLocation;
                    }
                    if (WsdlImport != null)
                    {
                        return WsdlImport.Location;
                    }
                    return null;
                }
            }

            public override string ToString()
            {
                return string.IsNullOrEmpty(this.Uri) ? base.ToString() : this.Uri;
            }

            public override bool Equals(object obj)
            {
                UnresolvedUri other = obj as UnresolvedUri;
                return other != null && MetadataFileNameManager.UriEqual(this.Uri, other.Uri);
            }

            public override int GetHashCode()
            {
                return this.ToString().GetHashCode();
            }
        }

        #endregion
    }
}
