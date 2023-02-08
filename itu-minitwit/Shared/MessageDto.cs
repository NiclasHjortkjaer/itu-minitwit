namespace itu_minitwit.Shared;

public class MessageDto 
{
    public int Id { get; set; }
    public UserDto Author { get; set; }
    public string Text { get; set; }
    public DateTime? PublishDate { get; set; }
    public int? Flagged { get; set; }
}
