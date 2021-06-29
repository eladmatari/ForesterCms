using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ShadowCopy.Logic
{
    public class SiteWatcherConfigSection : ConfigurationSection
    {

        [ConfigurationProperty("watchers")]
        public WatcherElementCollection Watchers
        {
            get { return (WatcherElementCollection)this["watchers"]; }
        }
    }

    [ConfigurationCollection(typeof(WatcherElement))]
    public class WatcherElementCollection : ConfigurationElementCollection
    {
        public WatcherElement this[int index]
        {
            get { return (WatcherElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new WatcherElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WatcherElement)element).Name;
        }
    }

    public class WatcherElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("appPool", IsRequired = false)]
        public string AppPool
        {
            get { return (string)this["appPool"]; }
            set { this["appPool"] = value; }
        }

        [ConfigurationProperty("source", IsRequired = true)]
        public string Source
        {
            get { return (string)this["source"]; }
            set { this["source"] = value; }
        }

        [ConfigurationProperty("sourceProj", IsRequired = false)]
        public string SourceProj
        {
            get { return (string)this["sourceProj"]; }
            set { this["sourceProj"] = value; }
        }

        [ConfigurationProperty("folder1", IsRequired = true)]
        public string Folder1
        {
            get { return (string)this["folder1"]; }
            set { this["folder1"] = value; }
        }

        [ConfigurationProperty("folder2", IsRequired = true)]
        public string Folder2
        {
            get { return (string)this["folder2"]; }
            set { this["folder2"] = value; }
        }

        [ConfigurationProperty("currentFolder", IsRequired = false)]
        public string CurrentFolder
        {
            get { return (string)this["currentFolder"]; }
            set { this["currentFolder"] = value; }
        }

        [ConfigurationProperty("site", IsRequired = false)]
        public string Site
        {
            get { return (string)this["site"]; }
            set { this["site"] = value; }
        }

        [ConfigurationProperty("applicationPath", IsRequired = false)]
        public string ApplicationPath
        {
            get { return (string)this["applicationPath"]; }
            set { this["applicationPath"] = value; }
        }

        [ConfigurationProperty("filter", IsRequired = false)]
        public string Filter
        {
            get { return (string)this["filter"]; }
            set { this["filter"] = value; }
        }

        [ConfigurationProperty("ignores", IsRequired = false)]
        public string Ignores
        {
            get { return (string)this["ignores"]; }
            set { this["ignores"] = value; }
        }

        private List<string> _ignoresArr;
        public List<string> IgnoresArr
        {
            get
            {
                if (_ignoresArr == null)
                    _ignoresArr = (Ignores ?? "").Split(',').Select(i => i.Trim()).Where(i => i.Length > 0).ToList();

                return _ignoresArr;
            }
        }

        [ConfigurationProperty("copyOnlys", IsRequired = false)]
        public string CopyOnlys
        {
            get { return (string)this["copyOnlys"]; }
            set { this["copyOnlys"] = value; }
        }

        private List<string> _copyOnlysArr;
        public List<string> CopyOnlysArr
        {
            get
            {
                if (_copyOnlysArr == null)
                    _copyOnlysArr = (CopyOnlys ?? "").Split(',').Select(i => i.Trim()).Where(i => i.Length > 0).ToList();

                return _copyOnlysArr;
            }
        }

        [ConfigurationProperty("waitBeforeUpdate", IsRequired = false)]
        public int WaitBeforeUpdate
        {
            get { return (int)this["waitBeforeUpdate"]; }
            set { this["waitBeforeUpdate"] = value; }
        }

        [ConfigurationProperty("framework", IsRequired = false)]
        public string Framework
        {
            get { return (string)this["framework"]; }
            set { this["framework"] = value; }
        }

        [ConfigurationProperty("webpackPath", IsRequired = false)]
        public string WebpackPath
        {
            get { return (string)this["webpackPath"]; }
            set { this["webpackPath"] = value; }
        }

        [ConfigurationProperty("publishArgs", IsRequired = false)]
        public string PublishArgs
        {
            get { return (string)this["publishArgs"]; }
            set { this["publishArgs"] = value; }
        }
    }
}
