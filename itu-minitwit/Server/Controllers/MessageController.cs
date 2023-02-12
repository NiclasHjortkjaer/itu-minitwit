using Microsoft.AspNetCore.Mvc;
using itu_minitwit.Shared;
using ituminitwit.Server.Interfaces.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace itu_minitwit.Server.Controllers;

[ApiController]
[Route("messages")]
public class MessageController : ControllerBase
{
    private readonly ILogger<MessageController> _logger;
    private readonly IMapper _mapper;
    private readonly IMessageRepository _repository;

    public MessageController(ILogger<MessageController> logger, IMapper mapper, IMessageRepository repository)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
    }

    [HttpGet]
    public async Task<IEnumerable<MessageDto>> Get()
    {
        var messages = await _repository.Get();
        var dtos = _mapper.Map<IEnumerable<MessageDto>>(messages);
        return dtos;
    }
    
    [HttpGet("mytimeline")]
    public async Task<IEnumerable<MessageDto>> GetMyTimeline()
    {
        var messages = await _repository.GetMyTimeline();
        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }
    
    [HttpGet("{username}")]
    public async Task<IEnumerable<MessageDto>> Get([FromRoute]string username)
    {
        var messages = await _repository.GetByUser(username);
        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    [Authorize]
    [HttpPost]
    public async void Post(MessageText message)
    {
        await _repository.Create(message);
    }
}

