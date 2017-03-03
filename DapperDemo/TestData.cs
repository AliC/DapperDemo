using System;
using System.Collections;
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

        public static IList<Wombat> GetWombats()
        {
            return new List<Wombat>
            {
                new Wombat { Id = 1, Name = "Harry", Address = "Wombat State Forest", Cuteness = 3 },
                new Wombat { Id = 2, Name = "Rosie", Address = "Wombat Hill", Cuteness = 8 },
                new Wombat { Id = 3, Name = "McTavish", Address = "Wombat", Cuteness = 4 },
                new Wombat { Id = 4, Name = "Sleepy", Address = " Adelaide Zoo", Cuteness = 10 },
            };
        }

        public static IList<Dodo> GetDodos()
        {
            return new List<Dodo>
            {
                new Dodo { Id = 1, Name = "Anthony", GameStatistics = new GameStatistics { BitePower = 19, Cuteness = 1, Speed = 8 } },
                new Dodo { Id = 2, Name = "Samantha", GameStatistics = new GameStatistics { BitePower = 2, Cuteness = 10, Speed = 20 } },
                new Dodo { Id = 3, Name = "Norris", GameStatistics = new GameStatistics { BitePower = 9, Cuteness = 18, Speed = 12 } },
                new Dodo { Id = 4, Name = "Tommy", GameStatistics = new GameStatistics { BitePower = 16, Cuteness = 14, Speed = 15 } },
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
            List<string> columnQueries = GetColumnQueries(dogs);
            
            return WrapWithSetIdentity("Dog", "INSERT dbo.Dog (DogId, Name, Breed, Age) SELECT " + String.Join(" UNION SELECT ", columnQueries));
        }

        public static string GetInsertScriptFor(IList<Wombat> wombats)
        {
            List<string> columnQueries = GetColumnQueries(wombats);

            return WrapWithSetIdentity("Wombat", "INSERT dbo.Wombat (WombatId, Name, GeographicalAddress, Rating) SELECT " + String.Join(" UNION SELECT ", columnQueries));
        }

        public static string GetInsertScriptFor(IList<Dodo> dodos)
        {
            List<string> columnQueriesForDodos = GetColumnQueries(dodos);
            string script = WrapWithSetIdentity("Dodo", "INSERT dbo.Dodo (DodoId, Name) SELECT " + String.Join(" UNION SELECT ", columnQueriesForDodos));

            List<string> columnQueriesForGameStatistics = GetColumnQueries(dodos.Select((d, i) => new { DodoId = i + 1, d.GameStatistics }));
            script += "\nGO\nINSERT dbo.GameStatistics (DodoId, BitePower, Cuteness, Speed) SELECT " + String.Join(" UNION SELECT ", columnQueriesForGameStatistics);

            return script;
        }

        private static List<string> GetColumnQueries(IList<Dog> dogs)
        {
            List<string> columnQueries = new List<string>();
            foreach (Dog dog in dogs)
            {
                columnQueries.Add($"{dog.DogId}, '{dog.Name}', '{dog.Breed}', {dog.Age}");
            }

            return columnQueries;
        }

        private static List<string> GetColumnQueries(IList<Wombat> wombats)
        {
            List<string> columnQueries = new List<string>();
            foreach (Wombat wombat in wombats)
            {
                columnQueries.Add($"{wombat.Id}, '{wombat.Name}', '{wombat.Address}', {wombat.Cuteness}");
            }

            return columnQueries;
        }

        private static List<string> GetColumnQueries(IList<Dodo> dodos)
        {
            List<string> columnQueries = new List<string>();
            foreach (Dodo dodo in dodos)
            {
                columnQueries.Add($"{dodo.Id}, '{dodo.Name}'");
            }

            return columnQueries;
        }

        private static List<string> GetColumnQueries(IEnumerable<dynamic> gameStatistics)
        {
            List<string> columnQueries = new List<string>();
            foreach (dynamic stat in gameStatistics)
            {
                columnQueries.Add($"{stat.DodoId}, {stat.GameStatistics.BitePower}, {stat.GameStatistics.Cuteness}, {stat.GameStatistics.Speed}");
            }

            return columnQueries;
        }

        private static string WrapWithSetIdentity(string tableName, string insertStatement)
        {
            return
                $"SET IDENTITY_INSERT dbo.{tableName} ON;\n" +
                insertStatement +
                $"\nSET IDENTITY_INSERT dbo.{tableName} OFF;\n";
        }
    }
}