using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Common.Interface
{
    public class ConfigRuleService
    {
        private ResourceXmlReaderConfigRule resourceConfigRule;

        public ConfigRuleService()
        {
            resourceConfigRule = new ResourceXmlReaderConfigRule();
        }
        public string GetResourceConfig(string resourceKey, string messageGroup)
        {
            try
            {
                if (resourceConfigRule.ResourcesConfig.ContainsKey(messageGroup))
                {
                    if (resourceConfigRule.ResourcesConfig[messageGroup].ContainsKey(resourceKey))
                    {
                        return resourceConfigRule.ResourcesConfig[messageGroup][resourceKey];
                    }
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    public class ResourceXmlReaderConfigRule
    {
        public readonly Dictionary<string, Dictionary<string, string>> ResourcesConfig = new Dictionary<string, Dictionary<string, string>>();
        private string path = "";
        public ResourceXmlReaderConfigRule()
        {
            path = AppDomain.CurrentDomain.BaseDirectory + @"\ConfigRule.xml";
            OpenAndStoreResource(path);
        }

        private void OpenAndStoreResource(string resourcePath)
        {
            try
            {

                XDocument doc = XDocument.Load(resourcePath);
                if (null != doc)
                {
                    var resources = doc.Descendants("ResourceGroup").ToList();

                    Dictionary<string, string> currentResource = null;

                    foreach (var item in resources)
                    {
                        currentResource = new Dictionary<string, string>();
                        var children = item.Descendants("Resource").ToList();
                        children.ForEach(o => currentResource.Add(o.Attribute("key").Value, o.Value.TrimStart().TrimEnd()));
                        if (!ResourcesConfig.ContainsKey(item.FirstAttribute.Value))
                        {
                            ResourcesConfig.Add(item.FirstAttribute.Value, currentResource);
                        }
                    }
                }
            }
            catch
            {

            }

        }
    }
}
