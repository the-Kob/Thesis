using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Data_Storage.Events {
    public class EventEntry
    {
        public string StudyId { get; set; }
        public int Score { get; set; }
        public int Combo { get; set; }
        public EventType EventType { get; set; } = EventType.None;
        public Agent Actuator { get; set; } = Agent.None;
        public Agent Receiver { get; set; } = Agent.None;
        public int ElapsedTime { get; set; } = 180;
        public float DistanceBetweenPlayers { get; set; } = -1f;
        public float DistanceSinceLastDisplacementTrigger { get; set; } = -1f;
        public DistanceTrend DistanceTrend { get; set; } = DistanceTrend.None;
        public MovementTrend MovementTrend { get; set; } = MovementTrend.None;
        public bool IsFirstPlaythrough { get; set; } = false;

        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>
            {
                {"study_id", StudyId},
                {"score", Score.ToString()},
                {"combo", Combo.ToString()},
                {"event_type", EventType.ToString()},
                {"event_actuator", Actuator.ToString()},
                {"event_receiver", Receiver.ToString()},
                {"elapse_time", ElapsedTime.ToString()},
                {"distance_between_players", DistanceBetweenPlayers.ToString("F2", CultureInfo.InvariantCulture)},
                {"distance_since_last_displacement_trigger", DistanceSinceLastDisplacementTrigger.ToString("F2", CultureInfo.InvariantCulture)},
                {"distance_trend", DistanceTrend.ToString()},
                {"movement_trend", MovementTrend.ToString()},
                {"is_first_playthrough", IsFirstPlaythrough.ToString()}
            };
        }
    }
}
