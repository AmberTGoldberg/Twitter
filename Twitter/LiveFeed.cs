using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter
{
    //******************************************************
    // Tweet Object
    public class Tweet
    {
        public string Description { get; set; }
        public Person Person { get; set; }
        public int Order { get; set; }
    }

    // Person Object
    public class Person
    {
        public string Name { get; set; }

        public List<string> Following { get; set; }
    }

    // TwitterFeed Object
    public class TwitterFeed
    {
        public string Name { get; set; }
        public List<string> AllTweets { get; set; }
    }
    //******************************************************

    public class LiveFeed
    {
        public LiveFeed()
        {

        }

        public Queue<TwitterFeed> CreateLiveFeed(List<string> peopleInput, List<string> tweetsInput)
        {
            var people = this.CreatePeople(peopleInput).OrderBy(x => x.Name).ToList();

            var tweets = this.CreateTweets(people, tweetsInput).OrderBy(x => x.Order).ToList();

            var liveFeed = this.CreateLiveFeed(people, tweets);

            return liveFeed;
        }

        // Go through list of users and the people they are following
        // Create new person and followers
        private List<Person> CreatePeople(List<string> peopleInput)
        {
            var people = new List<Person>();

            foreach (var follow in peopleInput)
            {

                var tempPerson = new Person();

                var separation = follow.Split(new string[] { "follows" }, StringSplitOptions.None);

                // check that the string has been split correctly
                // if the string has been split correctly by the word follows the user and the people it follows will be created
                if (separation.Length > 1)
                {
                    var followSeparation = separation[1].Split(',');

                    // check is person already exists
                    // if it exists, add the people it follows
                    var existingPerson = people.FirstOrDefault(x => x.Name == separation[0]);
                    if (existingPerson != null)
                    {
                        existingPerson.Following.AddRange(followSeparation);
                        existingPerson.Following = existingPerson.Following.Distinct().ToList();
                        continue;
                    }
                    else
                    {
                        // create a new person
                        tempPerson.Name = separation[0].Trim();
                        tempPerson.Following = new List<string>();
                        tempPerson.Following.AddRange(followSeparation);
                    }

                    people.Add(tempPerson);
                }

            }


            return people;
        }

        // Goes through the list of tweets and assigns a person to a tweet
        // adds a number to the tweet which is the order the tweets were "written"
        private List<Tweet> CreateTweets(List<Person> people, List<string> tweetsInput)
        {
            var tweets = new List<Tweet>();
            var count = 0; // count to keep track of the order of the tweets

            foreach (var tweet in tweetsInput)
            {
                var separation = tweet.Split('>').ToArray();

                // check that the string has been split correctly by >
                // if it is not split correctly the tweets are not added
                if (separation.Length > 1)
                {
                    count++;

                    var actualTweet = new Tweet()
                    {
                        Person = people.FirstOrDefault(x => x.Name.Trim() == separation[0].Trim()),
                        Order = count
                    };


                    // check for 140 charachters
                    // if twwet in the character range, add the whole tweet to the list
                    if (separation[1].Length <= 140)
                    {
                        actualTweet.Description = separation[1];

                    }

                    // if the tweet is greater than 140 characters
                    else if (separation[1].Length > 140)
                    {
                        actualTweet.Description = separation[1].Substring(0, 140);
                    }

                    tweets.Add(actualTweet);
                }

            }

            return tweets;
        }

        // Goes through people list and tweet list and creates the twitter feed
        private Queue<TwitterFeed> CreateLiveFeed(List<Person> people, List<Tweet> tweets)
        {
            var twitterFeedQueue = new Queue<TwitterFeed>();
            
            // add all names
            var users = people.Select(x => x.Name).ToList();

            // add all following
            users.AddRange(people.SelectMany(x => x.Following));

            // sort users alphabetically
            users = users.Select(x => x.Trim()).Distinct().OrderBy(x => x).ToList();
            
            // Go through the list of people in alphabetical order
            // add each person to the twitter feed
            // enqueue the twitter feed in order
            // for each person add either their own tweets or the people they are followings' tweets
            foreach (var user in users)
            {
                var feed = new TwitterFeed();
                var allTweets = new List<string>();

                var person = people.FirstOrDefault(x => x.Name == user);

                feed.Name = user;

                if (person != null)
                {
                    foreach (var tweet in tweets)
                    {
                        //check if it's person's tweet
                        if (tweet.Person != null)
                        {
                            if (tweet.Person.Name.Trim() == person.Name.Trim())
                            {
                                allTweets.Add("@" + tweet.Person.Name.Trim() + ": " + tweet.Description.Trim());
                            }

                            // check for the people followings' tweet                        
                            var followingTweet = person.Following.FirstOrDefault(x => x.Trim() == tweet.Person.Name.Trim());
                            if (followingTweet != null)
                            {
                                allTweets.Add("@" + tweet.Person.Name.Trim() + ": " + tweet.Description.Trim());
                            }
                        }
                    }

                    feed.AllTweets = allTweets;
                }
                twitterFeedQueue.Enqueue(feed);


            }

            return twitterFeedQueue;
        }

    }
}
