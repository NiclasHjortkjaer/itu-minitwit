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
    private readonly ILogger<UserController> _logger;
    private readonly IMapper _mapper;
    private readonly IUserRepository _repository;

    public UserController(ILogger<UserController> logger, IMapper mapper, IUserRepository repository)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
    }
    
    [HttpPut("follow")]
    public async void Follow(UserName userName)
    {
        await _repository.Follow(userName.Name);
    }
    
    [Authorize]
    [HttpPut("unfollow")]
    public async void Unfollow(UserName userName)
    {
        await _repository.Unfollow(userName.Name);
    }

    [Authorize]
    [HttpGet("current")]
    public async Task<UserDto> Get()
    {
        var user = await _repository.GetCurrentUser();
        return _mapper.Map<UserDto>(user);
    }
    
    [HttpGet("{username}")]
    public async Task<bool> IsFollowing([FromRoute]string username)
    {
        return await _repository.IsFollowing(username);
    }
}