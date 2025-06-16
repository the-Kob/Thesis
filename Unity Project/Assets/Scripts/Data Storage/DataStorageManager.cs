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
        public string StudyId { get; private set; }
        private string _studyIDPrefix;
        private string _studyIDSuffix;
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
            GenerateNewStudyID();
        }
        
        private static string GenerateRandomId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            System.Random random = new();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string GenerateFullStudyId()
        {
            _studyIDSuffix = GenerateRandomId(6);
            return _studyIDPrefix + _studyIDSuffix;
        }
        
        public void GenerateNewStudyID()
        {
            _studyIDPrefix = GenerateRandomId(6);
            StudyId = GenerateFullStudyId();
        }
        
        private static void ExecuteIfTutorialIsDone(Action action)
        {
            if (!GameManager.Instance.TutorialDone) return;
            action.Invoke();
        }

        public void CreateNewEntry(bool isFirstPlaythrough)
        {
            
            _currentEntry = new InputEntry(StudyId);
            _isFirstPlaythrough = isFirstPlaythrough;
        }

        internal void SaveEntry(float score, int combo)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                if (_currentEntry != null)
                {
                    _currentEntry.SetScore((int) score);
                    
                    fileLogManager.WriteToLogSync("Events Data", "Events",
                        EventFactory.CreateEndEvent((int) score, StudyId, combo, _isFirstPlaythrough).ToDictionary());
                }
                
                _entriesList.Add(_currentEntry);
                FileHandler.SaveToJson(_entriesList, filename);
                _currentEntry = null;
                StudyId = GenerateFullStudyId();
            });
        }

        public void SavePlayerEffectUsed(bool isPlayer1, int elapsedTime, int effect, float score, int combo, float distance)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                if (_currentEntry == null) return;
                
                _currentEntry.AddEffect(isPlayer1, elapsedTime, effect);
                fileLogManager.WriteToLogSync("Events Data", "Events",
                    EventFactory.CreateEffectEvent(isPlayer1, elapsedTime, effect, StudyId, (int) score, combo, distance, _isFirstPlaythrough).ToDictionary());
            });
        }
        public void SavePlayerSecondaryAttackUsed(bool isPlayer1, int elapsedTime, float score, int combo, float distance)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                if (_currentEntry == null) return;
                
                _currentEntry.AddSecondaryAttack(isPlayer1, elapsedTime);
                fileLogManager.WriteToLogSync("Events Data", "Events",
                    EventFactory.CreateSecondaryAttackEvent(isPlayer1, elapsedTime, StudyId, (int) score, combo, distance, _isFirstPlaythrough).ToDictionary());
            });
        }

        public void SavePlayerGettingHit(bool isPlayer1, int elapsedTime, float score, int combo, float distance)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                if (_currentEntry == null) return;
                
                _currentEntry.AddHit(isPlayer1, elapsedTime);
                fileLogManager.WriteToLogSync("Events Data", "Events",
                    EventFactory.CreateGettingHitEvent(isPlayer1, elapsedTime, StudyId, (int)score, combo, distance, _isFirstPlaythrough).ToDictionary());
            });
        }
        
        public void SaveEnemyKilled(bool isPlayer1, int elapsedTime, float score, int combo, float distance)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                fileLogManager.WriteToLogSync("Events Data", "Events",
                    EventFactory.CreateEnemyKillEvent(isPlayer1, elapsedTime, StudyId, (int)score, combo, distance, _isFirstPlaythrough).ToDictionary());
            });
        }

        public void SaveEnemyHit(bool isPlayer1, int elapsedTime, float score, int combo, float distance)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                fileLogManager.WriteToLogSync("Events Data", "Events",
                    EventFactory.CreateEnemyHitEvent(isPlayer1, elapsedTime, StudyId, (int)score, combo, distance, _isFirstPlaythrough).ToDictionary());
            });
        }

        public void SaveBulletMiss(bool isPlayer1, int elapsedTime, float score, int combo, float distance)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                fileLogManager.WriteToLogSync("Events Data", "Events",
                    EventFactory.CreateBulletMissEvent(isPlayer1, elapsedTime, StudyId, (int)score, combo, distance, _isFirstPlaythrough).ToDictionary());
            });
        }

        public void SavePlayerDisplacement(bool isPlayer1, int elapsedTime, float score, int combo, float distanceBetweenPlayers, float distanceSinceLastTrigger, DistanceTrend distanceTrend, MovementTrend movementTrend)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                fileLogManager.WriteToLogSync("Events Data", "Events",
                    EventFactory.CreatePlayerDisplacementEvent(isPlayer1, elapsedTime, StudyId, (int)score, combo, distanceBetweenPlayers, distanceSinceLastTrigger, distanceTrend, movementTrend, _isFirstPlaythrough).ToDictionary());
            });
        }
    }
}