using MyCodeCamp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCodeCamp.Data
{
    public class CampDbInitializer
    {
        private readonly CampContext context;

        public CampDbInitializer(CampContext context)
        {
            this.context = context;
        }

        public async Task Seed()
        {
            if (!context.Camps.Any())
            {
                // Add Data
                context.AddRange(sample);
                await context.SaveChangesAsync();
            }
        }

        private readonly List<Camp> sample = new List<Camp>
        {
            new Camp()
          {
            Name = "Your First Code Camp",
            Moniker = "ATL2016",
            EventDate = DateTime.Today.AddDays(45),
            Length = 1,
            Description = "This is the first code camp",
            Location = new Location()
            {
              Address1 = "123 Main Street",
              CityTown = "Atlanta",
              StateProvince = "GA",
              PostalCode = "30303",
              Country = "USA"
            },
            Speakers = new List<Speaker>
            {
              new Speaker()
              {
                Name = "Shawn Wildermuth",
                Bio = "I'm a speaker",
                CompanyName = "Wilder Minds LLC",
                GitHubName = "shawnwildermuth",
                TwitterName = "shawnwildermuth",
                PhoneNumber = "555-1212",
                HeadShotUrl = "http://wilderminds.com/images/minds/shawnwildermuth.jpg",
                WebsiteUrl = "http://wildermuth.com",
                Talks = new List<Talk>()
                {
                  new Talk()
                  {
                    Title =  "How to do ASP.NET Core",
                    Abstract = "How to do ASP.NET Core",
                    Category = "Web Development",
                    Level = "100",
                    Prerequisites = "C# Experience",
                    Room = "245",
                    StartingTime = DateTime.Parse("14:30")
                  },
                  new Talk()
                  {
                    Title =  "How to do Bootstrap 4",
                    Abstract = "How to do Bootstrap 4",
                    Category = "Web Development",
                    Level = "100",
                    Prerequisites = "CSS Experience",
                    Room = "246",
                    StartingTime = DateTime.Parse("13:00")
                  },
                }
              },
              new Speaker()
              {
                Name = "Resa Wildermuth",
                Bio = "I'm a speaker",
                CompanyName = "Wilder Minds LLC",
                GitHubName = "resawildermuth",
                TwitterName = "resawildermuth",
                PhoneNumber = "555-1212",
                HeadShotUrl = "http://wilderminds.com/images/minds/resawildermuth.jpg",
                WebsiteUrl = "http://wildermuth.com",
                Talks = new List<Talk>()
                {
                  new Talk()
                  {
                    Title =  "Managing a Consulting Business",
                    Abstract = "Managing a Consulting Business",
                    Category = "Soft Skills",
                    Level = "100",
                    Room = "230",
                    StartingTime = DateTime.Parse("10:30")
                  }
                }
              }
            }
          }
        };
    }
}
