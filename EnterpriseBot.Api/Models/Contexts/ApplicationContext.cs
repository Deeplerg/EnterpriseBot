using EnterpriseBot.Api.Models.Common.Business;
using EnterpriseBot.Api.Models.Common.Crafting;
using EnterpriseBot.Api.Models.Common.Essences;
using EnterpriseBot.Api.Models.Common.Storages;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseBot.Api.Models.Contexts
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> opt) : base(opt) { }

        #region business
        public DbSet<CandidateForJob> CandidatesForJob { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractInfo> ContractInfos { get; set; }
        public DbSet<ContractRequest> ContractRequests { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Truck> Trucks { get; set; }
        public DbSet<TruckGarage> TruckGarages { get; set; }
        #endregion

        #region crafting
        public DbSet<CraftingCategory> CraftingCategories { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        #endregion

        #region essences
        public DbSet<Bot> Bots { get; set; }
        public DbSet<Player> Players { get; set; }
        #endregion

        #region storages
        public DbSet<IncomeStorage> IncomeStorages { get; set; }
        public DbSet<OutcomeStorage> OutcomeStorages { get; set; }
        public DbSet<PersonalStorage> PersonalStorages { get; set; }
        public DbSet<ShowcaseStorage> ShowcaseStorages { get; set; }
        public DbSet<StorageItem> StorageItems { get; set; }
        public DbSet<StorageItemWithPrice> StorageItemsWithPrice { get; set; }
        public DbSet<TrunkStorage> TrunkStorages { get; set; }
        public DbSet<WorkerStorage> WorkerStorages { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            int decimalIntegers = 18; //amount of integer numbers on the left -
                                      //999 quadrillion
            int decimalScale = 6; //amount of decimal numbers on the right - 6.
            int decimalPrecision = decimalIntegers + decimalScale; //total amount of numbers

            string decimalTypeName = $"decimal({decimalPrecision}, {decimalScale})";

            #region business
            builder.Entity<Company>(b =>
            {
                b.HasOne(m => m.IncomeStorage)
                 .WithOne(m => m.OwningCompany)
                 .HasForeignKey<IncomeStorage>(m => m.OwningCompanyId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(m => m.OutcomeStorage)
                 .WithOne(m => m.OwningCompany)
                 .HasForeignKey<OutcomeStorage>(m => m.OwningCompanyId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(m => m.WorkerStorage)
                 .WithOne(m => m.OwningCompany)
                 .HasForeignKey<WorkerStorage>(m => m.OwningCompanyId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(m => m.TruckGarage)
                 .WithOne(m => m.Company)
                 .HasForeignKey<TruckGarage>(m => m.CompanyId)
                 .OnDelete(DeleteBehavior.Cascade);

                //b.Property(m => m.CompanyUnits)
                // .HasColumnType<decimal>(decimalTypeName);

                b.HasMany(m => m.Candidates)
                .WithOne(m => m.HiringCompany)
                .HasForeignKey(m => m.HiringCompanyId)
                .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(m => m.IncomeContracts)
                 .WithOne(m => m.IncomeCompany)
                 .HasForeignKey(m => m.IncomeShopId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(m => m.OutcomeContracts)
                 .WithOne(m => m.OutcomeCompany)
                 .HasForeignKey(m => m.OutcomeCompanyId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(m => m.ContractRequests)
                 .WithOne(m => m.RequestedCompany)
                 .HasForeignKey(m => m.RequestedCompanyId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(m => m.OutputItems)
                 .WithOne()
                 .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Job>(b =>
            {
                b.HasOne(m => m.Worker)
                 .WithOne(m => m.Job)
                 .HasForeignKey<Player>(m => m.JobId)
                 .OnDelete(DeleteBehavior.ClientSetNull);

                b.HasOne(m => m.Bot)
                 .WithOne(m => m.Job)
                 .HasForeignKey<Bot>(m => m.JobId)
                 .OnDelete(DeleteBehavior.Cascade);

                //b.Property(m => m.Salary)
                // .HasColumnType<decimal>(decimalTypeName);

                //b.Property(m => m.SpeedModifier)
                // .HasColumnType<decimal>(decimalTypeName);
            });

            builder.Entity<Contract>(b =>
            {
                //b.Property(m => m.ContractOverallCost)
                // .HasColumnType<decimal>(decimalTypeName);
            });

            //builder.Entity<ContractInfo>()
            //       .Property(m => m.ContractOverallCost)
            //       .HasColumnType<decimal>(decimalTypeName);

            builder.Entity<Shop>(b =>
            {
                //b.Property(m => m.ShopUnits)
                // .HasColumnType<decimal>(decimalTypeName);

                b.HasOne(m => m.IncomeStorage)
                 .WithOne(m => m.OwningShop)
                 .HasForeignKey<IncomeStorage>(m => m.OwningShopId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(m => m.ShowcaseStorage)
                 .WithOne(m => m.OwningShop)
                 .HasForeignKey<ShowcaseStorage>(m => m.OwningShopId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(m => m.IncomeContracts)
                .WithOne(m => m.IncomeShop)
                .HasForeignKey(m => m.IncomeShopId)
                .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(m => m.ContractRequests)
                 .WithOne(m => m.RequestedShop)
                 .HasForeignKey(m => m.RequestedShopId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Truck>()
                   .HasOne(m => m.TrunkStorage)
                   .WithOne(m => m.Truck)
                   .HasForeignKey<TrunkStorage>(m => m.TruckId);

            builder.Entity<TruckGarage>()
                   .HasMany(m => m.Trucks)
                   .WithOne(m => m.TruckGarage)
                   .HasForeignKey(m => m.TruckGarageId);
            #endregion

            #region essences
            builder.Entity<Player>(b =>
            {
                b.HasOne(m => m.PersonalStorage)
                 .WithOne(m => m.Player)
                 .HasForeignKey<PersonalStorage>(m => m.PlayerId)
                 .OnDelete(DeleteBehavior.Cascade);

                //b.Property(m => m.Units)
                // .HasColumnType<decimal>(decimalTypeName);

                b.HasMany(m => m.OwnedCompanies)
                 .WithOne(m => m.GeneralManager)
                 .HasForeignKey(m => m.GeneralManagerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region crafting
            builder.Entity<CraftingCategory>()
                   .HasKey(m => m.Name);
            #endregion

            #region storages
            builder.Entity<StorageItemWithPrice>(b =>
            {
                b.HasOne(m => m.StorageItem)
                 .WithOne()
                 .HasForeignKey<StorageItem>("StorageItemWithPriceId");

                //b.Property(m => m.Price)
                // .HasColumnType<decimal>(decimalTypeName);
            });

            builder.Entity<ShowcaseStorage>()
                   .HasMany<StorageItemWithPrice>(m => m.Items)
                   .WithOne(m => m.ShowcaseStorage)
                   .HasForeignKey(m => m.ShowcaseStorageId);

            builder.Entity<StorageItem>()
                   .HasOne(m => m.Item); //with no id on the Item side, but with id in StorageItem
            #endregion
        }
    }
}
