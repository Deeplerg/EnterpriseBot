using System;
using static EnterpriseBot.Api.Utils.Constants;

namespace EnterpriseBot.Api.Utils
{
    public static class Miscellaneous
    {
        #region user input checks
        /// <summary>
        /// Checks if a name is OK or not
        /// </summary>
        /// <param name="name"></param>
        /// <returns><see cref="bool"/> value determining whether the name has passed verification or not</returns>
        public static bool CheckName(string name)
        {
            if (name.Length > NameMaxLength)
                return false;

            if (string.IsNullOrWhiteSpace(name))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a business name is OK or not
        /// </summary>
        /// <param name="name"></param>
        /// <returns><see cref="bool"/> value determining whether the business name has passed verification or not</returns>
        public static bool CheckBusinessName(string businessName)
        {
            if (businessName.Length > BusinessNameMaxLength)
                return false;

            if (string.IsNullOrWhiteSpace(businessName))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a description is OK or not
        /// </summary>
        /// <param name="description"></param>
        /// <returns><see cref="bool"/> value determining whether the description has passed verification or not</returns>
        public static bool CheckDescription(string description)
        {
            if (description.Length > DescriptionMaxLength)
                return false;

            if (string.IsNullOrWhiteSpace(description))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if the names are the same
        /// </summary>
        /// <returns><see cref="bool"/> value determining whether the names are same or not</returns>
        public static bool CompareNames(string name1, string name2)
        {
            return string.Equals(name1.Trim(), name2.Trim(),
                                 StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }
}
