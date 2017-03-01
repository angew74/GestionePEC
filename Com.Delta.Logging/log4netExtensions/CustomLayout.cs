using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Layout;
using System.Collections;
using log4net.Util;
using log4net.Layout.Pattern;
using log4netExtensions.Layout.Pattern;

namespace Com.Delta.Logging.log4netExtensions
{
    public class CustomLayout : LayoutSkeleton
    {
        public const string DEFAULT_CONVERSION_PATTERN = "%message%newline";
        public const string DETAIL_CONVERSION_PATTERN = "%timestamp [%thread] %level %logger %ndc - %message%newline";
        private static Hashtable s_globalRulesRegistry;
        private string m_pattern;
        private PatternConverter m_head;
        private Hashtable m_instanceRulesRegistry = new Hashtable();

        static CustomLayout()
        {
            s_globalRulesRegistry = new Hashtable(15);
            s_globalRulesRegistry.Add("loggingAppCode", typeof(LoggingAppCodePatternConverter));
            s_globalRulesRegistry.Add("logCode", typeof(LogCodePatternConverter));
            s_globalRulesRegistry.Add("freeTextDetails", typeof(FreeTextDetailsPatternConverter));
            s_globalRulesRegistry.Add("uniqueLogID", typeof(UniqueLogIDPatternConverter));
            s_globalRulesRegistry.Add("userID", typeof(UserIDPatternConverter));
            s_globalRulesRegistry.Add("userMail", typeof(UserMailPatternConverter));
            s_globalRulesRegistry.Add("userIP", typeof(UserIPPatternConverter));
            s_globalRulesRegistry.Add("objectID", typeof(ObjectIDPatternConverter));
            s_globalRulesRegistry.Add("objectGroupID", typeof(ObjectGroupIDPatternConverter));
            s_globalRulesRegistry.Add("objectAppID", typeof(ObjectAppIDPatternConverter));
            s_globalRulesRegistry.Add("objectParentcode", typeof(ObjectParentcodePatternConverter));
            s_globalRulesRegistry.Add("passiveobjectID", typeof(PassiveobjectIDPatternConverter));
            s_globalRulesRegistry.Add("passiveobjectGroupID", typeof(PassiveobjectGroupIDPatternConverter));
            s_globalRulesRegistry.Add("passiveparentcodeobjectID", typeof(PassiveparentcodeobjectIDPatternConverter));
            s_globalRulesRegistry.Add("passiveapplicationID", typeof(PassiveapplicationIDPatternConverter));
        }

        public CustomLayout() : this(DEFAULT_CONVERSION_PATTERN) { }

        public CustomLayout(string pattern)
        {
            IgnoresException = true;
            m_pattern = pattern;
            if (m_pattern == null)
            {
                m_pattern = DEFAULT_CONVERSION_PATTERN;
            }
            ActivateOptions();
        }

        public string ConversionPattern
        {
            get { return m_pattern; }
            set { m_pattern = value; }
        }

        virtual protected PatternParser CreatePatternParser(string pattern)
        {
            PatternParser parser = new PatternParser(pattern);
            foreach (DictionaryEntry entry in s_globalRulesRegistry)
            {
                parser.PatternConverters[entry.Key] = entry.Value;
            }
            foreach (DictionaryEntry entry in m_instanceRulesRegistry)
            {
                parser.PatternConverters[entry.Key] = entry.Value;
            }
            return parser;
        }

        public override void ActivateOptions()
        {
            m_head = CreatePatternParser(m_pattern).Parse();

            PatternConverter curConverter = m_head;
            while (curConverter != null)
            {
                PatternLayoutConverter layoutConverter = curConverter as PatternLayoutConverter;
                if (layoutConverter != null)
                {
                    if (!layoutConverter.IgnoresException)
                    {
                        this.IgnoresException = false;
                        break;
                    }
                }
                curConverter = curConverter.Next;
            }
        }

        public override void Format(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }
            PatternConverter c = m_head;
            while (c != null)
            {
                c.Format(writer, loggingEvent);
                c = c.Next;
            }
        }

        public void AddConverter(ConverterInfo converterInfo)
        {
            if (converterInfo == null) throw new ArgumentNullException("converterInfo");
            if(!typeof(PatternConverter).IsAssignableFrom(converterInfo.Type))
                throw new ArgumentException("The converter type specified [" + converterInfo.Type + "] must be a subclass of log4net.Util.PatternConverter", "converterInfo");
            m_instanceRulesRegistry[converterInfo.Name] = converterInfo;
        }

        public void AddConverter(string name, Type type)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (type == null) throw new ArgumentNullException("type");

            ConverterInfo converterInfo = new ConverterInfo();
            converterInfo.Name = name;
            converterInfo.Type = type;
            AddConverter(converterInfo);
        }

        public sealed class ConverterInfo
        {
            private string m_name;
            private Type m_type;

            public ConverterInfo() { }

            public string Name
            {
                get { return m_name; }
                set { m_name = value; }
            }

            public Type Type
            {
                get { return m_type; }
                set { m_type = value; }
            }
        }
    }
}
