using Unity.VisualScripting;
using UnityEngine;

namespace Data_Storage.Events
{
    public enum EventType
    {
        None,
        Buff,
        Nerf,
        SecondaryAttack,
        GettingHit,
        End,
        EnemyHit,
        EnemyKill,
        BulletMiss,
        Displacement
    }

    public enum Agent
    {
        None,
        P1,
        P2,
        P1Enemy,
        P2Enemy
    }

    public enum DistanceTrend
    {
        None,
        Closer,
        Same,
        Farther
    }

    public enum MovementTrend
    {
        None, 
        Toward, 
        Away, 
        Sideways
    }       
    
    public static class EventFactory
    {
        public static EventEntry CreateEffectEvent(bool isPlayer1, int elapsedTime, int effect, string studyId, int score, int combo, float distance, bool isFirstPlaythrough)
        {
            return new EventEntry
            {
                StudyId = studyId,
                Score = score,
                Combo = combo,
                EventType = effect % 2 == 0 ? EventType.Buff : EventType.Nerf,
                Actuator = isPlayer1 ? Agent.P1 : Agent.P2,
                Receiver = effect < 2 ? Agent.P2Enemy : Agent.P1Enemy,
                ElapsedTime = elapsedTime,
                DistanceBetweenPlayers = distance,
                IsFirstPlaythrough = isFirstPlaythrough,
            };
        }

        public static EventEntry CreateGettingHitEvent(bool isPlayer1, int elapsedTime, string studyId, int score, int combo, float distance, bool isFirstPlaythrough)
        {
            return new EventEntry
            {
                StudyId = studyId,
                Score = score,
                Combo = combo,
                EventType = EventType.GettingHit,
                Actuator = isPlayer1 ? Agent.P1Enemy : Agent.P2Enemy,
                Receiver = isPlayer1 ? Agent.P1 : Agent.P2,
                ElapsedTime = elapsedTime,
                DistanceBetweenPlayers = distance,
                IsFirstPlaythrough = isFirstPlaythrough,
            };
        }

        public static EventEntry CreateSecondaryAttackEvent(bool isPlayer1, int elapsedTime, string studyId, int score, int combo, float distance, bool isFirstPlaythrough)
        {
            return new EventEntry
            {
                StudyId = studyId,
                Score = score,
                Combo = combo,
                EventType = EventType.SecondaryAttack,
                Actuator = isPlayer1 ? Agent.P1 : Agent.P2,
                Receiver = isPlayer1 ? Agent.P1Enemy : Agent.P2Enemy,
                ElapsedTime = elapsedTime,
                DistanceBetweenPlayers = distance,
                IsFirstPlaythrough = isFirstPlaythrough,
            };
        }
        
        public static EventEntry CreateBulletMissEvent(bool isPlayer1, int elapsedTime, string studyId, int score, int combo, float distance, bool isFirstPlaythrough)
        {
            return new EventEntry
            {
                StudyId = studyId,
                Score = score,
                Combo = combo,
                EventType = EventType.BulletMiss,
                Actuator = isPlayer1 ? Agent.P1 : Agent.P2,
                Receiver = isPlayer1 ? Agent.P1Enemy : Agent.P2Enemy,
                ElapsedTime = elapsedTime,
                DistanceBetweenPlayers = distance,
                IsFirstPlaythrough = isFirstPlaythrough,
            };
        }

        public static EventEntry CreateEndEvent(int score, string studyId, int combo, bool isFirstPlaythrough)
        {
            return new EventEntry
            {
                StudyId = studyId,
                Score = score,
                Combo = combo,
                EventType = EventType.End,
                IsFirstPlaythrough = isFirstPlaythrough,
            };
        }
        
        public static EventEntry CreateEnemyHitEvent(bool isPlayer1, int elapsedTime, string studyId, int score, int combo, float distance, bool isFirstPlaythrough)
        {
            return new EventEntry
            {
                StudyId = studyId,
                Score = score,
                Combo = combo,
                EventType = EventType.EnemyHit,
                Actuator = isPlayer1 ? Agent.P1 : Agent.P2,
                Receiver = isPlayer1 ? Agent.P1Enemy : Agent.P2Enemy,
                ElapsedTime = elapsedTime,
                DistanceBetweenPlayers = distance,
                IsFirstPlaythrough = isFirstPlaythrough,
            };
        }

        public static EventEntry CreateEnemyKillEvent(bool isPlayer1, int elapsedTime, string studyId, int score, int combo, float distance, bool isFirstPlaythrough)
        {
            return new EventEntry
            {
                StudyId = studyId,
                Score = score,
                Combo = combo,
                EventType = EventType.EnemyKill,
                Actuator = isPlayer1 ? Agent.P1 : Agent.P2,
                Receiver = isPlayer1 ? Agent.P1Enemy : Agent.P2Enemy,
                ElapsedTime = elapsedTime,
                DistanceBetweenPlayers = distance,
                IsFirstPlaythrough = isFirstPlaythrough,
            };
        }

        public static EventEntry CreatePlayerDisplacementEvent(bool isPlayer1, int elapsedTime, string studyId, int score,
            int combo, float distance, float distanceSinceLastTrigger, DistanceTrend distanceTrend, MovementTrend movementTrend, bool isFirstPlaythrough)
        {
            return new EventEntry
            {
                StudyId = studyId,
                Score = score,
                Combo = combo,
                EventType = EventType.Displacement,
                Actuator = isPlayer1 ? Agent.P1 : Agent.P2,
                Receiver = isPlayer1 ? Agent.P2 : Agent.P1,
                ElapsedTime = elapsedTime,
                DistanceBetweenPlayers = distance,
                DistanceSinceLastDisplacementTrigger = distanceSinceLastTrigger,
                DistanceTrend = distanceTrend,
                MovementTrend = movementTrend,
                IsFirstPlaythrough = isFirstPlaythrough,
            };
        }
    }

}
