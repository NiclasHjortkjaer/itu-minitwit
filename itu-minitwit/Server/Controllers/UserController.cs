using AutoMapper;
using itu_minitwit.Shared;
using ituminitwit.Server.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace itu_minitwit.Server.Controllers;

[ApiController]
[Route("users")]

public class UserController : Controller
{
    private readonly ILogger<MessageController> _logger;
    private readonly IMapper _mapper;
    private readonly IUserRepository _repository;

    public UserController(ILogger<MessageController> logger, IMapper mapper, IUserRepository repository)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
    }
    
    [Authorize]
    [HttpPut("follow/{username}")]
    public async void Follow([FromRoute] string username)
    {
        await _repository.Follow(username);
    }
    
    [Authorize]
    [HttpPut("unfollow/{username}")]
    public async void Unfollow([FromRoute] string username)
    {
        await _repository.Unfollow(username);
    }

    [Authorize]
    [HttpGet("current")]
    public async Task<UserDto> Get()
    {
        var user = await _repository.GetCurrentUser();
        return _mapper.Map<UserDto>(user);
    }
}