using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    public class Team
    {
        public Team()
        {
            Players = new HashSet<Player>();
            HomeGames = new HashSet<Game>();
            AwayGames = new HashSet<Game>();
        }

        [Key]
        public int TeamId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(2048)]
        public string LogoUrl { get; set; }

        [Required]
        [MaxLength(4)]
        public string Initials { get; set; }

        [Required]
        public decimal Budget { get; set; }


        [ForeignKey(nameof(PrimaryKitColor))]
        public int PrimaryKitColorId { get; set; }
        public virtual Color PrimaryKitColor { get; set; }

        [ForeignKey(nameof(SecondaryKitColor))]
        public int SecondaryKitColorId { get; set; }
        public virtual Color SecondaryKitColor { get; set; }

        [ForeignKey(nameof(Town))]
        public int TownId { get; set; }
        public virtual Town Town { get; set; }

        public virtual ICollection<Player> Players { get; set; }

        [InverseProperty("HomeTeam")]
        public virtual ICollection<Game> HomeGames { get; set; }

        [InverseProperty("AwayTeam")]
        public virtual ICollection<Game> AwayGames { get; set; }
    }
}
