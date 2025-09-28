using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Shared
{
    public class Expense
    {

        public Guid Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public decimal? Amount { get; set; }
        [Required]
        public DateTime? Date { get; set; } = DateTime.UtcNow;
        public string? Category { get; set; }
        public string? Notes { get; set; }

    }
}
