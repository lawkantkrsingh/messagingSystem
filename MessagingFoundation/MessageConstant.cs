namespace AdvantestMessagingFoundation
{
    /// <summary>
    /// enum to decide priority of the message
    /// </summary>
    public enum MessagePriority
    {
        Low,
        Medium,
        High
    }

    /// <summary>
    /// enum for message command
    /// </summary>
    public enum MessageCommand
    {
    }

    public class Message
    {
        public string Content { get; set; }
        public MessagePriority Priority { get; set; }
    }
}
