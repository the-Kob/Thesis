using UnityEngine;

namespace Data_Storage.Events
{
    public enum EventType
    {
        Buff,
        Nerf,
        SecondaryAttack,
        GettingHit,
        End,
        EnemyHit,
        EnemyKill,
        BulletMiss
    }

    public enum Agent
    {
        None,
        P1,
        P2,
        P1Enemy,
        P2Enemy
    }
    
    public static class EventFactory
    {
        public static EventEntry CreateEffectEvent(bool isPlayer1, int elapsedTime, int effect, int studyId, int score, int combo)
        {
            return new EventEntry
            {
                StudyId = studyId,
                Score = score,
                Combo = combo,
                EventType = effect % 2 == 0 ? EventType.Buff : EventType.Nerf,
                Actuator = isPlayer1 ? Agent.P1 : Agent.P2,
                Receiver = effect < 2 ? Agent.P2Enemy : Agent.P1Enemy,
                ElapsedTime = elapsedTime
            };
        }

        public static EventEntry CreateGettingHitEvent(bool isPlayer1, int elapsedTime, int studyId, int score, int combo)
        {
            return new EventEntry
            {
                StudyId = studyId,
                Score = score,
                Combo = combo,
                EventType = EventType.GettingHit,
                Actuator = isPlayer1 ? Agent.P1Enemy : Agent.P2Enemy,
                Receiver = isPlayer1 ? Agent.P1 : Agent.P2,
                ElapsedTime = elapsedTime
            };
        }

        public static EventEntry CreateSecondaryAttackEvent(bool isPlayer1, int elapsedTime, int studyId, int score, int combo)
        {
            return new EventEntry
            {
                StudyId = studyId,
                Score = score,
                Combo = combo,
                EventType = EventType.SecondaryAttack,
                Actuator = isPlayer1 ? Agent.P1 : Agent.P2,
                Receiver = isPlayer1 ? Agent.P1Enemy : Agent.P2Enemy,
                ElapsedTime = elapsedTime
            };
        }
        
        public static EventEntry CreateBulletMissEvent(bool isPlayer1, int elapsedTime, int studyId, int score, int combo)
        {
            return new EventEntry
            {
                StudyId = studyId,
                Score = score,
                Combo = combo,
                EventType = EventType.BulletMiss,
                Actuator = isPlayer1 ? Agent.P1 : Agent.P2,
                Receiver = isPlayer1 ? Agent.P1Enemy : Agent.P2Enemy,
                ElapsedTime = elapsedTime
            };
        }

        public static EventEntry CreateEndEvent(int score, int studyId, int combo)
        {
            return new EventEntry
            {
                StudyId = studyId,
                Score = score,
                Combo = combo,
                EventType = EventType.End,
                Actuator = Agent.None,
                Receiver = Agent.None,
                ElapsedTime = 180
            };
        }
        
        public static EventEntry CreateEnemyHitEvent(bool isPlayer1, int elapsedTime, int studyId, int score, int combo)
        {
            return new EventEntry
            {
                StudyId = studyId,
                Score = score,
                Combo = combo,
                EventType = EventType.EnemyHit,
                Actuator = isPlayer1 ? Agent.P1 : Agent.P2,
                Receiver = isPlayer1 ? Agent.P1Enemy : Agent.P2Enemy,
                ElapsedTime = elapsedTime
            };
        }

        public static EventEntry CreateEnemyKillEvent(bool isPlayer1, int elapsedTime, int studyId, int score, int combo)
        {
            return new EventEntry
            {
                StudyId = studyId,
                Score = score,
                Combo = combo,
                EventType = EventType.EnemyKill,
                Actuator = isPlayer1 ? Agent.P1 : Agent.P2,
                Receiver = isPlayer1 ? Agent.P1Enemy : Agent.P2Enemy,
                ElapsedTime = elapsedTime
            };
        }
    }

}
