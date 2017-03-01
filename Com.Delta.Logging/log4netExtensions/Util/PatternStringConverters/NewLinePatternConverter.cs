using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Core;
using log4net.Util;

namespace log4netExtensions.Util.PatternStringConverters
{
    internal sealed class NewLinePatternConverter : LiteralPatternConverter, IOptionHandler
    {
        #region IOptionHandler Membri di

        public void ActivateOptions()
        {
            if (string.Compare(Option, "DOS", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                Option = "\r\n";
            }
            else if (string.Compare(Option, "UNIX", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                Option = "\n";
            }
            else
            {
                Option = SystemInfo.NewLine;
            }
        }

        #endregion
    }
}
