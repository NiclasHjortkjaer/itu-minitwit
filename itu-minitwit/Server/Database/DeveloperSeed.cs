namespace itu_minitwit.Server.Database;

public static class DeveloperSeed
{
    public static void SeedDatabase(this MinitwitContext minitwitContext)
    {
        if (minitwitContext.Users.Count() == 0)
        {
            var user1 = new User() { Username = "jason", Email = "jason@derulo.com", PwHash = "asdjl", Follows = new List<User>(), Followers = new List<User>()};
            var user2 = new User() { Username = "mark", Email = "mark@mail.com", PwHash = "HASHAHASHAS", Follows = new List<User>(), Followers = new List<User>()};
            var user3 = new User() { Username = "Birgitte", Email = "Birgitte@gmail.com", PwHash = "hahahashshshsh", Follows = new List<User>(),Followers = new List<User>()};
            
            minitwitContext.Users.Add(user1);
            minitwitContext.Users.Add(user2);
            minitwitContext.Users.Add(user3);
            user1.Follows.Add(user2);
            user1.Follows.Add(user3);

            var message1 = new Message() { Author = user1, Text = "hej hej", PublishDate = DateTime.Today.Subtract(TimeSpan.FromDays(1)), Flagged = 0};
            var message2 = new Message() { Author = user1, Text = "hej hej part 2", PublishDate = DateTime.Today, Flagged = 0};
            var message3 = new Message() { Author = user1, Text = "undskyld jeg spammer", PublishDate = DateTime.Now, Flagged = 0 };
            var message4 = new Message() { Author = user2, Text = "hold", PublishDate = DateTime.MinValue, Flagged = 1 };

            var message5 = new Message() { Author = user1, Text = "We keep losing rounds because of this dogshit ESEA client. Lagging out mid duels", PublishDate = DateTime.Today.Subtract(TimeSpan.FromDays(1)), Flagged = 0 };
            var message6 = new Message() { Author = user1, Text = "I'm 5'9\" but insecure about my height. So I've always worn 'lifts' in my shoes. With a pair of Nike Air Forces, I'm 6'. I met my girlfriend with these lifts in. Now I can't take my shoes off around her. I've kept my shoes on for almost two years now.", PublishDate = DateTime.Today.Subtract(TimeSpan.FromDays(0)), Flagged = 0 };
            var message7 = new Message() { Author = user1, Text = "Mærsk beviser at en lav skattebyrde fører til, at globale virksomheder kan vokse sig store i Danmark. \r\n\r\n4% af 200 mia. er større end 22% af 200 mio.", PublishDate = DateTime.Today.Subtract(TimeSpan.FromDays(18)), Flagged = 0 };
            var message8 = new Message() { Author = user1, Text = "Chinese Rap Song ‘Fuck Off Stupid Foreigners’", PublishDate = DateTime.Today.Subtract(TimeSpan.FromDays(1)), Flagged = 0 };
            var message9 = new Message() { Author = user1, Text = "“You’re trying to make people feel bad for purchasing a video game?”\r\n\r\nxQc defends streamers that are broadcasting and playing Hogwarts Legacy", PublishDate = DateTime.Today.Subtract(TimeSpan.FromDays(1)), Flagged = 0 };
            var message10 = new Message() { Author = user1, Text = "I think its hilarious u kids talking about jabbi. u wouldnt say this stuff to him at lan, hes jacked. not only that but he wears the freshest clothes, eats at the chillest restaurants and hangs out with the hottest dudes. yall are pathetic lol", PublishDate = DateTime.Today.Subtract(TimeSpan.FromDays(-5)), Flagged = 0 };
            var message11 = new Message() { Author = user1, Text = "My 3-year-old said she wished we had a pet. I reminded her we have a dog and wow the genuine surprise on her face as it dawned on her that our dog is a pet and not just some other guy who lives here.", PublishDate = DateTime.Today.Subtract(TimeSpan.FromDays(10)), Flagged = 0 };
            var message12 = new Message() { Author = user1, Text = "En borgerhenvendelse fortæller mig, at stadig flere bosteder på det specialiserede område er ejet og drevet af folkene bag Tvind-imperiet. Endda at der kanaliseres penge til Sydamerika. Nogen der kan be- eller afkræfte..? #kommunal #handicapområdet #økonomi #dkpol #tvind", PublishDate = DateTime.Today.Subtract(TimeSpan.FromDays(1)), Flagged = 0 };

            minitwitContext.Messages.Add(message1);
            minitwitContext.Messages.Add(message2);
            minitwitContext.Messages.Add(message3);
            minitwitContext.Messages.Add(message4);
            minitwitContext.Messages.Add(message5);
            minitwitContext.Messages.Add(message6);
            minitwitContext.Messages.Add(message7);
            minitwitContext.Messages.Add(message8);
            minitwitContext.Messages.Add(message9);
            minitwitContext.Messages.Add(message10);
            minitwitContext.Messages.Add(message11);
            minitwitContext.Messages.Add(message12);

            minitwitContext.SaveChanges();
        }
    }
}