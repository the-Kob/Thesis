using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data_Storage.Events;
using Data_Storage.Logging;
using UnityEditor;
using UnityEngine;

namespace Data_Storage
{
    public class DataStorageManager : MonoBehaviour
    {
        public static DataStorageManager Instance { get; private set; }
        
        [SerializeField] private string filename = "data.json";

        private List<InputEntry> _entriesList = new ();
        private InputEntry _currentEntry;
        private string _studyId;
        private readonly FileLogManager fileLogManager = new ();
        private bool _isFirstPlaythrough;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            _entriesList = FileHandler.ReadListFromJson<InputEntry>(filename);
        }

        private void Start()
        {
            _studyId = GenerateUniqueStudyId();
        }
        
        private string GenerateUniqueStudyId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            System.Random random = new();
            HashSet<string> existingIds = new();
    
            foreach (var entry in _entriesList)
            {
                existingIds.Add(entry.studyId);
            }

            string newId;
            do
            {
                newId = new string(Enumerable.Repeat(chars, 12)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            } while (existingIds.Contains(newId));

            return newId;
        }
        
        private static void ExecuteIfTutorialIsDone(Action action)
        {
            if (!GameManager.Instance.TutorialDone) return;
            action.Invoke();
        }

        public void CreateNewEntry(bool isFirstPlaythrough)
        {
            _currentEntry = new InputEntry(_studyId);
            _isFirstPlaythrough = isFirstPlaythrough;
        }

        internal void SaveEntry(float score, int combo)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                if (_currentEntry != null)
                {
                    _currentEntry.SetScore((int) score);
                    
                    StartCoroutine(fileLogManager.WriteToLog("Events Data", "Events",
                        EventFactory.CreateEndEvent((int) score, _studyId, combo, _isFirstPlaythrough).ToDictionary()));
                }
                
                _entriesList.Add(_currentEntry);
                FileHandler.SaveToJson(_entriesList, filename);
                _currentEntry = null;
                _studyId = GenerateUniqueStudyId();
            });
        }

        public void SavePlayerEffectUsed(bool isPlayer1, int elapsedTime, int effect, float score, int combo, float distance)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                if (_currentEntry == null) return;
                
                _currentEntry.AddEffect(isPlayer1, elapsedTime, effect);
                StartCoroutine(fileLogManager.WriteToLog("Events Data", "Events",
                    EventFactory.CreateEffectEvent(isPlayer1, elapsedTime, effect, _studyId, (int) score, combo, distance, _isFirstPlaythrough).ToDictionary()));
            });
        }
        public void SavePlayerSecondaryAttackUsed(bool isPlayer1, int elapsedTime, float score, int combo, float distance)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                if (_currentEntry == null) return;
                
                _currentEntry.AddSecondaryAttack(isPlayer1, elapsedTime);
                StartCoroutine(fileLogManager.WriteToLog("Events Data", "Events",
                    EventFactory.CreateSecondaryAttackEvent(isPlayer1, elapsedTime, _studyId, (int) score, combo, distance, _isFirstPlaythrough).ToDictionary()));
            });
        }

        public void SavePlayerGettingHit(bool isPlayer1, int elapsedTime, float score, int combo, float distance)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                if (_currentEntry == null) return;
                
                _currentEntry.AddHit(isPlayer1, elapsedTime);
                StartCoroutine(fileLogManager.WriteToLog("Events Data", "Events",
                    EventFactory.CreateGettingHitEvent(isPlayer1, elapsedTime, _studyId, (int)score, combo, distance, _isFirstPlaythrough).ToDictionary()));
            });
        }
        
        public void SaveEnemyKilled(bool isPlayer1, int elapsedTime, float score, int combo, float distance)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                StartCoroutine(fileLogManager.WriteToLog("Events Data", "Events",
                    EventFactory.CreateEnemyKillEvent(isPlayer1, elapsedTime, _studyId, (int)score, combo, distance, _isFirstPlaythrough).ToDictionary()));
            });
        }

        public void SaveEnemyHit(bool isPlayer1, int elapsedTime, float score, int combo, float distance)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                StartCoroutine(fileLogManager.WriteToLog("Events Data", "Events",
                    EventFactory.CreateEnemyHitEvent(isPlayer1, elapsedTime, _studyId, (int)score, combo, distance, _isFirstPlaythrough).ToDictionary()));
            });
        }

        public void SaveBulletMiss(bool isPlayer1, int elapsedTime, float score, int combo, float distance)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                StartCoroutine(fileLogManager.WriteToLog("Events Data", "Events",
                    EventFactory.CreateBulletMissEvent(isPlayer1, elapsedTime, _studyId, (int)score, combo, distance, _isFirstPlaythrough).ToDictionary()));
            });
        }

        public void SavePlayerDisplacement(bool isPlayer1, int elapsedTime, float score, int combo, float distanceBetweenPlayers, float distanceSinceLastTrigger, DistanceTrend distanceTrend, MovementTrend movementTrend)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                StartCoroutine(fileLogManager.WriteToLog("Events Data", "Events",
                    EventFactory.CreatePlayerDisplacementEvent(isPlayer1, elapsedTime, _studyId, (int)score, combo, distanceBetweenPlayers, distanceSinceLastTrigger, distanceTrend, movementTrend, _isFirstPlaythrough).ToDictionary()));
            });
        }
    }
}