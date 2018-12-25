﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vidly.Models;

namespace Vidly.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Enter Customer's Name.")]
        [StringLength(255)]
        public string Name { get; set; }
        
        public bool IsSubscribedNewsLetter { get; set; }

        public byte MembershipTypeId { get; set; }
        public MembershipTypeDto MembershipType { get; set; }

        [MIn18YearsIfaMember]
        public DateTime? DateOfBirth { get; set; }
    }
}