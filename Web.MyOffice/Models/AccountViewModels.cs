using System.ComponentModel.DataAnnotations;

using Web.MyOffice.Res;

namespace ASE.MVC
{
    /*public class ExternalLoginConfirmationViewModel
    {
        [LocalizedRequired()]
        [Display(Name = "Email")]
        [EmailAddress]
        public string UserName { get; set; }
    }*/

    /*public class ManageUserViewModel
    {
        [LocalizedRequired()]
        [Display(Name = "Имя")]
        public string Email { get; set; }

        [LocalizedRequired()]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string OldPassword { get; set; }

        [LocalizedRequired()]
        [StringLength(100, ErrorMessage = "{0} должен быть не менее {2} знаков.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение")]
        [Compare("NewPassword", ErrorMessage = "Пароль и подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }
    }*/

    public class LoginViewModel
    {
        [LocalizedRequired()]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [LocalizedRequired()]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        //[Display(Name = "Remember me?")]
        //public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [LocalizedRequired()]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [LocalizedRequired()]
        [StringLength(100, ErrorMessage = "{0} должен быть не менее {2} знаков.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [LocalizedDisplay("Password")]
        [LocalizedMinLengthAttribute(6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplay("ConfirmPassword")]
        [Compare("Password", ErrorMessage = "Пароль и подтверждение не совпадают.")]
        [LocalizedMinLengthAttribute(6)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Роли")]
        public string Roles { get; set; }
    }

    /*public class EditViewModel
    {
        public string Id { get; set; }

        [LocalizedRequired()]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение")]
        [Compare("Password", ErrorMessage = "Пароль и подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Роли")]
        public string Roles { get; set; }
    }*/

    public class ResetPasswordViewModel
    {
        [LocalizedRequired()]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [LocalizedRequired()]
        [LocalizedDisplay("ResetPasswordCode")]
        public string Code { get; set; }

        [LocalizedRequired()]
        [DataType(DataType.Password)]
        [LocalizedDisplay("NewPassword")]
        [LocalizedMinLengthAttribute(6)]
        public string NewPassword { get; set; }

        [LocalizedRequired()]
        [DataType(DataType.Password)]
        [LocalizedDisplay("ConfirmPassword")]
        [Compare("NewPassword", ErrorMessage = "Пароль и подтверждение не совпадают.")]
        [LocalizedMinLengthAttribute(6)]
        public string ConfirmPassword { get; set; }

        public string Message { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [LocalizedRequired()]
        [LocalizedDisplay("OldPassword")]
        public string OldPassword { get; set; }

        [LocalizedRequired()]
        [DataType(DataType.Password)]
        [LocalizedDisplay("NewPassword")]
        [LocalizedMinLengthAttribute(6)]
        public string NewPassword { get; set; }

        [LocalizedRequired()]
        [DataType(DataType.Password)]
        [LocalizedDisplay("ConfirmPassword")]
        [LocalizedCompareAttribute("NewPassword")]
        [LocalizedMinLengthAttribute(6)]
        public string ConfirmPassword { get; set; }
    }
}
