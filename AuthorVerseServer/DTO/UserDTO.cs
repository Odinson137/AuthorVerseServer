using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO.CustomValidations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AuthorVerseServer.DTO
{
    public class UserDTO
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
    }

    public class UserLoginDTO
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W)(?!.*\s).{8,}$",
            ErrorMessage = "Password must have at least 1 lowercase letter, 1 uppercase letter, 1 digit, 1 special character, and be at least 8 characters long.")]
        public string Password { get; set; }
    }

    public class UserRegistrationDTO
    {
        [Required(ErrorMessage = "Username is required")]
        //[RequiredMessage("Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W)(?!.*\s).{8,}$",
        ErrorMessage = "Password must have at least 1 lowercase letter, 1 uppercase letter, 1 digit, 1 special character, and be at least 8 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }

    public class UserVerify
    {
        public string UserName { get; set; } = null!;
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? LogoUrl { get; set; } = null!;
    }

    public class UserGoogleVerify
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string? IconUrl { get; set; }
        public string Token { get; set; } = null!;
    }

    public class UserProfile
    {
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string UserPrincipalName { get; set; }
        public string Id { get; set; }
    }
}
