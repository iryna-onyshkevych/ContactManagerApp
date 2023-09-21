using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ContactManagerApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "DateOfBirth is required!")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Married info is required!")]
        public bool Married { get; set; }

        [Required(ErrorMessage = "Phone number is required!")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Salary info is required!")]
        public decimal Salary { get; set; }
    }
}
