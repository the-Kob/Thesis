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

        public override IEnumerator WriteToLog(string database, string table, Dictionary<string, string> dictionary, bool justHeaders = false)
        {
            var directoryPath = Path.Combine(Application.streamingAssetsPath, "Results", database);
            var filePath = Path.Combine(directoryPath, $"{table}.csv");

            Directory.CreateDirectory(directoryPath);

            var writeHeader = !File.Exists(filePath);

            using (var sw = new StreamWriter(filePath, append: true))
            {
                try
                {
                    if (writeHeader)
                    {
                        sw.WriteLine(StringifyDictionaryForCsvLogs(dictionary, true));
                    }

                    if (!justHeaders)
                    {
                        sw.WriteLine(StringifyDictionaryForCsvLogs(dictionary));
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to write log file: {ex.Message}");
                }
            }


            yield return null;
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
