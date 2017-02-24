namespace DapperDemo
{
    public abstract class AnimalDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Wombat : AnimalDTO
    {
        public string Address { get; set; }
        public int Cuteness { get; set; }
    }

}