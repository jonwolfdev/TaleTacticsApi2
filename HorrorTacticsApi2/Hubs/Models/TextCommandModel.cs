﻿using System.ComponentModel.DataAnnotations;

namespace HorrorTacticsApi2.Hubs.Models
{
    public record TextCommandModel(
        [MinLength(1), MaxLength(5000), Required] string Text);
}