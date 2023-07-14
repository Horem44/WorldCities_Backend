﻿using System.ComponentModel.DataAnnotations;

namespace WorldCities.Core.DTO.Countries
{
    public class CountryAddRequest
    {
        [Required]
        public Guid Guid { get; set; }

        [Required]
        public string Name { get; set; }
    }
}