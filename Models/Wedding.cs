using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models {
    public class Wedding {

        [Key]
        public int WeddingId { get; set; }

        [Required (ErrorMessage = "Wedder one is required.")]
        [MinLength (2, ErrorMessage = "Wedder one must be at least 2 characters long.")]
        [Display (Name = "Wedder One:")]
        public string WedderOne { get; set; }

        [Required (ErrorMessage = "Wedder two is required.")]
        [MinLength (2, ErrorMessage = "Wedder two must be at least 2 characters long.")]
        [Display (Name = "Wedder Two:")]
        public string WedderTwo { get; set; }

        [Required (ErrorMessage = "Wedding date is required.")]
        [Display (Name = "Wedding Date:")]
        public DateTime WeddingDate { get; set; }

        [Required (ErrorMessage = "Street address is required.")]
        [MinLength (5, ErrorMessage = "Street address must be at least 5 characters long.")]
        [Display (Name = "Street Address:")]
        public string Street { get; set; }

        [Required (ErrorMessage = "City is required.")]
        [MinLength (2, ErrorMessage = "City must be at least 2 characters.")]
        [Display (Name = "City:")]

        public string City { get; set; }

        [Required (ErrorMessage = "State is required.")]
        [Display (Name = "State:")]

        public string State { get; set; }

        [Required (ErrorMessage = "Zip code is required.")]
        [MinLength (5, ErrorMessage = "Zip code must be at least 5 characters long.")]
        [Display (Name = "Zip code:")]

        public string Zip { get; set; }

        public int UserId { get; set; }

        public User Creator { get; set; }

        public List<RSVP> Guests { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}