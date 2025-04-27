using System;
using System.Collections.Generic;
using System.IO;
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
        private int _studyId;
        private readonly FileLogManager fileLogManager = new ();
        
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
            if (_entriesList.Count == 0)
            {
                _studyId = 0;
            }
            else
            {
                _studyId = _entriesList[^1].studyId + 1; // [^1] is the last element in the list
            }
        }
        
        private static void ExecuteIfTutorialIsDone(Action action)
        {
            if (!GameManager.Instance.tutorialDone) return;
            action.Invoke();
        }

        public void CreateNewEntry()
        {
            _currentEntry = new InputEntry(_studyId);
        }

        internal void SaveEntry(float score, int combo)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                if (_currentEntry != null)
                {
                    _currentEntry.SetScore((int) score);
                    
                    StartCoroutine(fileLogManager.WriteToLog("Events Data", "Events",
                        EventFactory.CreateEndEvent((int) score, _studyId, combo).ToDictionary()));
                }
                
                _entriesList.Add(_currentEntry);
                FileHandler.SaveToJson(_entriesList, filename);
                _currentEntry = null;
            });
        }

        public void SavePlayerEffectUsed(bool isPlayer1, int elapsedTime, int effect, float score, int combo)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                if (_currentEntry == null) return;
                
                _currentEntry.AddEffect(isPlayer1, elapsedTime, effect);
                StartCoroutine(fileLogManager.WriteToLog("Events Data", "Events",
                    EventFactory.CreateEffectEvent(isPlayer1, elapsedTime, effect, _studyId, (int) score, combo).ToDictionary()));
            });
        }
        public void SavePlayerSecondaryAttackUsed(bool isPlayer1, int elapsedTime, float score, int combo)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                if (_currentEntry == null) return;
                
                _currentEntry.AddSecondaryAttack(isPlayer1, elapsedTime);
                StartCoroutine(fileLogManager.WriteToLog("Events Data", "Events",
                    EventFactory.CreateSecondaryAttackEvent(isPlayer1, elapsedTime, _studyId, (int) score, combo).ToDictionary()));
            });
        }

        public void SavePlayerGettingHit(bool isPlayer1, int elapsedTime, float score, int combo)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                if (_currentEntry == null) return;
                
                _currentEntry.AddHit(isPlayer1, elapsedTime);
                StartCoroutine(fileLogManager.WriteToLog("Events Data", "Events",
                    EventFactory.CreateGettingHitEvent(isPlayer1, elapsedTime, _studyId, (int)score, combo).ToDictionary()));
            });
        }
        
        public void SaveEnemyKilled(bool isPlayer1, int elapsedTime, float score, int combo)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                StartCoroutine(fileLogManager.WriteToLog("Events Data", "Events",
                    EventFactory.CreateEnemyKillEvent(isPlayer1, elapsedTime, _studyId, (int)score, combo).ToDictionary()));
            });
        }

        public void SaveEnemyHit(bool isPlayer1, int elapsedTime, float score, int combo)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                StartCoroutine(fileLogManager.WriteToLog("Events Data", "Events",
                    EventFactory.CreateEnemyHitEvent(isPlayer1, elapsedTime, _studyId, (int)score, combo).ToDictionary()));
            });
        }

        public void SaveBulletMiss(bool isPlayer1, int elapsedTime, float score, int combo)
        {
            ExecuteIfTutorialIsDone(() =>
            {
                StartCoroutine(fileLogManager.WriteToLog("Events Data", "Events",
                    EventFactory.CreateBulletMissEvent(isPlayer1, elapsedTime, _studyId, (int)score, combo).ToDictionary()));
            });
        }
    }
}