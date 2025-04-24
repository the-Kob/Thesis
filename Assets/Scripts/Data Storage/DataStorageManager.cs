using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Data_Storage
{
    public class DataStorageManager : MonoBehaviour
    {
        public static DataStorageManager Instance { get; private set; }
        
        [SerializeField] private string filename;

        private List<InputEntry> _entriesList = new List<InputEntry>();
        
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
        }

        private void Start()
        {
            
        }
    }
}