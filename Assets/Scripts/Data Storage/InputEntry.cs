using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data_Storage {
    [Serializable]
    public class InputEntry
    {
        public int studyId;
        private int _score;
        private List<(int, int)> _p1EffectsUsed;
        private List<(int, int)> _p2EffectsUsed;
        private List<int> _p1Hits;
        private List<int> _p2Hits;
        private List<int> _p1SecondaryAttacksUsed;
        private List<int> _p2SecondaryAttacksUsed;
        
        public InputEntry (int id) {
            studyId = id;
            _p1EffectsUsed = new List<(int,int)>();
            _p2EffectsUsed = new List<(int,int)>();
            _p1Hits = new List<int>();
            _p2Hits = new List<int>();
            _p1SecondaryAttacksUsed = new List<int>();
            _p2SecondaryAttacksUsed = new List<int>();
        }

        public void SetScore(int score)
        {
            _score = score;
        }

        public void AddEffect(bool isPlayer1, int elapsedTime, int effect)
        {
            if (isPlayer1)
            {
                _p1EffectsUsed.Add((elapsedTime, effect));
            }
            else
            {
                _p2EffectsUsed.Add((elapsedTime, effect));
            }
        }

        public void AddHit(bool isPlayer1, int elapsedTime)
        {
            if (isPlayer1)
            {
                _p1Hits.Add(elapsedTime);
            }
            else
            {
                _p2Hits.Add(elapsedTime);
            }
        }
        
        public void AddSecondaryAttack(bool isPlayer1, int elapsedTime)
        {
            if (isPlayer1)
            {
                _p1SecondaryAttacksUsed.Add(elapsedTime);
            }
            else
            {
                _p2SecondaryAttacksUsed.Add(elapsedTime);
            }
        }
    }
}
