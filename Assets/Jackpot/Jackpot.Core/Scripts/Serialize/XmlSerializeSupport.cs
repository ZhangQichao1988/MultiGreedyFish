/**
 * Jackpot
 * Copyright(c) 2014 KLab, Inc. All Rights Reserved.
 * Proprietary and Confidential - This source code is not for redistribution
 * 
 * Subject to the prior written consent of KLab, Inc(Licensor) and its terms and
 * conditions, Licensor grants to you, and you hereby accept nontransferable,
 * nonexclusive limited right to access, obtain, use, copy and/or download
 * a copy of this product only for requirement purposes. You may not rent,
 * lease, loan, time share, sublicense, transfer, make generally available,
 * license, disclose, disseminate, distribute or otherwise make accessible or
 * available this product to any third party without the prior written approval
 * of Licensor. Unauthorized copying of this product, including modifications
 * of this product or programs in which this product has been merged or included
 * with other software products is expressly forbidden.
 */
using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace Jackpot
{
    public static class XmlSerializeSupport
    {
        public static void WriteFromXml<TKey, TValue>(IDictionary<TKey, TValue> dictionary, byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer)){
                var reader = new XmlTextReader(stream);
                reader.ReadStartElement("Dictionary");

                XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
                XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.ReadStartElement("KeyValuePair");

                    reader.ReadStartElement("Key");
                    TKey key = (TKey) keySerializer.Deserialize(reader);
                    reader.ReadEndElement();

                    reader.ReadStartElement("Value");
                    TValue val = (TValue) valueSerializer.Deserialize(reader);
                    reader.ReadEndElement();

                    reader.ReadEndElement();
                    dictionary[key] = val;

                    reader.MoveToContent();
                }

                reader.ReadEndElement();
            }
        }

        public static byte[] ToXml<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new XmlTextWriter(stream, Encoding.UTF8);
                writer.WriteStartElement("Dictionary");

                XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
                XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

                foreach (TKey key in dictionary.Keys)
                {
                    writer.WriteStartElement("KeyValuePair");

                    writer.WriteStartElement("Key");
                    keySerializer.Serialize(writer, key);
                    writer.WriteEndElement();

                    writer.WriteStartElement("Value");
                    valueSerializer.Serialize(writer, dictionary[key]);
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                writer.Flush();

                return stream.GetBuffer();
            }
        }
    }
}
