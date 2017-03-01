using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml;
using Com.Delta.PrintManager.Engine;
using System.Data;
using System.Web;

namespace Com.Delta.PrintManager
{
    public abstract class Print
    {
        protected ReportDocument reportDoc;
        protected Hashtable parameters;
        protected List<DataTable> tableCollection;

        public abstract byte[] Invoke();
    }
}
