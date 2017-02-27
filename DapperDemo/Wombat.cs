using System.ComponentModel.DataAnnotations.Schema;

namespace DapperDemo
{
    public class Wombat
    {
        [Column("WombatId")]
        public int Id { get; set; }
        public string Name { get; set; }
        [Column("GeographicalAddress")]
        public string Address { get; set; }
        [Column("Rating")]
        public int Cuteness { get; set; }
    }

}