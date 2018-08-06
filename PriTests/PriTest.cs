using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PriFormat;

namespace PriTests
{
    [TestClass]
    public class PriTest
    {
        [TestMethod]
        public void TestLoadPri()
        {
            var resPath = Path.Combine(Environment.CurrentDirectory, @"..\..\Res");
            var path = Path.Combine(resPath, "Nana.pri");
            var stream = File.OpenRead(path);
            var p = PriFile.Parse(stream);
            foreach (var sec in p.Sections)
            {
                var secId = sec.SectionIdentifier;
                var secLen = sec.SectionLength;
                var secFlags = sec.SectionFlags;
                var flags = sec.Flags;
                var secQualifier = sec.SectionQualifier;
            }

            foreach (var toc in p.TableOfContents)
            {
                var secId = toc.SectionIdentifier;
                var secLen = toc.SectionLength;
                var secFlags = toc.SectionFlags;
                var flags = toc.Flags;
                var secQualifier = toc.SectionQualifier;
            }

            var priFlags = p.PriDescriptorSection.PriFlags;
            foreach (var resourceMapSectionRef in p.PriDescriptorSection.ResourceMapSections)
            {
                ResourceMapSection resourceMapSection = p.GetSectionByRef(resourceMapSectionRef);

                if (resourceMapSection.HierarchicalSchemaReference != null)
                    continue;

                DecisionInfoSection decisionInfoSection = p.GetSectionByRef(resourceMapSection.DecisionInfoSection);

                foreach (var candidateSet in resourceMapSection.CandidateSets.Values)
                {
                    ResourceMapItem item = p.GetResourceMapItemByRef(candidateSet.ResourceMapItem);

                    //Console.WriteLine("  {0}:", item.FullName);

                    foreach (var candidate in candidateSet.Candidates)
                    {
                        string value = null;

                        if (candidate.SourceFile != null)
                            value = string.Format("<external in {0}>", p.GetReferencedFileByRef(candidate.SourceFile.Value).FullName);
                        else
                        {
                            ByteSpan byteSpan;

                            if (candidate.DataItem != null)
                                byteSpan = p.GetDataItemByRef(candidate.DataItem.Value);
                            else
                                byteSpan = candidate.Data.Value;

                            stream.Seek(byteSpan.Offset, SeekOrigin.Begin);

                            byte[] data;

                            using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.Default, true))
                                data = binaryReader.ReadBytes((int)byteSpan.Length);

                            switch (candidate.Type)
                            {
                                case ResourceValueType.AsciiPath:
                                case ResourceValueType.AsciiString:
                                    value = Encoding.ASCII.GetString(data).TrimEnd('\0');
                                    break;
                                case ResourceValueType.Utf8Path:
                                case ResourceValueType.Utf8String:
                                    value = Encoding.UTF8.GetString(data).TrimEnd('\0');
                                    break;
                                case ResourceValueType.Path:
                                case ResourceValueType.String:
                                    value = Encoding.Unicode.GetString(data).TrimEnd('\0');
                                    break;
                                case ResourceValueType.EmbeddedData:
                                    value = string.Format("<{0} bytes>", data.Length);
                                    break;
                            }
                        }

                        QualifierSet qualifierSet = decisionInfoSection.QualifierSets[candidate.QualifierSet];

                        string qualifiers = string.Join(", ", qualifierSet.Qualifiers.Select(q => string.Format("{0}={1}", q.Type, q.Value)));

                        //Console.WriteLine("    Candidate {0}: {1}", qualifiers, value);
                    }
                }
            }
        }
    }
}
