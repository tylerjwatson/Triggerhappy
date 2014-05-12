using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Linq;

namespace TriggerHappy {

    public class ChainLoader {
        protected TriggerHappyPlugin parent;

        /// <summary>
        /// Returns a list of types loaded in the assembly by their attribute.
        /// </summary>
        /// <returns>
        /// A dictionary containing the action name as a string, and the type of the action class,
        /// or null if an error occured.
        /// </returns>
        public Dictionary<string, Type> LoadAttributeTypes<T>() where T: Attribute {
            Dictionary<string, Type> actionDict = new Dictionary<string, Type>();
            var qActions = from i in Assembly.GetExecutingAssembly().GetTypes()
                                    let actions = i.GetCustomAttributes(typeof(T), true)
                                    where actions.Length == 1
                                    select new { Attribute = actions.FirstOrDefault() as T, Type = i };

            foreach (var action in qActions) {
                var p = (action.Attribute as T).GetType().GetProperty("Name");
                string nameValue = null;
                if (p == null) {
                    continue;
                }

                nameValue = p.GetValue(action.Attribute, null) as string;
                if (string.IsNullOrEmpty(nameValue) == true) {
                    continue;
                }

                if (actionDict.ContainsKey(nameValue) == true) {
                    //TODO: Log error
                    return null;
                } else {
                    actionDict.Add(nameValue, action.Type);
                }
            }

            return actionDict;
        }

        public ChainLoader(TriggerHappyPlugin pluginInstance) {
            this.parent = pluginInstance;
        }

        public bool VerifyChains() {
            bool duplicatesFound = false;

            //basic sanity check
            if (parent.chainList == null || parent.chainList.Count == 0) {
                THLog.Log(LogLevel.Error, "Chain verify failed: no chains have been loaded, triggerhappy is inactive.");
                return false;
            }

            //verify there's an __INCOMING__ chain and only one of them
            if (parent.chainList.Count(i => i.Name.Equals("__INCOMING__")) != 1) {
                THLog.Log(LogLevel.Error, "Chain verify failed: there must be one and ONLY one __INCOMING__ chain.");
                return false;
            }

            //verify there's no duplicate chain names
            var qDuplicateChains = from i in parent.chainList
                                            group i by i.Name into Group
                                            where Group.Count() > 1
                                            select Group.Key;

            foreach (string duplicateChain in qDuplicateChains) {
                if (duplicatesFound == false) {
                    duplicatesFound = true;
                }
                THLog.Log(LogLevel.Error, "Chain verify failed: duplicate chain by the name of {0}.", duplicateChain);
            }

            if (duplicatesFound == true) {
                return false;
            }

            //verify accessible chains with no actions
            //these are not fatal errors, just warn
            foreach (Chain chain in parent.chainList) {
                TerrariaApi.Server.GetDataEventArgs nullObject = null;
                if (chain.ChainCanProcess(ref nullObject, false) == true && chain.Actions.Count == 0) {
                    THLog.Log(LogLevel.Warning, "Chain verify warning: Chain {0} is accessible but has no actions; it does nothing!", chain.Name);
                }
            }

            //(maybe) verify cyclic redundancies
            //no idea how the fuck I am going to achieve this without a maintaining a chain callstack
            //cbf for now
            return true;
        }

        /// <summary>
        /// Loads the chains in the specified directory.
        /// </summary>
        /// <param name="dirName">Dir name.</param>
        public void LoadChainsInDirectory(string dirName) {
            if (Directory.Exists(dirName) == false) {
                THLog.Log(LogLevel.Error, "Chain directory does not exist, creating a new one.");
                try {
                    THLog.Debug("CreateDirectory {0}", dirName);
                    Directory.CreateDirectory(dirName);
                } catch (Exception) {
                    THLog.Log(LogLevel.Error, "CreateDirectory {0} failed", dirName);
                    return;
                }
                THLog.Debug("CreateDirectory {0} succeeded", dirName);
            }

            THLog.Debug("LoadChainsInDirectory {0}", dirName);

            foreach (string xmlFile in Directory.GetFiles(dirName, "*.xml")) {
                try {
                    THLog.Debug("LoadChain {0}", xmlFile);
                    LoadChain(Path.Combine(dirName, xmlFile));
                } catch (Exception) {
                    THLog.Log(LogLevel.Error, "LoadChain {0} failed.", dirName);
                }

                THLog.Debug("LoadChain {0} succeeded", xmlFile);
            }

            THLog.Debug("LoadChainsInDirectory {0} succeeded.", dirName);
        }

        /// <summary>
        /// Loads a chain list from an XML file.
        /// </summary>
        /// <returns>The chain.</returns>
        /// <param name="pathName">Path name.</param>
        public void LoadChain(string pathName) {
            XDocument chainDoc = null;
            XElement rootElement = null;
            List<Chain> chainList = new List<Chain>();
            int chainCount = 0;

            try {
                using (FileStream fs = new FileStream(pathName, FileMode.Open)) {
                    THLog.Debug(null, "Loading chain file {0}.", pathName);
                    using (XmlReader xmlReader = XmlReader.Create(fs)) {
                        chainDoc = XDocument.Load(xmlReader);
                        THLog.Debug(null, "XmlLoad {0} succeeded.", pathName);
                    }
                }
            } catch (Exception) {
                THLog.Log(LogLevel.Error, "Cannot load chain file {0}, skipping", pathName);
                return;
            }

            if ((rootElement = chainDoc.Element("Configuration")) == null) {
                THLog.Log(LogLevel.Error, "File {0} is not a chain file, skipping", pathName);
                return;
            }

            THLog.Debug("File {0} is a valid chain configurqation file.", pathName);

            foreach (XElement chainElement in rootElement.Elements("Chain")) {
                string chainName = null;

                if (chainElement.HasAttributes == true && chainElement.Attribute("Name") != null
                    && string.IsNullOrEmpty((chainName = chainElement.Attribute("Name").Value)) == true) {
                    THLog.Log(LogLevel.Debug, "A chain in file {0} does not have a name, skipping", pathName);
                    continue;
                }

                THLog.Debug("ChainAdd {0}", chainName);

                if (parent.GetChainByName(chainName) == null) {
                    try {
                        chainList.Add(new Chain(parent, chainElement));
                    } catch (Exception) {
                        THLog.Log(LogLevel.Error, "Internal error creating chain.");
                    }

                    THLog.Debug("ChainAdd {0} succeeded", chainName);
                } else {
                    THLog.Log(LogLevel.Error, "A chain by the name of \"{0}\" already exists, skipping", chainName);
                }

                chainCount++;
            }

            THLog.Debug("LoadChains loaded {1} chains", chainCount);
            parent.chainList.AddRange(chainList);
        }
    }
}

