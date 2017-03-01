using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Delta.Data
{

    public interface IDaoBaseSession<TI>
    {
        void Session_Init();
        bool Session_isActive();
        void Dispose();

        Type TransactionRootElement
        {
            get;
        }

        bool TransactionModeOn
        {
            get;
        }

        void StartTransaction(Type requestor);

        void EndTransaction(Type requestor);

        void RollBackTransaction(Type requestor);

        TI DaoImpl
        {
            get;
        }
    }
}
