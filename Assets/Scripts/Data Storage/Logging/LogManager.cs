using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data_Storage.Logging {
    public abstract class LogManager
    {
        protected static string StringifyDictionaryForJsonLogs(Dictionary<string, string> dictionary)
        {
            var parts = new List<string>();

            foreach (var pair in dictionary)
            {
                parts.Add($"{pair.Key}: \"{pair.Value}\"");
            }
            
            return "{ " + string.Join(", ", parts) + " }";
        }

        protected static string StringifyDictionaryForCsvLogs(Dictionary<string, string> dictionary, bool isHeader = false)
        {
            var items = isHeader 
                ? dictionary.Keys.AsEnumerable() 
                : dictionary.Values.AsEnumerable();

            return string.Join(",", items);
        }
        
        public abstract void InitLogs(MonoBehaviour monoBehaviourObject);
        public abstract IEnumerator WriteToLog(string database, string table, Dictionary<string, string> argsNValues, bool justHeaders = false);
        public abstract IEnumerator GetFromLog(string database, string table, string query, Func<string, int> yieldedReactionToGet);
        public abstract IEnumerator UpdateLog(string database, string table, string query, Dictionary<string, string> argsNValues);
        public abstract IEnumerator EndLogs();
    }
}
