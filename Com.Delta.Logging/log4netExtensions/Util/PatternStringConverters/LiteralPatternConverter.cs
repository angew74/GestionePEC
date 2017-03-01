using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Util;

namespace log4netExtensions.Util.PatternStringConverters
{
    internal class LiteralPatternConverter : PatternConverter
    {
        public override PatternConverter SetNext(PatternConverter patternConverter)
        {
            LiteralPatternConverter literalPc = patternConverter as LiteralPatternConverter;
            if (literalPc != null)
            {
                Option += literalPc.Option;
                return this;
            }
            return base.SetNext(patternConverter);
        }

        public override void Format(System.IO.TextWriter writer, object state)
        {
            writer.Write(Option);
        }

        protected override void Convert(System.IO.TextWriter writer, object state)
        {
            throw new InvalidOperationException("Should never get here because of the overridden Format method");
        }
    }
}
