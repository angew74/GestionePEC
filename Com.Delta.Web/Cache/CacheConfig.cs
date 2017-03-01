using System.Configuration;

namespace Com.Delta.Web.Cache
{
    /// <summary>
    /// The Class that will have the XML config file data loaded into it via the configuration Manager.
    /// </summary>

    public class CacheConfig : ConfigurationSection
    {
        /// <summary>
        /// The value of the property here "Folders" needs to match that of the config file section
        /// </summary>
        [ConfigurationProperty("Folders")]
        public FoldersCollection FolderItems
        {
            get { return ((FoldersCollection)(base["Folders"])); }
        }
    }

    /// <summary>
    /// The collection class that will store the list of each element/item that
    ///        is returned back from the configuration manager.
    /// </summary>
    [ConfigurationCollection(typeof(FolderElement))]
    public class FoldersCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FolderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FolderElement)(element)).FolderType;
        }

        public FolderElement this[int idx]
        {
            get
            {
                return (FolderElement)BaseGet(idx);
            }
        }
    }

    /// <summary>
    /// The class that holds onto each element returned by the configuration manager.
    /// </summary>
    public class FolderElement : ConfigurationElement
    {

        [ConfigurationProperty("folderType", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string FolderType
        {
            get
            {
                return ((string)(base["folderType"]));
            }

            set
            {
                base["folderType"] = value;
            }
        }

        [ConfigurationProperty("path", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Path
        {
            get
            {
                return ((string)(base["path"]));
            }
            set
            {
                base["path"] = value;
            }
        }
    }
}