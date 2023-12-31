﻿using System.ComponentModel.DataAnnotations;

namespace Core.Services.Form.Requests
{
    public class FormRequest
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Ordinal { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; } = 0;
        public int ModifiedBy { get; set; } = 0;
    }
}
