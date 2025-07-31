// [Sprint91_Recovery_P4] Nova: Class definition conflict resolved
namespace Data.Models
{
    public class PromptTraceLog
    {
        public Guid Id { get; set; }
        public string Prompt { get; set; } = "";
        public string Result { get; set; } = "";
        public DateTime ExecutedAt { get; set; }
        public string Output { get; set; } = string.Empty;
    }
}
