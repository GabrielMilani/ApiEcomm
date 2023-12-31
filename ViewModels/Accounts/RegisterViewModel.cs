﻿using System.ComponentModel.DataAnnotations;

namespace ApiEcomm.ViewModels.Accounts
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail e inválido.")]
        public string Email { get; set; }
    }
}
