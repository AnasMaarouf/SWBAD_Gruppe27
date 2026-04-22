using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public class ApiUser : IdentityUser {
    public string? FullName { get; set; }
}