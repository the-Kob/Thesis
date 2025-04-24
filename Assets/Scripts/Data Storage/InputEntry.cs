using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data_Storage {
    [Serializable]
    public class InputEntry
    {
        public int studyId;
        public int score;
        public List<(int,int)> BuffsUsedByP1;
        public List<(int,int)> BuffsUsedByP2;
        public List<int> secondaryAttackByP1;
        public List<int> secondaryAttackByP2;
        public List<int> deathsByP1;
        public List<int> deathsByP2;
        
        public InputEntry (int id) {
            BuffsUsedByP1 = new List<(int,int)>();
            BuffsUsedByP2 = new List<(int,int)>();
            deathsByP1 = new List<int>();
            deathsByP2 = new List<int>();
            secondaryAttackByP1 = new List<int>();
            secondaryAttackByP2 = new List<int>();
            studyId = id;
        }
    }
}
