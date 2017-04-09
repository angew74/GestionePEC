using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SendMail.BusinessEF.Base
{
    public class BaseSingletonService<T> where T : class, new() 
    {   
 
        static readonly T instance=new T();

        static BaseSingletonService()
        {
        }

        protected BaseSingletonService()
        {
        
        }

        public static T Instance
        {
            get { return instance; }
        }  
    }
}
