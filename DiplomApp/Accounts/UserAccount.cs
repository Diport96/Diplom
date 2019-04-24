using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp.Accounts
{
    class UserAccount
    {
        [Key]
        public int ID { get; set; }

        [Required]        
        public string Login { get; set; }

        [Required]
        public byte[] Salt { get; set; }

        [Required]
        public byte[] Key { get; set; }
    }
}
