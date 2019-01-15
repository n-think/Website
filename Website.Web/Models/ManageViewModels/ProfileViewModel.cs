using System.ComponentModel.DataAnnotations;

namespace Website.Web.Models.ManageViewModels
{
    public class ProfileViewModel
    {
        [Display(Name = "Логин")]
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required(ErrorMessage = "RequiredError")]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }
        
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Display(Name = "Отчество")]
        public string PatrName { get; set; }
        [Display(Name = "Адрес")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [Display(Name = "Город")]
        public string City { get; set; }

        public string StatusMessage { get; set; }
    }
}
