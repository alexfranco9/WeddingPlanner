
using System;
using System.ComponentModel.DataAnnotations;


namespace WeddingPlanner.Models {
    public class LoginUser {

        [Required (ErrorMessage = "Email is required.")]
        [EmailAddress]
        [Display (Name = "Email Address:")]
        public string LoginEmail { get; set; }

        [Required (ErrorMessage = "Password is required.")]
        [DataType (DataType.Password)]
        [Display (Name = "Password:")]
        public string LoginPassword { get; set; }
    }
}