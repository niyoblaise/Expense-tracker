using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Shared
{
    public class Income
    {
        public Guid Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public decimal? Amount { get; set; }

        [Required]
        public DateTime? Date { get; set; } = DateTime.UtcNow;

        public string? Source { get; set; }
        public string? Notes { get; set; }
    }
}
