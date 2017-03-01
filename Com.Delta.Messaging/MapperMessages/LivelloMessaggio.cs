using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Delta.Messaging.MapperMessages
{
    [Serializable]
    public enum LivelloMessaggio
    {   
        CRITICAL =-1,
        ERROR = 0,
        WARNING = 1,
        INFO = 2,
        APPLICATION = 3,
        MAPPER_ERROR=4,
        DETAILS=5,
        OK = 6 
    }
}
