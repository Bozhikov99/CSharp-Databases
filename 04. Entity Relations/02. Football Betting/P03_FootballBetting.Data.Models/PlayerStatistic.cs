using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    public class PlayerStatistic
    {
        public int GameId { get; set; }

        public int PlayerId { get; set; }

        [Required]
        public byte ScoredGoals { get; set; }

        [Required]
        public byte Assists { get; set; }

        public byte MinutesPlayed { get; set; }
    }
}
