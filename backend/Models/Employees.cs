using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public Guid PublicId { get; set; } = Guid.NewGuid();

        // --- ADDED for Soft Deletes ---
        public bool IsActive { get; set; } = true;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? JobTitle { get; set; }

        [MaxLength(1024)]
        public string? ResumeS3Key { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Foreign Keys
        public int VendorId { get; set; }
        public int JobId { get; set; }
        public int CreatedByUserId { get; set; }
        public int? UpdatedByUserId { get; set; }

        // Navigation Properties
        [ForeignKey("VendorId")]
        public virtual Vendor Vendor { get; set; } = null!;

        [ForeignKey("CreatedByUserId")]
        public virtual User CreatedByUser { get; set; } = null!;

        [ForeignKey("JobId")] // ADDED
        public virtual Job Job { get; set; } = null!;
    }
}