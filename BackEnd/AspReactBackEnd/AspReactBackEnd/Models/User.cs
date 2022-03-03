namespace AspReactBackEnd.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User : BaseEntity
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        [Required(ErrorMessage = "Name field is required.")]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        public string Name { get; set; }
        [Column("email")]
        [Required(ErrorMessage = "Email field is required.")]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        [EmailAddress]
        public string Email { get; set; }

        [Column("password")]
        [Required(ErrorMessage = "Password field is required.")]
        public string Password { get; set; }

        [Column("phone")]
        [Required(ErrorMessage = "Phone field is required.")]
        [StringLength(maximumLength: 15, MinimumLength = 10)]
        public string Phone { get; set; }

        [Column("address")]
        [StringLength(maximumLength: 250)]
        public string? Address { get; set; }

        [Column("avatar")]
        public string? Avatar { get; set; }
    }
}
