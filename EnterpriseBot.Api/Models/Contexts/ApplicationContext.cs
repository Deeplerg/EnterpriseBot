using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Donation;
using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Game.Localization;
using EnterpriseBot.Api.Game.Money;
using EnterpriseBot.Api.Game.Reputation;
using EnterpriseBot.Api.Game.Storages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;

namespace EnterpriseBot.Api.Models.Contexts
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> opt) : base(opt) { }

        #region business
        #region company
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyContract> CompanyContracts { get; set; }
        public DbSet<CompanyContractRequest> CompanyContractRequests { get; set; }
        public DbSet<CompanyJob> CompanyJobs { get; set; }
        public DbSet<CompanyJobApplication> CompanyJobApplications { get; set; }
        public DbSet<CompanyWorker> CompanyWorkers { get; set; }
        public DbSet<ProductionRobot> ProductionRobots { get; set; }
        public DbSet<Truck> Trucks { get; set; }
        public DbSet<TruckGarage> TruckGarages { get; set; }
        #endregion
        #endregion

        #region crafting
        public DbSet<CraftingCategory> CraftingCategories { get; set; }
        public DbSet<CraftingSubCategory> CraftingSubCategories { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        #endregion

        #region donation
        public DbSet<Donation> Donations { get; set; }
        public DbSet<DonationPurchase> DonationPurchases { get; set; }
        #endregion

        #region essences
        public DbSet<Player> Players { get; set; }
        #endregion

        #region localization
        public DbSet<LocalizedString> LocalizedStrings { get; set; }
        public DbSet<StringLocalization> StringLocalizations { get; set; }
        #endregion

        #region money
        public DbSet<Purse> Purses { get; set; }
        #endregion

        #region reputation
        public DbSet<Reputation> Reputations { get; set; }
        public DbSet<Review> Reviews { get; set; }
        #endregion

        #region storages
        public DbSet<CompanyStorage> CompanyStorages { get; set; }
        public DbSet<InventoryStorage> InventoryStorages { get; set; }
        public DbSet<ItemPrice> ItemPrices { get; set; }
        public DbSet<ShowcaseStorage> ShowcaseStorages { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<StorageItem> StorageItems { get; set; }
        public DbSet<TrunkStorage> TrunkStorages { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            int decimalIntegers = 26; //amount of integer numbers on the left -
                                      //999 septillion (999,999,999,999,999,999,999,999,999)
            int decimalScale = 6; //amount of decimal numbers on the right - 6. (0.123456)
            int decimalPrecision = decimalIntegers + decimalScale; //total amount of numbers

            string decimalTypeName = $"decimal({decimalPrecision}, {decimalScale})";

            BindingFlags nonPublicFlags = BindingFlags.NonPublic | BindingFlags.Instance;

            #region business
            #region company
            builder.Entity<Company>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.Owner)
                 .WithMany(m => m.OwnedCompanies)
                 .HasForeignKey($"{nameof(Company.Owner)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(m => m.Purse)
                 .WithOne()
                 .HasForeignKey<Company>($"{nameof(Company.Purse)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(m => m.Reputation)
                 .WithOne()
                 .HasForeignKey<Company>($"{nameof(Company.Reputation)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(m => m.TruckGarage)
                 .WithOne(m => m.Company)
                 .HasForeignKey<TruckGarage>($"{nameof(TruckGarage.Company)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(m => m.Jobs)
                 .WithOne(m => m.Company)
                 .HasForeignKey($"{nameof(CompanyJob.Company)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(m => m.Robots)
                 .WithOne(m => m.Company)
                 .HasForeignKey($"{nameof(ProductionRobot.Company)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(m => m.IncomeContracts)
                 .WithOne(m => m.IncomeCompany)
                 .HasForeignKey($"{nameof(CompanyContract.IncomeCompany)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(m => m.OutcomeContracts)
                 .WithOne(m => m.OutcomeCompany)
                 .HasForeignKey($"{nameof(CompanyContract.OutcomeCompany)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(m => m.SentContractRequests)
                 .WithOne(m => m.RequestingCompany)
                 .HasForeignKey($"{nameof(CompanyContractRequest.RequestingCompany)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(m => m.InboxContractRequests)
                 .WithOne(m => m.RequestedCompany)
                 .HasForeignKey($"{nameof(CompanyContractRequest.RequestedCompany)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(m => m.Storages)
                 .WithOne(m => m.OwningCompany)
                 .HasForeignKey($"{nameof(CompanyStorage.OwningCompany)}Id")
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<CompanyContract>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.OutcomeCompany)
                 .WithMany(m => m.OutcomeContracts)
                 .HasForeignKey($"{nameof(CompanyContract.OutcomeCompany)}Id")
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(m => m.IncomeCompany)
                 .WithMany(m => m.IncomeContracts)
                 .HasForeignKey($"{nameof(CompanyContract.IncomeCompany)}Id")
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(m => m.ContractItem)
                 .WithOne()
                 .HasForeignKey<CompanyContract>($"{nameof(CompanyContract.ContractItem)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                //b.Property(m => m.ContractOverallCost)
                // .HasColumnType(decimalTypeName);

                b.Ignore(m => m.IsCompleted);
            });

            builder.Entity<CompanyContractRequest>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.RequestedCompany)
                 .WithMany(m => m.InboxContractRequests)
                 .HasForeignKey($"{nameof(CompanyContractRequest.RequestedCompany)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(m => m.RequestingCompany)
                 .WithMany(m => m.SentContractRequests)
                 .HasForeignKey($"{nameof(CompanyContractRequest.RequestingCompany)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(m => m.ContractItem)
                 .WithOne()
                 .HasForeignKey<CompanyContractRequest>($"{nameof(CompanyContractRequest.ContractItem)}Id")
                 .OnDelete(DeleteBehavior.Restrict);

                //b.Property(m => m.ContractOverallCost)
                // .HasColumnType(decimalTypeName);
            });

            builder.Entity<CompanyJob>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.Employee)
                 .WithMany(m => m.CompanyJobs)
                 .HasForeignKey($"{nameof(CompanyJob.Employee)}Id")
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(m => m.Company)
                 .WithMany(m => m.Jobs)
                 .HasForeignKey($"{nameof(CompanyJob.Company)}Id")
                 .OnDelete(DeleteBehavior.Cascade); //already configured in Company


                string workerPropertyName = typeof(CompanyJob).GetProperty("Worker", nonPublicFlags).Name;

                b.HasOne(typeof(CompanyWorker), workerPropertyName)
                 .WithOne()
                 .HasForeignKey(typeof(CompanyJob), workerPropertyName + "Id")
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasMany(m => m.Applications)
                 .WithOne(m => m.Job)
                 .HasForeignKey($"{nameof(CompanyJobApplication.Job)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.Ignore(m => m.IsOccupied);

                b.Ignore(m => m.Recipe);

                b.Ignore(m => m.WorkingStorage);

                b.Ignore(m => m.IsWorkingNow);

                b.Ignore(m => m.ItemsAmountMadeThisWeek);

                b.Ignore(m => m.ProduceItemJobId);

                b.Ignore(m => m.StopWorkingJobId);
            });

            builder.Entity<CompanyJobApplication>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.Job)
                 .WithMany(m => m.Applications)
                 .HasForeignKey($"{nameof(CompanyJobApplication.Job)}Id")
                 .OnDelete(DeleteBehavior.Cascade); //already configured in CompanyJob

                b.HasOne(m => m.Applicant)
                 .WithMany(m => m.CompanyJobApplications)
                 .HasForeignKey($"{nameof(CompanyJobApplication.Applicant)}Id")
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<CompanyWorker>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.Company)
                 .WithOne()
                 .HasForeignKey<CompanyWorker>($"{nameof(CompanyWorker.Company)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(m => m.Recipe)
                 .WithOne()
                 .HasForeignKey<CompanyWorker>($"{nameof(CompanyWorker.Recipe)}Id")
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(m => m.WorkingStorage)
                 .WithOne()
                 .HasForeignKey<CompanyWorker>($"{nameof(CompanyWorker.WorkingStorage)}Id")
                 .OnDelete(DeleteBehavior.Restrict);

                //b.Property(m => m.SpeedMultiplier)
                // .HasColumnName(decimalTypeName);

                b.Ignore(m => m.LeadTimeInSeconds);
            });

            builder.Entity<ProductionRobot>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.Company)
                 .WithMany(m => m.Robots)
                 .HasForeignKey($"{nameof(ProductionRobot.Company)}Id")
                 .OnDelete(DeleteBehavior.Cascade); //already configured in Company


                string workerPropertyName = typeof(ProductionRobot).GetProperty("Worker", nonPublicFlags).Name;

                b.HasOne(typeof(CompanyWorker), workerPropertyName)
                 .WithOne()
                 .HasForeignKey(typeof(ProductionRobot), workerPropertyName + "Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.Ignore(m => m.Recipe);

                b.Ignore(m => m.WorkingStorage);

                b.Ignore(m => m.IsWorkingNow);

                b.Ignore(m => m.ItemsAmountMadeThisWeek);

                b.Ignore(m => m.SpeedMultiplier);
            });

            builder.Entity<Truck>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.TruckGarage)
                 .WithMany(m => m.Trucks)
                 .HasForeignKey($"{nameof(Truck.TruckGarage)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(m => m.Trunk)
                 .WithOne(m => m.OwningTruck)
                 .HasForeignKey<TrunkStorage>($"{nameof(TrunkStorage.OwningTruck)}Id")
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<TruckGarage>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.Company)
                 .WithOne(m => m.TruckGarage)
                 .HasForeignKey<TruckGarage>($"{nameof(TruckGarage.Company)}Id")
                 .OnDelete(DeleteBehavior.Cascade); //already configured in Company

                b.HasMany(m => m.Trucks)
                 .WithOne(m => m.TruckGarage)
                 .HasForeignKey($"{nameof(Truck.TruckGarage)}Id")
                 .OnDelete(DeleteBehavior.Cascade); //already configured in Truck
            });
            #endregion
            #endregion

            #region crafting
            builder.Entity<CraftingCategory>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.Name)
                 .WithOne()
                 .HasForeignKey<CraftingCategory>($"{nameof(CraftingCategory.Name)}Id")
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(m => m.Description)
                 .WithOne()
                 .HasForeignKey<CraftingCategory>($"{nameof(CraftingCategory.Description)}Id")
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasMany(m => m.SubCategories)
                 .WithOne()
                 .HasForeignKey($"{nameof(CraftingSubCategory.MainCategory)}Id")
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<CraftingSubCategory>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.Name)
                 .WithOne()
                 .HasForeignKey<CraftingSubCategory>($"{nameof(CraftingSubCategory.Name)}Id")
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(m => m.Description)
                 .WithOne()
                 .HasForeignKey<CraftingSubCategory>($"{nameof(CraftingSubCategory.Description)}Id")
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(m => m.MainCategory)
                 .WithMany(m => m.SubCategories)
                 .HasForeignKey($"{nameof(CraftingSubCategory.MainCategory)}Id")
                 .OnDelete(DeleteBehavior.Cascade); //already configured in CraftingCategory

                b.HasMany(m => m.Items)
                 .WithOne(m => m.Category)
                 .HasForeignKey($"{nameof(Item.Category)}Id")
                 .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Ingredient>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.Item)
                 .WithOne()
                 .HasForeignKey<Ingredient>($"{nameof(Ingredient.Item)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(m => m.Recipe)
                 .WithMany(m => m.Ingredients)
                 .HasForeignKey($"{nameof(Ingredient.Recipe)}Id")
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Item>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.Name)
                 .WithOne()
                 .HasForeignKey<Item>($"{nameof(Item.Name)}Id")
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(m => m.Category)
                 .WithMany(m => m.Items)
                 .HasForeignKey($"{nameof(Item.Category)}Id")
                 .OnDelete(DeleteBehavior.Restrict); //already configured in CraftingSubCategory

                //b.Property(m => m.Space)
                // .HasColumnType(decimalTypeName);
            });

            builder.Entity<Recipe>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.ResultItem)
                 .WithOne()
                 .HasForeignKey<Recipe>($"{nameof(Recipe)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(m => m.Ingredients)
                 .WithOne(m => m.Recipe)
                 .HasForeignKey($"{nameof(Ingredient.Recipe)}Id")
                 .OnDelete(DeleteBehavior.Cascade); //already configured in Ingredient
            });
            #endregion

            #region donation
            builder.Entity<Donation>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasMany(m => m.History)
                 .WithOne(m => m.RelatedDonation)
                 .HasForeignKey($"{nameof(DonationPurchase.RelatedDonation)}Id")
                 .OnDelete(DeleteBehavior.Restrict);

                b.Ignore(m => m.HasDonation);
            });
            #endregion

            #region essences
            builder.Entity<Player>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.About)
                 .WithOne()
                 .HasForeignKey<Player>($"{nameof(Player.About)}Id")
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(m => m.Status)
                 .WithOne()
                 .HasForeignKey<Player>($"{nameof(Player.Status)}Id")
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(m => m.Inventory)
                 .WithOne(m => m.OwningPlayer)
                 .HasForeignKey<InventoryStorage>($"{nameof(InventoryStorage.OwningPlayer)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(m => m.Purse)
                 .WithOne()
                 .HasForeignKey<Player>($"{nameof(Player.Purse)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(m => m.CompanyJobs)
                 .WithOne(m => m.Employee)
                 .HasForeignKey($"{nameof(CompanyJob.Employee)}Id")
                 .OnDelete(DeleteBehavior.Restrict); //already configured in CompanyJob

                b.HasMany(m => m.OwnedCompanies)
                 .WithOne(m => m.Owner)
                 .HasForeignKey($"{nameof(Company.Owner)}Id")
                 .OnDelete(DeleteBehavior.Cascade); //already configured in Company

                b.HasOne(m => m.Donation)
                 .WithOne(m => m.Player)
                 .HasForeignKey<Donation>($"{nameof(Player)}Id")
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(m => m.Reputation)
                 .WithOne()
                 .HasForeignKey<Player>($"{nameof(Player.Reputation)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.Ignore(m => m.HasDonation);

                b.Ignore(m => m.HasJob);
            });
            #endregion

            #region localization
            builder.Entity<LocalizedString>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasMany(m => m.Localizations)
                 .WithOne()
                 .HasForeignKey($"{nameof(LocalizedString)}Id")
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<StringLocalization>(b =>
            {
                b.HasKey(m => m.Id);
            });
            #endregion

            #region money
            builder.Entity<Purse>(b =>
            {
                b.HasKey(m => m.Id);

                b.OwnsMany(m => m.Money)
                 .WithOwner();
            });
            #endregion

            #region reputation
            builder.Entity<Reputation>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasMany(m => m.Reviews)
                 .WithOne()
                 .HasForeignKey($"{nameof(Reputation)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.Ignore(m => m.Rating);
            });
            #endregion

            #region storages
            builder.Entity<CompanyStorage>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.OwningCompany)
                 .WithMany(m => m.Storages)
                 .HasForeignKey($"{nameof(CompanyStorage.OwningCompany)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                string storagePropertyName = typeof(CompanyStorage).GetProperty("Storage", nonPublicFlags).Name;

                b.HasOne(typeof(Storage), storagePropertyName)
                 .WithOne()
                 .HasForeignKey(typeof(CompanyStorage), storagePropertyName + "Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.Ignore(m => m.Capacity);

                b.Ignore(m => m.AvailableSpace);

                b.Ignore(m => m.OccupiedSpace);

                b.Ignore(m => m.Items);
            });

            builder.Entity<InventoryStorage>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.OwningPlayer)
                 .WithOne(m => m.Inventory)
                 .HasForeignKey<InventoryStorage>($"{nameof(InventoryStorage.OwningPlayer)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                string storagePropertyName = typeof(InventoryStorage).GetProperty("Storage", nonPublicFlags).Name;

                b.HasOne(typeof(Storage), storagePropertyName)
                 .WithOne()
                 .HasForeignKey(typeof(InventoryStorage), storagePropertyName + "Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.Ignore(m => m.Capacity);

                b.Ignore(m => m.AvailableSpace);

                b.Ignore(m => m.OccupiedSpace);

                b.Ignore(m => m.Items);
            });

            builder.Entity<ItemPrice>(b =>
            {
                b.HasKey(m => m.Id);

                //b.Property(m => m.Price)
                // .HasColumnType(decimalTypeName);

                b.HasOne(m => m.Item)
                 .WithOne()
                 .HasForeignKey<ItemPrice>($"{nameof(ItemPrice.Item)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(m => m.OwningShowcase)
                 .WithMany(m => m.Prices)
                 .HasForeignKey($"{nameof(ItemPrice.OwningShowcase)}Id")
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ShowcaseStorage>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.OwningCompany)
                 .WithOne()
                 .HasForeignKey<ShowcaseStorage>($"{nameof(ShowcaseStorage.OwningCompany)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(m => m.Prices)
                 .WithOne(m => m.OwningShowcase)
                 .HasForeignKey($"{nameof(ItemPrice.OwningShowcase)}Id")
                 .OnDelete(DeleteBehavior.Cascade); //already configured in ItemPrice

                string storagePropertyName = typeof(ShowcaseStorage).GetProperty("Storage", nonPublicFlags).Name;

                b.HasOne(typeof(Storage), storagePropertyName)
                 .WithOne()
                 .HasForeignKey(typeof(ShowcaseStorage), storagePropertyName + "Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.Ignore(m => m.Capacity);

                b.Ignore(m => m.AvailableSpace);

                b.Ignore(m => m.OccupiedSpace);

                b.Ignore(m => m.Items);
            });

            builder.Entity<Storage>(b =>
            {
                b.HasKey(m => m.Id);

                //b.Property(m => m.Capacity)
                // .HasColumnName(decimalTypeName);

                b.HasMany(m => m.Items)
                 .WithOne()
                 .HasForeignKey($"{nameof(Storage)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.Ignore(m => m.AvailableSpace);

                b.Ignore(m => m.OccupiedSpace);
            });

            builder.Entity<StorageItem>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.Item)
                 .WithOne()
                 .HasForeignKey<StorageItem>($"{nameof(StorageItem.Item)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.Ignore(m => m.Space);
            });

            builder.Entity<TrunkStorage>(b =>
            {
                b.HasKey(m => m.Id);

                b.HasOne(m => m.OwningTruck)
                 .WithOne()
                 .HasForeignKey<TrunkStorage>($"{nameof(TrunkStorage.OwningTruck)}Id")
                 .OnDelete(DeleteBehavior.Cascade);

                string storagePropertyName = typeof(TrunkStorage).GetProperty("Storage", nonPublicFlags).Name;

                b.HasOne(typeof(Storage), storagePropertyName)
                 .WithOne()
                 .HasForeignKey(typeof(TrunkStorage), storagePropertyName + "Id")
                 .OnDelete(DeleteBehavior.Cascade);

                b.Ignore(m => m.Capacity);

                b.Ignore(m => m.AvailableSpace);

                b.Ignore(m => m.OccupiedSpace);

                b.Ignore(m => m.Items);
            });
            #endregion
        }
    }
}
