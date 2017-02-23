using System;
using System.Collections.Generic;
using System.Linq;

namespace DapperDemo
{
    public class TestData
    {
        public static IList<Dog> GetDogs()
        {
            return new List<Dog>
            {
                new Dog { DogId = 1, Name = "Fido", Breed = "Mongrel", Age = 2 },
                new Dog { DogId = 2, Name = "Lassie", Breed = "St. Bernard", Age = 7 },
                new Dog { DogId = 3, Name = "Shep", Breed = "Sheepdog", Age = 14 },
                new Dog { DogId = 4, Name = "Lassie", Breed = "Irish Wolfhound", Age = 3 },
            };
        }

        public static Dog GetDogs(int id)
        {
            return GetDogs().First(dog => dog.DogId == id);
        }

        public static IList<Dog> GetDogs(string name)
        {
            return GetDogs().Where(dog => dog.Name == name).ToList();
        }

        public static string GetInsertScriptFor(IList<Dog> dogs)
        {
            List<string> columnQueries = new List<string>();
            foreach (Dog dog in dogs)
            {
                columnQueries.Add($"{dog.DogId}, '{dog.Name}', '{dog.Breed}', {dog.Age}");
            }

            return
                "SET IDENTITY_INSERT dbo.Dog ON;" +
                "INSERT dbo.Dog (DogId, Name, Breed, Age) SELECT " + String.Join(" UNION SELECT ", columnQueries) +
                "SET IDENTITY_INSERT dbo.Dog OFF;";
        }
    }
}