using itu_minitwit.Shared;

namespace itu_minitwit.Client.Services;

public interface IMessageService
{
    public Task<IEnumerable<MessageDto>> GetTimeline();
    public Task<IEnumerable<MessageDto>> GetMyTimeline();
    public Task PostTweet(MessageText message);
}

