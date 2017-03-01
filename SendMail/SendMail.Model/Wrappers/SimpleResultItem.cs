using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Model.Wrappers
{
    [Serializable()]
    public class BaseResultItem
    {
        public BaseResultItem()
        {

        }

        public BaseResultItem(string text, string value, string description, string subType, string Source)
        {
            this.Description = description;
            this.Text = text;
            this.SubType = subType;
            this.Value = value;
        }

        [DatabaseField("VALUE")]
        public virtual String Value
        { get; set; }
        [DatabaseField("TEXT")]
        public virtual String Text
        { get; set; }
        [DatabaseField("DESCR")]
        public virtual String Description
        { get; set; }
        [DatabaseField("SUBTYPE")]
        public virtual String SubType
        { get; set; }

        [DatabaseField("SOURCE")]
        public virtual String Source
        { get; set; }
    }

    public class SimpleResultItem : BaseResultItem
    {
        public SimpleResultItem()
            : base()
        { }

        public SimpleResultItem(string text, string value, string description, string subType, string source, Int64 searchScore)
            : base(text, value, description, subType, source)
        {
            this.SearchScore = searchScore;
        }

        [DatabaseField("SCORE")]
        public virtual Int64 SearchScore
        { get; set; }
    }

    public class SimpleTreeItem : BaseResultItem
    {
        public SimpleTreeItem()
            : base()
        { this.Padre = ""; }

        public SimpleTreeItem(string text, string value, string description, string subType, string source, string path, string padre)
            : base(text, value, description, subType, source)
        {
            this.Path = path;
            if (string.IsNullOrEmpty(padre))
                padre = "";
            else
                this.Padre = padre;
        }

        [DatabaseField("PATH")]
        public virtual String Path
        { get; set; }

        [DatabaseField("PADRE")]
        public virtual String Padre
        { get; set; }

        //proprietà per la gestione di alberi provenienti da sorgenti dati mulitple

        public virtual string ExtendedPath
        {
            get { return base.Source + "@" + (Path ?? "").Replace(".", "." + base.Source + "@"); }
        }

        public virtual string ExtendedValue
        {
            get { return base.Source + "@" + Value; }
        }

        public virtual string ExtendedPadre
        {
            get { return base.Source + "@" + Padre; }
        }
    }
}
