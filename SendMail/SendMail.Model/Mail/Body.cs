using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SendMail.Model
{
   
        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
        [System.SerializableAttribute()]

        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class Body
        {
            private BodyHeader header = new BodyHeader();
            private BodyFooter footer = new BodyFooter();
            private List<BodyChunk> chunks = new List<BodyChunk>();

            [System.Xml.Serialization.XmlElementAttribute("Header", typeof(BodyHeader), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public BodyHeader Header
            {
                get { return header; }
                set { header = value; }
            }

            [System.Xml.Serialization.XmlElementAttribute("Footer", typeof(BodyFooter), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public BodyFooter Footer
            {
                get { return footer; }
                set { footer = value; }
            }

            [System.Xml.Serialization.XmlElementAttribute("Chunk", typeof(BodyChunk), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public List<BodyChunk> Chunks
            {
                get
                {
                    return this.chunks;
                }
                set
                {
                    this.chunks = value;
                }
            }

            public string getAsString()
            {
                throw new System.NotImplementedException();
            }

            public string getAsHtml()
            {
                string response = "<p>";
                if (Header != null)
                    response = response + Header.getAsHtml();
                foreach (BodyChunk chunk in Chunks)
                {
                    response = response + chunk.getAsHtml();
                }
                if (Footer != null)
                    response = response + Footer.getAsHtml();
                response = response + "</p>";
                return response;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="dow"></param>
            /// <param name="del"></param>
            /// <returns></returns>
            public string getAsHtml(bool dow, bool del)
            {
                string response = "<p>";
                if (Header != null)
                    response = response + Header.getAsHtml();
                foreach (BodyChunk chunk in Chunks)
                {
                    response = response + chunk.getAsHtml(dow, del);
                }
                if (Footer != null)
                    response = response + Footer.getAsHtml();
                response = response + "</p>";
                return response;
            }

            public Body BodySeserialize(string bodyString)
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(bodyString);
                XmlSerializer xs = new XmlSerializer(typeof(Body));
                UTF8Encoding encoding = new UTF8Encoding();
                Byte[] byteArray = encoding.GetBytes(doc.OuterXml);
                MemoryStream memoryStream = new MemoryStream(byteArray);
                return (Body)xs.Deserialize(memoryStream);
            }

            public string BodyDeserialize(Body body)
            {

                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = null;
                xs = new XmlSerializer(typeof(Body));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                xs.Serialize(xmlTextWriter, body);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                UTF8Encoding encoding = new UTF8Encoding();
                String XmlizedString = encoding.GetString(memoryStream.ToArray());
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(XmlizedString.Substring(1));
                return doc.InnerXml;
            }


            #region "Metodi helpers"

            /// <summary>
            /// Restituisce il numero di Chunck elaborati per le Pratiche.
            /// </summary>
            /// <returns></returns>
            public int ChunkPraticheCount()
            {
                return chunks.Count(q => !string.IsNullOrEmpty(q.PraticaId));
            }

            /// <summary>
            /// Restituisce il numero di Chunck elaborati per una determinata Pratica.
            /// </summary>
            /// <param name="praticaId">id della pratica</param>
            /// <returns></returns>
            public int ChunkPraticaCount(decimal praticaId)
            {
                return chunks.Count(q => q.PraticaId == praticaId.ToString());
            }

            /// <summary>
            /// Restituisce il numero di Chunck elaborati per una determinata Pratica.
            /// </summary>
            /// <returns></returns>
            public decimal ChunkImporti()
            {
                decimal result = default(decimal);
                foreach (BodyChunk item in chunks)
                {
                    result += ModelHelper.ParseDecimalPoint(item.Importo);

                }
                return result;
            }

            #endregion

        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
        [System.SerializableAttribute()]

        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class BodyChunk
        {
            public BodyChunk()
            {

            }

            public BodyChunk(string praticaId, string documentoId)
            {
                id = this.GetHashCode().ToString();
                this.praticaId = praticaId;
                this.documentoId = documentoId;
            }

            private string id;

            private string praticaId;

            public string PraticaId
            {
                get { return praticaId; }
                set { praticaId = value; }
            }

            private string documentoId;

            public string DocumentoId
            {
                get { return documentoId; }
                set { documentoId = value; }
            }

            private string importo;

            public string Importo
            {
                get { return importo; }
                set { importo = value; }
            }

            public string Id
            {
                get { return id; }
                set { id = value; }
            }

            private string titleField;

            private List<string> lineField = new List<string>();

            private string positionField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string Title
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

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Line", IsNullable = true)]
            public List<string> Line
            {
                get
                {
                    return this.lineField;
                }
                set
                {
                    this.lineField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Position
            {
                get
                {
                    return this.positionField;
                }
                set
                {
                    this.positionField = value;
                }
            }

            public string getAsString()
            {
                string response = "";
                response += String.Join("\r\n", lineField.ToArray());
                return response;
            }

            public string getAsHtml()
            {
                string response = "<p>";
                if (!string.IsNullOrEmpty(titleField))
                    response = response + "<H4>" + titleField + "</H4>";
                if (!string.IsNullOrEmpty(documentoId))
                {
                    response = response + "<a href='#' onclick='DownloadDocumento(\"" + this.documentoId + "\");return false;'>Download</a> ";
                    response = response + "<a href='#' onclick='DeleteDocumento(\"" + this.documentoId + "\",\"" + this.id + "\");'>Cancella</a> ";
                }
                foreach (string line in lineField)
                {
                    response = response + "<span>" + line + "</span><br/>";
                }
                response = response + "</p>";
                return response;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="dow"></param>
            /// <param name="del"></param>
            /// <returns></returns>
            public string getAsHtml(bool dow, bool del)
            {
                string response = "<p>";
                if (!string.IsNullOrEmpty(titleField))
                    response = response + "<H4>" + titleField + "</H4>";
                if (!string.IsNullOrEmpty(documentoId) && dow)
                    response = response + "<a href='#' onclick='DownloadDocumento(\"" + this.documentoId + "\");return false;'>Download</a> ";
                if (del)
                    response = response + "<a href='#' onclick='DeleteChunk(\"" + this.Id + "\");'>Cancella</a> ";
                foreach (string line in lineField)
                {
                    response = response + "<span>" + line + "</span><br/>";
                }
                response = response + "</p>";
                return response;
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
        [System.SerializableAttribute()]

        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class BodyFooter
        {

            private string titleField;

            private List<string> lineField = new List<string>();


            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string Title
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

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Line", IsNullable = true)]
            public List<string> Line
            {
                get
                {
                    return this.lineField;
                }
                set
                {
                    this.lineField = value;
                }
            }

            public string getAsString()
            {
                throw new System.NotImplementedException();
            }

            public string getAsHtml()
            {
                string response = "<p>";
                if (!string.IsNullOrEmpty(titleField))
                    response = response + "<H4>" + titleField + "</H4>";
                foreach (string line in lineField)
                {
                    response = response + "<span>" + line + "</span><br/>";
                }
                response = response + "</p>";
                return response;
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
        [System.SerializableAttribute()]

        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class BodyHeader
        {

            private string titleField;

            private List<string> lineField = new List<string>();

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string Title
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

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Line", IsNullable = true)]
            public List<string> Line
            {
                get
                {
                    return this.lineField;
                }
                set
                {
                    this.lineField = value;
                }
            }

            public string getAsString()
            {
                throw new System.NotImplementedException();
            }

            public string getAsHtml()
            {
                string response = "<p>";
                if (!string.IsNullOrEmpty(titleField))
                    response = response + "<H3>" + titleField + "</H3>";
                foreach (string line in lineField)
                {
                    response = response + "<span>" + line + "</span><br/>";
                }
                response = response + "</p>";
                return response;
            }
        }


    }

