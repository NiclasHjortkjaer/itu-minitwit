namespace MiniTwit.Database;

public static class DeveloperSeed
{
    public static void SeedDatabase(this MiniTwitContext minitwitContext)
    {
        if (minitwitContext.Users.Count() == 0)
        {
            var user1 = new User() { Username = "jason", Email = "jason@derulo.com", PwHash = "asdjl", Follows = new HashSet<User>(), Followers = new HashSet<User>(), Salt = "salt"};
            var user2 = new User() { Username = "mark", Email = "mark@mail.com", PwHash = "HASHAHASHAS", Follows = new HashSet<User>(), Followers = new HashSet<User>(), Salt = "SALTTTT"};
            var user3 = new User() { Username = "Birgitte", Email = "Birgitte@gmail.com", PwHash = "hahahashshshsh", Follows = new HashSet<User>(),Followers = new HashSet<User>(), Salt = "ABCDEFG"};
            
            minitwitContext.Users.Add(user1);
            minitwitContext.Users.Add(user2);
            minitwitContext.Users.Add(user3);
            user1.Follows.Add(user2);
            user1.Follows.Add(user3);

            var message1 = new Message() { Author = user1, Text = "hej hej", PublishDate = DateTime.Today.Subtract(TimeSpan.FromDays(1)), Flagged = 0};
            var message2 = new Message() { Author = user1, Text = "hej hej part 2", PublishDate = DateTime.Today, Flagged = 0};
            var message3 = new Message() { Author = user1, Text = "undskyld jeg spammer", PublishDate = DateTime.Now, Flagged = 0 };
            var message4 = new Message() { Author = user2, Text = "hold", PublishDate = DateTime.MinValue, Flagged = 1 };

            minitwitContext.Messages.Add(message1);
            minitwitContext.Messages.Add(message2);
            minitwitContext.Messages.Add(message3);
            minitwitContext.Messages.Add(message4);

            minitwitContext.SaveChanges();
        }
    }
}