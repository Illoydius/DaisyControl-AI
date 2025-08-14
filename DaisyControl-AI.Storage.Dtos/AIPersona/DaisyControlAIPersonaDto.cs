using System.Text.Json.Serialization;
using DaisyControl_AI.Storage.Dtos.User;

namespace DaisyControl_AI.Storage.Dtos.AIPersona
{
    public class DaisyControlAIPersonaDto : IStorageDto
    {
        private List<InferenceTask> _inferenceTasks = new();

        [JsonPropertyName("personaId")]
        public string Id { get; set; }

        [JsonPropertyName("revision")]
        public long Revision { get; set; }

        [JsonPropertyName("personaInfo")]
        public DaisyControlUserInfo PersonaInfo { get; set; }

        [JsonPropertyName("lastModifiedAt")]
        public DateTimeOffset LastModifiedAtUtc { get; set; }

        [JsonPropertyName("createdAtUtc")]
        public DateTimeOffset CreatedAtUtc { get; set; }

        [JsonPropertyName("status")]
        public UserStatus Status { get; set; }

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
    }
}
