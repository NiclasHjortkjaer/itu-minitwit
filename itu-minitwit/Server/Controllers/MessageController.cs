using Microsoft.AspNetCore.Mvc;
using itu_minitwit.Shared;
using ituminitwit.Server.Interfaces.Repositories;
using AutoMapper;

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
}

