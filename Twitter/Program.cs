using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter
{
    class Program
    {
        static void Main(string[] args)
        {
            var liveFeed = new LiveFeed();

            try
            {
                // Read in the file
                var tweetFile = string.Empty;
                var peopleFile = string.Empty;

                if (args.Any())
                {
                    peopleFile = args[0];
                    tweetFile = args[1];
                }
                
                string[] people = System.IO.File.ReadAllLines(peopleFile);
                string[] tweets = System.IO.File.ReadAllLines(tweetFile);
                
                // Call method to process information
                var results = liveFeed.CreateLiveFeed(people.ToList(), tweets.ToList());

                // Display file
                while (results.Count > 0)
                {
                    var tempFeed = results.Dequeue();
                    Console.WriteLine(tempFeed.Name);

                    if (tempFeed.AllTweets != null)
                    {
                        foreach (var tweet in tempFeed.AllTweets)
                        {
                            Console.WriteLine("\t" + tweet);
                        }
                    }
                }

                Console.Read();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: "+ e.Message);
            }

        }
    }
}
