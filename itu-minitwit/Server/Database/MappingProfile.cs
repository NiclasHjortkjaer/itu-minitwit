using System;
using AutoMapper;
using itu_minitwit.Server.Database;
using itu_minitwit.Shared;

namespace ituminitwit.Server.Database;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<Message, MessageDto>();
        CreateMap<MessageDto, Message>();
        CreateMap<UserDto, User>();
        CreateMap<User, UserDto>();
    }
}


