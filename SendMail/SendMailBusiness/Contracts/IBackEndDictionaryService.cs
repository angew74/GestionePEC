using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model;

namespace SendMail.Business.Contracts
{
    public interface IBackEndDictionaryService
    {
        IList<BackEndRefCode> GetAll(bool refresh);
        BackEndRefCode Load(decimal id);
        BackEndRefCode Update(BackEndRefCode e);
        BackEndRefCode insertTitolo(BackEndRefCode e);
        void deleteTitolo(decimal id);

        BackEndRefCode GetByCode(string idCodeBackEnd);
        List<BackEndRefCode> GetByDescr(string descrBackEnd);
        void Insert(BackEndRefCode entity);
        void Delete(decimal id);
    }
}
