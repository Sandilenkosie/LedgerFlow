using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModels
{
    public class RegisterViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        public string PasswordHash { get; set; } = null!;
        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("PasswordHash", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = null!;
    }

    public class LoginViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        public string PasswordHash { get; set; } = null!;
    }

}
