﻿using System.ComponentModel.DataAnnotations;

namespace XLocker.Entities
{
    public class Business : BaseEntity
    {

        [Key]
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;

        public string Identification { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}
