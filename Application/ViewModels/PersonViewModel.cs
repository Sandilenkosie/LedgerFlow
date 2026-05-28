using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModels
{
    public class PersonViewModel
    {
        public Guid Id { get; set; }
        [DisplayName("Full Name")]
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string IdNumber { get; set; }

    }
}
