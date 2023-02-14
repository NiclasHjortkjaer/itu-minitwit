using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniTwit.DTOs;
using MiniTwit.Repositories;

namespace MiniTwit.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SimulatorController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;

        public SimulatorController(IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }
        
        // POST: Simulator/register
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDTO registerDto)
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
            return (await _messageRepository.Get())
                .Take(100)
                .Select(m => new MsgDTO()
                    {
                        Text = m.Text,
                        Pub_date = m.PublishDate.ToString(), //TODO: right format
                        Username = m.Author.Username
                    }
                );
        }
    }
}
