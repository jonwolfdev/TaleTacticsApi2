﻿using System.ComponentModel.DataAnnotations;

namespace HorrorTacticsApi2.Hubs.Models
{
    public record GameCodeModel(
        [MinLength(1), MaxLength(50), Required] string GameCode);
}