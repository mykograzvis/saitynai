﻿using Ligonine.Auth.Model;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Ligonine.Data.Models
{
    public class Session
    {
        public Guid Id { get; set; }
        public string LastRefreshToken { get; set; }
        public DateTimeOffset InitiatedAt { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        [Required]
        public required string UserId { get; set; }
        public ForumRestUser User { get; set; }

        }
    }
