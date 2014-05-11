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

		/// <summary>
		/// Loads a chain list from an XML file.
		/// </summary>
		/// <returns>The chain.</returns>
		/// <param name="pathName">Path name.</param>
		public void LoadChain(string pathName) {
			XDocument chainDoc = null;
			XElement rootElement = null;
			List<Chain> chainList = new List<Chain>();

			try {
				using (FileStream fs = new FileStream(pathName, FileMode.Open)) {
					using (XmlReader xmlReader = XmlReader.Create(fs)) {
						chainDoc = XDocument.Load(xmlReader);
					}
				}

				if ((rootElement = chainDoc.Element("Configuration")) == null) {
					//TODO: Log error
					return;
				}

				foreach (XElement chainElement in rootElement.Elements("Chain")) {
					string chainName = null;

					if ( chainElement.HasAttributes == true && chainElement.Attribute("Name") != null 
						&& string.IsNullOrEmpty((chainName = chainElement.Attribute("Name").Value)) == true) {
						//TODO: Log error
						continue;
					}

					if (parent.GetChainByName(chainName) == null) {
						//TODO: Log chain already exists
						chainList.Add(new Chain(parent, chainElement));
					}
				}

			} catch (Exception) {
				//TODO: Log error
				return;
			}

			parent.chainList.AddRange(chainList);
		}
	}
}

