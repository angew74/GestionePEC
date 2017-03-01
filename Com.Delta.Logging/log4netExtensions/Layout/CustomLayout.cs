using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Layout;
using System.Collections;
using log4net.Util;
using log4net.Layout.Pattern;
using log4netExtensions.Layout.Pattern;
using log4netExtensions.Util.PatternStringConverters;

namespace log4netExtensions.Layout
{
    /// <summary>
    /// Layout custom per log su oracle db
    /// </summary>
    public class CustomLayout : LayoutSkeleton
    {
        /// <summary>
        /// The defaul t_ conversio n_ pattern
        /// </summary>
        public const string DEFAULT_CONVERSION_PATTERN = "%loggingAppCode %logCode %message%newline";
        /// <summary>
        /// The detai l_ conversio n_ pattern
        /// </summary>
        public const string DETAIL_CONVERSION_PATTERN = "%loggingAppCode %logCode %freeTextDetails %message%newline";
        private static Hashtable s_globalRulesRegistry;
        private string m_pattern;
        private PatternConverter m_head;
        private Hashtable m_instanceRulesRegistry = new Hashtable();

        static CustomLayout()
        {
            s_globalRulesRegistry = new Hashtable(16);
            s_globalRulesRegistry.Add("literal", typeof(LiteralPatternConverter));
            s_globalRulesRegistry.Add("newline", typeof(NewLinePatternConverter));
            s_globalRulesRegistry.Add("n", typeof(NewLinePatternConverter));
            s_globalRulesRegistry.Add("message", typeof(MessagePatternConverter));
            s_globalRulesRegistry.Add("m", typeof(MessagePatternConverter));
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
            s_globalRulesRegistry.Add("module", typeof(ModulePatternConverter));
            s_globalRulesRegistry.Add("function", typeof(FunctionPatternConverter));
            s_globalRulesRegistry.Add("action", typeof(ActionPatternConverter));
            s_globalRulesRegistry.Add("enanchedInfos", typeof(EnanchedInfoPatternConverter));
            s_globalRulesRegistry.Add("mobjectID", typeof(MObjectIdPaetternConverter));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomLayout"/> class.
        /// </summary>
        public CustomLayout() : this(DEFAULT_CONVERSION_PATTERN) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomLayout"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
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

        /// <summary>
        /// Gets or sets the conversion pattern.
        /// </summary>
        /// <value>
        /// The conversion pattern.
        /// </value>
        public string ConversionPattern
        {
            get { return m_pattern; }
            set { m_pattern = value; }
        }

        /// <summary>
        /// Creates the pattern parser.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        virtual protected PatternParser CreatePatternParser(string pattern)
        {
            PatternParser parser = new PatternParser(pattern);
            foreach (DictionaryEntry entry in s_globalRulesRegistry)
            {
                log4net.Util.ConverterInfo converterInfo = new log4net.Util.ConverterInfo();
                converterInfo.Name = (string)entry.Key;
                converterInfo.Type = (Type)entry.Value;
                parser.PatternConverters[entry.Key] = converterInfo;                
                //parser.PatternConverters[entry.Key] = entry.Value;
            }
            foreach (DictionaryEntry entry in m_instanceRulesRegistry)
            {
                parser.PatternConverters[entry.Key] = entry.Value;
            }
            return parser;
        }

        /// <summary>
        /// Activates the options.
        /// </summary>
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

        /// <summary>
        /// Formats the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="loggingEvent">The logging event.</param>
        /// <exception cref="System.ArgumentNullException">writer
        /// or
        /// loggingEvent</exception>
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

        /// <summary>
        /// Adds the converter.
        /// </summary>
        /// <param name="converterInfo">The converter information.</param>
        /// <exception cref="System.ArgumentNullException">converterInfo</exception>
        /// <exception cref="System.ArgumentException">The converter type specified [ + converterInfo.Type + ] must be a subclass of log4net.Util.PatternConverter;converterInfo</exception>
        public void AddConverter(ConverterInfo converterInfo)
        {
            if (converterInfo == null) throw new ArgumentNullException("converterInfo");
            if(!typeof(PatternConverter).IsAssignableFrom(converterInfo.Type))
                throw new ArgumentException("The converter type specified [" + converterInfo.Type + "] must be a subclass of log4net.Util.PatternConverter", "converterInfo");
            m_instanceRulesRegistry[converterInfo.Name] = converterInfo;
        }

        /// <summary>
        /// Adds the converter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="System.ArgumentNullException">
        /// name
        /// or
        /// type
        /// </exception>
        public void AddConverter(string name, Type type)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (type == null) throw new ArgumentNullException("type");

            ConverterInfo converterInfo = new ConverterInfo();
            converterInfo.Name = name;
            converterInfo.Type = type;
            AddConverter(converterInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class ConverterInfo
        {
            private string m_name;
            private Type m_type;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConverterInfo"/> class.
            /// </summary>
            public ConverterInfo() { }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public string Name
            {
                get { return m_name; }
                set { m_name = value; }
            }

            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>
            /// The type.
            /// </value>
            public Type Type
            {
                get { return m_type; }
                set { m_type = value; }
            }
        }
    }
}
