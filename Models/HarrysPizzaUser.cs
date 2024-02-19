using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HarrysPizza.Models
{
    public class HarrysPizzaUser: IdentityUser
    {

        [PersonalData, Required, StringLength(30)]
        public string FirstName { get; set; }

        [PersonalData, Required, StringLength(60)]
        public string Surname { get; set; }

        public string Name { get { return $"{FirstName} {Surname}"; } }
    }
}
