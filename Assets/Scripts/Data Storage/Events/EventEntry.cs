using System.Collections.Generic;
using UnityEngine;

namespace Data_Storage.Events {
    public class EventEntry
    {
        public int StudyId { get; set; }
        public int Score { get; set; }
        public int Combo { get; set; }
        public EventType EventType { get; set; }
        public Agent Actuator { get; set; }
        public Agent Receiver { get; set; }
        public int ElapsedTime { get; set; }
        public float DistanceBetweenPlayers { get; set; }
        public DistanceTrend DistanceTrend { get; set; }
        public MovementTrend MovementTrend { get; set; }

        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>
            {
                {"study_id", StudyId.ToString()},
                {"score", Score.ToString()},
                {"combo", Combo.ToString()},
                {"event_type", EventType.ToString()},
                {"event_actuator", Actuator.ToString()},
                {"event_receiver", Receiver.ToString()},
                {"elapse_time", ElapsedTime.ToString()},
                {"distance_between_players", DistanceBetweenPlayers.ToString("F2")},
                {"distance_trend", DistanceTrend.ToString()},
                {"movement_trend", MovementTrend.ToString()},
            };
        }
    }
}
