using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data_Storage.Logging
{
    public class DebugLogManager : LogManager
    {
        public override void InitLogs(MonoBehaviour monoBehaviourObject)
        {
            Debug.Log("Log initialized.");
        }

        public override IEnumerator WriteToLog(string database, string table, Dictionary<string, string> dictionary, bool justHeaders = false)
        {
            Debug.Log($"Database: {database}; Table: {table}; Dictionary: {dictionary}");
            
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
