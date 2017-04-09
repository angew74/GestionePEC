using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model.Wrappers;

namespace SendMail.BusinessEF.Contracts
{
    public interface IReferralTypeService
    {
        List<BaseResultItem> LoadAllreferralTypes();
    }
}
