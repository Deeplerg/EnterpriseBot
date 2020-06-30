using System;

namespace EnterpriseBot.Api.Models.Common.Enums
{
    [Flags]
    public enum RecipeCanBeDoneBy
    {
        Player    = 0b0000_0001,
        Robot     = 0b0000_0010
    }
}
