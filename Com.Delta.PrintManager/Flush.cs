using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Web;

namespace Com.Delta.PrintManager
{
    public abstract class Flush
    {
        protected String pru;
        protected String url;
        protected Hashtable parameters;
        protected List<DataTable> tableCollection;
        protected HttpResponse response;

        public abstract void Invoke();
    }
}
