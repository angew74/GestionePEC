using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Delta.Data;

namespace Com.Delta.Data
{
    public interface IDao<T>
    {
        T getDaoFactory(StoreType type);
    }
}
