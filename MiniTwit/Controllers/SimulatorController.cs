using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MiniTwit.Database;
using MiniTwit.DTOs;
using MiniTwit.Other_Services;
using MiniTwit.Repositories;
using Prometheus;

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
        private readonly ILogger<SimulatorController> _logger;
        private static string ApiToken = "c2ltdWxhdG9yOnN1cGVyX3NhZmUh"; //Should be in environment variable or something
        private static int _latest = 0;

        
        public SimulatorController(IMessageRepository messageRepository, IUserRepository userRepository, MiniTwitContext miniTwitContext, IHttpContextAccessor httpContextAccessor, IHubContext<TwitHub> twitHubContext, ILogger<SimulatorController> logger)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _miniTwitContext = miniTwitContext;
            _httpContextAccessor = httpContextAccessor;
            _twitHubContext = twitHubContext;
            _logger = logger;
        }
        
        // POST: Simulator/register
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO registerDto, [FromQuery] int? latest)
        {
            UpdateLatest(latest);

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
                _logger.LogError("POST: Simulator/register - Caused the following error: {error}, at {time}", error, DateTime.UtcNow.ToLongTimeString());
                return StatusCode(400, new { status = 400, error_msg = error});
            }
            else
            {
                _logger.LogInformation("POST: Simulator/register - Successfully registered user, at {time}", DateTime.UtcNow.ToLongTimeString());
                return StatusCode(204);
            }
        }

        // GET: Simulator/msgs
        [HttpGet("msgs")]
        public async Task<IEnumerable<MsgDTO>> Msgs([FromQuery] int? latest, [FromQuery] int no = 100)
        {
            UpdateLatest(latest);
            
            return (await _messageRepository.Get(no))
                .Select(m => new MsgDTO()
                    {
                        Content = m.Text,
                        Pub_date = m.PublishDate.ToString(), //TODO: right format
                        User = m.Author.Username
                    }
                );
        }
        
        // GET: Simulator/msgs/user
        [HttpGet("msgs/{user}")]
        public async Task<IActionResult> GetMsgs([FromRoute] string user, [FromQuery] int? latest, [FromQuery] int no = 100)
        {
            UpdateLatest(latest);

            if (await _userRepository.Exists(user) == null)
            {
                _logger.LogError("GET: Simulator/msgs/user - Caused the following error: User does not exist, at {time}", DateTime.UtcNow.ToLongTimeString());
                return StatusCode(404);
            }
            
            _logger.LogInformation("GET: Simulator/msgs/user - Getting message from user, at {time}", DateTime.UtcNow.ToLongTimeString());
            return Ok((await _messageRepository.GetByUser(user, no))
                .Select(m => new MsgDTO()
                    {
                        Content = m.Text,
                        Pub_date = m.PublishDate.ToString(), //TODO: right format
                        User = m.Author.Username
                    }
                ));
        }
        
        // POST: Simulator/msgs/user
        [HttpPost("msgs/{username}")]
        public async Task<ActionResult> PostMsgs([FromRoute] string username, [FromBody] TweetDTO tweet, [FromQuery] int? latest)
        {
            UpdateLatest(latest);

            if (_httpContextAccessor.HttpContext.Request.Headers["Authorization"] != $"Basic {ApiToken}")
            {
                var error = new String("You are not authorized to use this resource!");
                _logger.LogError("POST: Simulator/msgs/user - Caused the following error: {error} , logged at {time}", error, DateTime.UtcNow.ToLongTimeString());
                return StatusCode(403, new { status = 403, error_msg = error});
            }
            
            var author = await _userRepository.Exists(username);
            if (author == null)
            {
                _logger.LogError("POST: Simulator/msgs/user - Caused the following error: Author does not exist, logged at {time}", DateTime.UtcNow.ToLongTimeString());
                return StatusCode(404);
            }
            var message = new Message
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
                PublishDate = message.PublishDate.Value.AddHours(1).ToString()
            });
            _logger.LogInformation("POST: Simulator/msgs/user - Successfully created message at {time}", DateTime.UtcNow.ToLongTimeString());
            return StatusCode(204);
        }
        
        // GET: Simulator/fllws/user
        [HttpGet("fllws/{username}")]
        public async Task<IActionResult> GetFllws([FromRoute] string username, [FromQuery] int? latest, [FromQuery] int no = 100)
        {
            UpdateLatest(latest);

            var fllwsUsers = await _userRepository.GetFollows(username);

            if (fllwsUsers == null)
            {
                _logger.LogError("GET: Simulator/fllws/user - Caused the following error: User does not exist, logged at {time}", DateTime.UtcNow.ToLongTimeString());
                return StatusCode(404);
            }
            
            _logger.LogInformation("GET: Simulator/fllws/user - Get {username} followers was successfully, logged at {time}", username, DateTime.UtcNow.ToLongTimeString());
            return Ok(new FllwsDTO()
            {
                Follows = fllwsUsers.Take(no).Select(m => m.Username),
            });
        }
        
        // POST: Simulator/fllws/user
        [HttpPost("fllws/{username}")]
        public async Task<ActionResult> PostFllws([FromRoute] string username, [FromBody] FollowDTO follow, [FromQuery] int? latest)
        {
            UpdateLatest(latest);

            if (_httpContextAccessor.HttpContext.Request.Headers["Authorization"] != $"Basic {ApiToken}")
            {
                var error = new String("You are not authorized to use this resource!");
                _logger.LogError("POST: Simulator/msgs/user - Caused the following error: {error} , logged at {time}", error, DateTime.UtcNow.ToLongTimeString());
                return StatusCode(403, new { status = 403, error_msg = error});
            }

            var user = await _miniTwitContext.Users
                .Include(u => u.Follows)
                .FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                _logger.LogError("POST: Simulator/fllws/user - Get {username} caused error: User does not exist, at {time}", username, DateTime.UtcNow.ToLongTimeString());
                return StatusCode(404);
            }
            if (follow.Follow != null)
            {
                var toFollow = await _userRepository.Exists(follow.Follow);
                if (toFollow == null) return StatusCode(404);
                user.Follows.Add(toFollow);
                await _miniTwitContext.SaveChangesAsync();
            }
            else
            {
                var toUnFollow = await _userRepository.Exists(follow.Unfollow!);
                if (toUnFollow == null) {return StatusCode(404);}
                user.Follows.Remove(toUnFollow);
                await _miniTwitContext.SaveChangesAsync();
            }
            _logger.LogInformation("POST: Simulator/fllws/user - Follow to {username} was successfully, logged at {time}", username, DateTime.UtcNow.ToLongTimeString());
            return StatusCode(204);
        }

        [HttpGet("latest")]
        public async Task<LatestDTO> GetLatest()
        {
            _logger.LogInformation("GET: Simulator/latest - Get latest was successfully, logged at {time}", DateTime.UtcNow.ToLongTimeString());
            return new LatestDTO { latest = _latest };
        }

        private void UpdateLatest(int? latest)
        {
            if (latest != null)
            {
                _logger.LogInformation("UpdatingLatest - Update of latest value was successfully, logged at {time}", DateTime.UtcNow.ToLongTimeString());
                _latest = latest.Value;
            }
        }
    }
}
