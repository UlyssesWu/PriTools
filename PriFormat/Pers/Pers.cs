using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PriFormat.Pers
{
    class Pers
    {
        public List<GifPart> GifParts = new List<GifPart>();
        public Version Version;
        public uint XmlLength;
        public byte[] Data { get; internal set; }
        public byte[] XmlData { get; internal set; }
        private XmlDocument _xml = null;

        public XmlDocument Xml
        {
            get
            {
                if (_xml != null) return _xml;
                if (XmlData == null) return null;
                _xml = new XmlDocument();
                _xml.Load(new MemoryStream(XmlData));
                return _xml;
            }
        }

        internal void ParseXml()
        {
            if (Xml == null)
            {
                return;
            }
            var parts = Xml.SelectSingleNode("Xml").SelectSingleNode("Parts");
            var gifs = parts.ChildNodes;
            foreach (XmlNode gif in gifs)
            {
                try
                {
                    GifPart gp = new GifPart();
                    gp.Offset = int.Parse(gif["Offset"].InnerText);
                    gp.Size = int.Parse(gif["Size"].InnerText);
                    gp.Frames = int.Parse(gif["Frames"].InnerText);
                    GifParts.Add(gp);
                }
                catch (NullReferenceException)
                {
                    continue;
                    throw;
                }

            }
        }

        public static bool IsPers(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4)
            {
                return false;
            }
            return bytes[0] == 'P' && bytes[1] == 'E' && bytes[2] == 'R' && bytes[3] == 'S';
        }

        public byte[] Save()
        {
            throw new NotImplementedException();
        }

        public static Pers Read(byte[] bytes)
        {
            Pers persRaw = new Pers();
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                BinaryReader br = new BinaryReader(ms);
                if (br.ReadChars(4).ToRealString() != "PERS")
                {
                    throw new FormatException("not a correct pers file");
                }
                persRaw.Version = new Version(br.ReadUInt16(), br.ReadUInt16());
                persRaw.XmlLength = br.ReadUInt32();
                persRaw.XmlData = br.ReadBytes((int)persRaw.XmlLength);
                var dataStart = br.BaseStream.Position;
                persRaw.Data = br.ReadBytes((int)(bytes.Length - persRaw.XmlLength - 12));
                persRaw.ParseXml();
                int index = 0;
                foreach (var gifPart in persRaw.GifParts)
                {
                    gifPart.Name = index.ToString();
                    index++;
                    br.BaseStream.Seek(dataStart, SeekOrigin.Begin);
                    br.BaseStream.Seek(gifPart.Offset, SeekOrigin.Current);
                    gifPart.Data = br.ReadBytes(gifPart.Size);
                }
            }
            return persRaw;
        }
    }
}
