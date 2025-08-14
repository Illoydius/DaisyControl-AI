using System.Text.Json.Serialization;
using DaisyControl_AI.Storage.Dtos.JsonConverters;

namespace DaisyControl_AI.Storage.Dtos.User
{
    public class DaisyControlUserDto : IStorageDto
    {
        private List<InferenceTask> _inferenceTasks = new();

        [JsonPropertyName("userId")]
        public string Id { get; set; }

        [JsonPropertyName("revision")]
        public long Revision { get; set; }

        [JsonPropertyName("userInfo")]
        public DaisyControlUserInfo UserInfo { get; set; }

        [JsonPropertyName("messagesHistory")]
        public List<DaisyControlMessage> MessagesHistory { get; set; } = new();

        [JsonPropertyName("aiPersonaId")]
        public string AIPersonaId { get; set; } = null;

        [JsonPropertyName("lastModifiedAt")]
        public DateTimeOffset LastModifiedAtUtc { get; set; }

        [JsonPropertyName("createdAtUtc")]
        public DateTimeOffset CreatedAtUtc { get; set; }

        [JsonPropertyName("status")]
        public UserStatus Status { get; set; }

        // TODO: move to DaisyControlAIPersonaDto by User?
        [JsonConverter(typeof(DateTimeUnixJsonConverter))]
        [JsonPropertyName("lastThoughtAboutAtUtc")]
        public DateTime LastThoughtAboutAtUtc { get; set; } = DateTime.UtcNow.AddYears(-10);

        // TODO: move to DaisyControlAIPersonaDto by User?
        [JsonConverter(typeof(DateTimeUnixJsonConverter))]
        [JsonPropertyName("nextMessageToProcessOperationAvailabilityAtUtc")]
        public DateTime NextMessageToProcessOperationAvailabilityAtUtc { get; set; } = DateTime.UtcNow.AddYears(10);

        // TODO: move to DaisyControlAIPersonaDto by User?
        [JsonConverter(typeof(DateTimeUnixJsonConverter))]
        [JsonPropertyName("nextImmediateGoalOperationAvailabilityAtUtc")]
        public DateTime NextImmediateGoalOperationAvailabilityAtUtc { get; set; } = DateTime.UtcNow.AddYears(10);

        // TODO: move to DaisyControlAIPersonaDto by User?
        [JsonConverter(typeof(DateTimeUnixJsonConverter))]
        [JsonPropertyName("nextFollowUpAvailabilityAtUtc")]
        public DateTime NextFollowUpAvailabilityAtUtc { get; set; } = DateTime.UtcNow.AddYears(10);

        // TODO: move to DaisyControlAIPersonaDto by User?
        [JsonPropertyName("aiImmediateGoals")]
        public List<DaisyGoal> AIImmediateGoals { get; set; } = new();

        [JsonPropertyName("inferenceTasks")]
        public List<InferenceTask> InferenceTasks
        {
            get { return _inferenceTasks; }
            set
            {
                _inferenceTasks = value;
            }
        }

        /// <summary>
        /// Auto-calculated from InferenceTasks property count.
        /// </summary>
        [JsonPropertyName("pendingInferenceTasksCounter")]
        public int PendingInferenceTasksCounter
        {
            get { return _inferenceTasks.Count(); }
        }

        // Todo: add living with people? who
    }
}
