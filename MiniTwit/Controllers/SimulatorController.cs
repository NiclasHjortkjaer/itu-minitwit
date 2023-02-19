using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MiniTwit.Database;
using MiniTwit.DTOs;
using MiniTwit.Hubs;
using MiniTwit.Repositories;

namespace MiniTwit.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SimulatorController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly MiniTwitContext _miniTwitContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<TwitHub> _twitHubContext;
        private static string ApiToken = "c2ltdWxhdG9yOnN1cGVyX3NhZmUh"; //Should be in environment variable or something

        public SimulatorController(IMessageRepository messageRepository, IUserRepository userRepository, MiniTwitContext miniTwitContext, IHttpContextAccessor httpContextAccessor, IHubContext<TwitHub> twitHubContext)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _miniTwitContext = miniTwitContext;
            _httpContextAccessor = httpContextAccessor;
            _twitHubContext = twitHubContext;
        }
        
        // POST: Simulator/register
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            string error = null;
            if (registerDto.Username == null)
            {
                error = "You have to enter a username";
            } else if (registerDto.Email == null || !registerDto.Email.Contains("@"))
            {
                error = "You have to enter a valid email address";
            } else if (registerDto.Pwd == null)
            {
                error = "You have to enter a password";
            } else if (await _userRepository.Exists(registerDto.Username) != null)
            {
                error = "The username is already taken";
            } 
            else
            {
                await _userRepository.Register(registerDto.Username, registerDto.Email, registerDto.Pwd,
                    registerDto.Pwd);
            }

            if (error != null)
            {
                return StatusCode(400, new { status = 400, error_msg = error});
            }
            else
            {
                return StatusCode(204);
            }
        }

        // GET: Simulator/msgs
        [HttpGet("msgs")]
        public async Task<IEnumerable<MsgDTO>> Msgs()
        {
            return (await _messageRepository.Get(100))
                .Select(m => new MsgDTO()
                    {
                        Text = m.Text,
                        Pub_date = m.PublishDate.ToString(), //TODO: right format
                        Username = m.Author.Username
                    }
                );
        }
        
        // GET: Simulator/msgs/user
        [HttpGet("msgs/{user}")]
        public async Task<IEnumerable<MsgDTO>> GetMsgs([FromRoute] string user)
        {
            return (await _messageRepository.GetByUser(user, 100))
                .Select(m => new MsgDTO()
                    {
                        Text = m.Text,
                        Pub_date = m.PublishDate.ToString(), //TODO: right format
                        Username = m.Author.Username
                    }
                );
        }
        
        // POST: Simulator/msgs/user
        [HttpPost("msgs/{username}")]
        public async Task<ActionResult> PostMsgs([FromRoute] string username, [FromBody] TweetDTO tweet)
        {
            if (_httpContextAccessor.HttpContext.Request.Headers["Authorization"] !=
                $"Basic {ApiToken}") return StatusCode(403, new { status = 403, error_msg = "You are not authorized to use this resource!"});
            
            var author = await _userRepository.Exists(username);
            if (author == null) return StatusCode(404);
            var message = new Message()
            {
                Author = author,
                Text = tweet.Content,
                PublishDate = DateTime.Now,
                Flagged = 0
            };
            _miniTwitContext.Messages.Add(message);
            await _miniTwitContext.SaveChangesAsync();
            
            await _twitHubContext.Clients.Groups("public", message.Author.Username).SendAsync("ReceiveMessage", new
            {
                Text = message.Text,
                Username = message.Author.Username,
                PublishDate = message.PublishDate.ToString()
            });
            
            return StatusCode(204);
        }
        
        // GET: Simulator/fllws/user
        [HttpGet("fllws/{username}")]
        public async Task<FllwsDTO> GetFllws([FromRoute] string username)
        {
            var fllwsUsers = await _userRepository.GetFollows(username);

            IEnumerable<string> fllws = new List<string>();
            if (fllwsUsers != null)
            {
                fllws = fllwsUsers.Take(100).Select(m => m.Username);
            }
            
            return new FllwsDTO()
            {
                Follows = fllws,
            };
        }
        
        // POST: Simulator/fllws/user
        [HttpPost("fllws/{username}")]
        public async Task<ActionResult> PostFllws([FromRoute] string username, [FromBody] FollowDTO follow)
        {
            if (_httpContextAccessor.HttpContext.Request.Headers["Authorization"] !=
                $"Basic {ApiToken}") return StatusCode(403, new { status = 403, error_msg = "You are not authorized to use this resource!"});

            var user = await _miniTwitContext.Users
                .Include(u => u.Follows)
                .FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return StatusCode(404);
            if (follow.Follow != null)
            {
                var toFollow = await _userRepository.Exists(follow.Follow);
                user.Follows.Add(toFollow);
                await _miniTwitContext.SaveChangesAsync();
            }
            else
            {
                var toUnFollow = await _userRepository.Exists(follow.Unfollow!);
                user.Follows.Remove(toUnFollow);
                await _miniTwitContext.SaveChangesAsync();
            }
            return StatusCode(204);
        }
    }
}
