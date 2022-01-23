using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BrokenCode.Model
{
    public class User
    {
        [Key]
        [HiddenInput]
        public Guid Id { get; set; }

        [HiddenInput]
        public Guid DomainId { get; set; }

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        public string UserName { get; set; }

        public bool BackupEnabled { get; set; }

        [EnumDataType(typeof(UserState))]
        public UserState State { get; set; }

        public virtual Email Email { get; set; }

        public virtual Drive Drive { get; set; }

        public virtual Calendar Calendar { get; set; }

    }
}
