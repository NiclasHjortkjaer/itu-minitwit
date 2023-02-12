namespace itu_minitwit.Server.Database;

public class Message
{
    public int Id { get; set; }
    public string AuthorId { get; set; }
    public virtual User Author { get; set; }
    public string Text { get; set; }
    public DateTime? PublishDate { get; set; }
    public int? Flagged { get; set; }
}