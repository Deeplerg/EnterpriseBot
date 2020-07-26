using System;

namespace EnterpriseBot.ApiWrapper.Models.Enums
{
    [Flags]
    public enum RecipeCanBeDoneBy
    {
        Player    = 0b0000_0001,
        Robot     = 0b0000_0010
    }
}
