using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    public class User
    {
        public User()
        {
            Bets = new HashSet<Bet>();
        }

        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(30)]
        public string Username { get; set; }

        [Required]
        [MaxLength(90)]
        public string Password { get; set; }

        [Required]
        [MaxLength(40)]
        public string Email { get; set; }

        [Required]
        [MaxLength(75)]
        public string Name { get; set; }

        [Required]
        public decimal Balance { get; set; }

        public virtual ICollection<Bet> Bets { get; set; }
    }
}
