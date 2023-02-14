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

        public SimulatorController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        
        // GET: Simulator
        [HttpGet("register")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
