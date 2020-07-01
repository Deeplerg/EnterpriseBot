using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Donation;
using EnterpriseBot.Api.Game.Localization;
using EnterpriseBot.Api.Game.Money;
using EnterpriseBot.Api.Game.Reputation;
using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Donation;
using EnterpriseBot.Api.Models.ModelCreationParams.Essences;
using EnterpriseBot.Api.Models.ModelCreationParams.Money;
using EnterpriseBot.Api.Models.ModelCreationParams.Storages;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static EnterpriseBot.Api.Utils.UserInputUtils;

namespace EnterpriseBot.Api.Game.Essences
{
    public class Player
    {
        protected Player() { }

        #region model
        public long Id { get; protected set; }

        public string Name { get; protected set; }
        public LocalizedString Status { get; protected set; }
        public LocalizedString About { get; protected set; }

        public virtual InventoryStorage Inventory { get; protected set; }

        public virtual Purse Purse { get; protected set; }

        public virtual IReadOnlyCollection<CompanyJob> CompanyJobs 
        {
            get => new ReadOnlyCollection<CompanyJob>(companyJobs);
            protected set => companyJobs = value.ToList();
        }
        public virtual IReadOnlyCollection<Company> OwnedCompanies 
        {
            get => new ReadOnlyCollection<Company>(ownedCompanies);
            protected set => ownedCompanies = value.ToList();
        }

        private List<CompanyJob> companyJobs = new List<CompanyJob>();
        private List<Company> ownedCompanies = new List<Company>();

        public virtual Donation.Donation Donation { get; protected set; }

        public bool VkConnected { get; protected set; }
        public long? VkId { get; protected set; }
        
        public DateTime RegistrationDate { get; protected set; }

        public Reputation.Reputation Reputation { get; protected set; }


        [JsonIgnore]
        public string PasswordHash { get; protected set; }
        [JsonIgnore]
        public string PasswordSaltBase64 { get; protected set; }


        public bool HasDonation { get => Donation.HasDonation; }
        public bool HasJob { get => CompanyJobs != null && CompanyJobs.Any(); }

        public virtual IReadOnlyCollection<CompanyJobApplication> CompanyJobApplications
        {
            get => new ReadOnlyCollection<CompanyJobApplication>(companyJobApplications);
            protected set => companyJobApplications = value.ToList();
        }
        private List<CompanyJobApplication> companyJobApplications = new List<CompanyJobApplication>();

        #endregion

        #region actions
        public static GameResult<Player> Create(PlayerCreationParams pars,
            GameplaySettings gameplaySettings, UserInputRequirements req)
        {
            if (!CheckName(pars.Name))
            {
                return Errors.IncorrectNameInput(req);
            }

            if(!CheckPasswordReliability(pars.RawPassword))
            {
                return Errors.IncorrectPasswordInput(req);
            }


            Player player = null;


            byte[] passwordSalt = Hash.CreateSalt();
            string passwordHash = Hash.CreateHash(pars.RawPassword, passwordSalt);

            var donationCreationResult = Game.Donation.Donation.Create(new DonationCreationParams
            {
                Privilege = default(Privilege),
                Player = player
            });
            if (donationCreationResult.LocalizedError != null) return donationCreationResult.LocalizedError;

            var purseCreationResult = Purse.Create(new PurseCreationParams
            {
                UnitsAmount = gameplaySettings.Player.DefaultUnits,
                BusinessCoinsAmount = gameplaySettings.Player.DefaultBusinessCoins
            });
            if (purseCreationResult.LocalizedError != null) return purseCreationResult.LocalizedError;

            var reputationCreationResult = Game.Reputation.Reputation.Create(new ReputationCreationParams
            {
            });
            if (reputationCreationResult.LocalizedError != null) return reputationCreationResult.LocalizedError;


            player = new Player
            {
                Name = pars.Name,

                Donation = donationCreationResult,
                Purse = purseCreationResult,
                Reputation = reputationCreationResult,

                PasswordHash = passwordHash,
                PasswordSaltBase64 = Convert.ToBase64String(passwordSalt),

                VkConnected = false,
                VkId = null,

                RegistrationDate = DateTime.Now
            };

            var storageCreationResult = InventoryStorage.Create(new InventoryStorageCreationParams
            {
                Capacity = gameplaySettings.Storage.Inventory.DefaultCapacity,
                OwningPlayer = player
            });
            if (storageCreationResult.LocalizedError != null) return storageCreationResult.LocalizedError;

            player.Inventory = storageCreationResult;

            return player;
        }

        public bool HasPermission(CompanyJobPermissions permission, Company company)
        {
            var job = CompanyJobs.SingleOrDefault(j => j.Company == company);
            if (job is null) return company.Owner == this;

            return job.Permissions.HasFlag(permission);
        }

        public GameResult<string> SetName(string newName,
            UserInputRequirements req)
        {
            if(!CheckName(newName))
            {
                return Errors.IncorrectNameInput(req);
            }

            Name = newName;

            return Name;
        }

        public GameResult<StringLocalization> EditAbout(string newAbout, LocalizationLanguage language,
            UserInputRequirements req)
        {
            if(!CheckDescription(newAbout))
            {
                return Errors.IncorrectDescriptionInput(req);
            }

            var editResult = About.Edit(newAbout, language);
            if (editResult.LocalizedError != null) return editResult.LocalizedError;

            return editResult;
        }

        public GameResult<StringLocalization> EditStatus(string newStatus, LocalizationLanguage language,
            UserInputRequirements req)
        {
            if (!CheckPlayerStatus(newStatus))
            {
                return Errors.IncorrectPlayerStatusInput(req);
            }

            var editResult = Status.Edit(newStatus, language);
            if (editResult.LocalizedError != null) return editResult.LocalizedError;

            return editResult;
        }

        public EmptyGameResult ChangePassword(string newPassword,
            UserInputRequirements req)
        {
            if(!CheckPasswordReliability(newPassword))
            {
                return Errors.IncorrectPasswordInput(req);
            }

            byte[] salt = Hash.CreateSalt();
            string hash = Hash.CreateHash(newPassword, salt);

            string base64Salt = Convert.ToBase64String(salt);

            PasswordHash = hash;
            PasswordSaltBase64 = base64Salt;

            return new EmptyGameResult();
        }

        public bool VerifyPassword(string password)
        {
            byte[] salt = Convert.FromBase64String(PasswordSaltBase64);

            return Hash.Verify(password, salt, PasswordHash);
        }

        public GameResult<long> LinkVk(long vkId)
        {
            VkId = vkId;
            VkConnected = true;

            return VkId;
        }

        public EmptyGameResult UnlinkVk()
        {
            VkId = null;
            VkConnected = false;

            return new EmptyGameResult();
        }
        #endregion
    }
}
