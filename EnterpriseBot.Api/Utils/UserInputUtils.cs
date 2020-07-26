using System;
using System.Linq;
using static EnterpriseBot.Api.Utils.Constants;

namespace EnterpriseBot.Api.Utils
{
    public static class UserInputUtils
    {
        /// <summary>
        /// Checks if a name is OK or not
        /// </summary>
        /// <param name="name"></param>
        /// <returns><see cref="bool"/> value determining whether the name has passed verification or not</returns>
        public static bool CheckName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (name.Length > NameMaxLength)
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
            if (string.IsNullOrWhiteSpace(businessName))
                return false;

            if (businessName.Length > BusinessNameMaxLength)
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
            if (string.IsNullOrWhiteSpace(description))
                return false;

            if (description.Length > DescriptionMaxLength)
                return false;

            return true;
        }

        /// <summary>
        /// Used for review text verification
        /// </summary>
        /// <param name="text">Text to check</param>
        /// <returns><see cref="bool"/> value determining whether the text has passed verification or not</returns>
        public static bool CheckReviewText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            if (text.Length > ReviewTextMaxLength)
                return false;

            return true;
        }

        /// <summary>
        /// Used for resume verification
        /// </summary>
        /// <param name="resume">Resume to check</param>
        /// <returns><see cref="bool"/> value determining whether the resume has passed verification or not</returns>
        public static bool CheckResume(string resume)
        {
            if (string.IsNullOrWhiteSpace(resume))
                return false;

            if (resume.Length > ResumeMaxLength)
                return false;

            return true;
        }

        /// <summary>
        /// Checks whether a password is secure or not
        /// </summary>
        /// <param name="password">Raw unencrypted password</param>
        /// <returns>Is the password reliable or not</returns>
        public static bool CheckPasswordReliability(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < PasswordMinLength)
                return false;

            if (!password.Any(c => char.IsDigit(c)))
                return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerStatus"></param>
        /// <returns></returns>
        public static bool CheckPlayerStatus(string playerStatus)
        {
            if (string.IsNullOrWhiteSpace(playerStatus))
                return false;

            if (playerStatus.Length > PlayerStatusMaxLength)
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
    }
}
