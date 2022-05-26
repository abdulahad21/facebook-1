using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class CommentModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Post ID: ")]
        public int PostID { get; set; }
        [Display(Name = "First Name: ")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name: ")]
        public string LastName { get; set; }

        [Display(Name = "Email: ")]
        public string Email { get; set; }

        [Display(Name = "Comment Date Time: ")]
        public DateTime CommentDateTime { get; set; }

        [Display(Name = "Comment: ")]
        public string Comment { get; set; }

    }
}
