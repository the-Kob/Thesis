using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Data_Storage.Logging
{
    public class FileLogManager : LogManager
    {
        public override void InitLogs(MonoBehaviour monoBehaviourObject)
        {
            Debug.Log("Log initialized.");
        }

        // FileLogManager.cs
        public override IEnumerator WriteToLog(string database, string table, Dictionary<string, string> dictionary, bool justHeaders = false)
        {
            WriteToLogInternal(database, table, dictionary, justHeaders);
            yield return null; // still satisfies coroutine
        }

        public void WriteToLogSync(string database, string table, Dictionary<string, string> dictionary, bool justHeaders = false)
        {
            WriteToLogInternal(database, table, dictionary, justHeaders);
        }

        private void WriteToLogInternal(string database, string table, Dictionary<string, string> dict, bool justHeaders)
        {
            var dir = Path.Combine(Application.persistentDataPath, "Results", database);
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, $"{table}.csv");
            var writeHeader = !File.Exists(path);

            try
            {
                using var sw = new StreamWriter(path, append: true);
                if (writeHeader)
                    sw.WriteLine(StringifyDictionaryForCsvLogs(dict, true));
                if (!justHeaders)
                    sw.WriteLine(StringifyDictionaryForCsvLogs(dict));
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Log Write Error] {ex}");
            }
        }

        public override IEnumerator GetFromLog(string database, string table, string query, Func<string, int> yieldedReactionToGet)
        {
            Debug.Log($"Database: {database}; Table: {table}");

            yield return yieldedReactionToGet("[]");
        }

        public override IEnumerator UpdateLog(string database, string table, string query, Dictionary<string, string> argsNValues)
        {
            Debug.Log($"Database: {database}; Table: {table}");

            yield return null;
        }

        public override IEnumerator EndLogs()
        {
            Debug.Log("Log terminated.");
            yield return null;
        }
    }
}
