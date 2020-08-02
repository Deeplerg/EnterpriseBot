using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace EnterpriseBot.Api.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocalizedStrings",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizedStrings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Purses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reputations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reputations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Storages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Capacity = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CraftingCategories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameId = table.Column<long>(nullable: true),
                    DescriptionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftingCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CraftingCategories_LocalizedStrings_DescriptionId",
                        column: x => x.DescriptionId,
                        principalTable: "LocalizedStrings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CraftingCategories_LocalizedStrings_NameId",
                        column: x => x.NameId,
                        principalTable: "LocalizedStrings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StringLocalizations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Language = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    LocalizedStringId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StringLocalizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StringLocalizations_LocalizedStrings_LocalizedStringId",
                        column: x => x.LocalizedStringId,
                        principalTable: "LocalizedStrings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Money",
                columns: table => new
                {
                    PurseId = table.Column<long>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<decimal>(nullable: false),
                    Currency = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Money", x => new { x.PurseId, x.Id });
                    table.ForeignKey(
                        name: "FK_Money_Purses_PurseId",
                        column: x => x.PurseId,
                        principalTable: "Purses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    StatusId = table.Column<long>(nullable: true),
                    AboutId = table.Column<long>(nullable: true),
                    PurseId = table.Column<long>(nullable: true),
                    VkConnected = table.Column<bool>(nullable: false),
                    VkId = table.Column<long>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(nullable: false),
                    ReputationId = table.Column<long>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PasswordSaltBase64 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_LocalizedStrings_AboutId",
                        column: x => x.AboutId,
                        principalTable: "LocalizedStrings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Players_Purses_PurseId",
                        column: x => x.PurseId,
                        principalTable: "Purses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Players_Reputations_ReputationId",
                        column: x => x.ReputationId,
                        principalTable: "Reputations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Players_LocalizedStrings_StatusId",
                        column: x => x.StatusId,
                        principalTable: "LocalizedStrings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CraftingSubCategories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameId = table.Column<long>(nullable: true),
                    DescriptionId = table.Column<long>(nullable: true),
                    MainCategoryId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftingSubCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CraftingSubCategories_LocalizedStrings_DescriptionId",
                        column: x => x.DescriptionId,
                        principalTable: "LocalizedStrings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CraftingSubCategories_CraftingCategories_MainCategoryId",
                        column: x => x.MainCategoryId,
                        principalTable: "CraftingCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CraftingSubCategories_LocalizedStrings_NameId",
                        column: x => x.NameId,
                        principalTable: "LocalizedStrings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    DescriptionId = table.Column<long>(nullable: true),
                    OwnerId = table.Column<long>(nullable: true),
                    PurseId = table.Column<long>(nullable: true),
                    ReputationId = table.Column<long>(nullable: true),
                    ContractsCompleted = table.Column<long>(nullable: false),
                    Extensions = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_LocalizedStrings_DescriptionId",
                        column: x => x.DescriptionId,
                        principalTable: "LocalizedStrings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Companies_Players_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Companies_Purses_PurseId",
                        column: x => x.PurseId,
                        principalTable: "Purses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Companies_Reputations_ReputationId",
                        column: x => x.ReputationId,
                        principalTable: "Reputations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Donations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Privilege = table.Column<int>(nullable: false),
                    PlayerId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Donations_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryStorages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwningPlayerId = table.Column<long>(nullable: true),
                    StorageId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryStorages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryStorages_Players_OwningPlayerId",
                        column: x => x.OwningPlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryStorages_Storages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameId = table.Column<long>(nullable: true),
                    CategoryId = table.Column<long>(nullable: true),
                    Space = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_CraftingSubCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CraftingSubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_LocalizedStrings_NameId",
                        column: x => x.NameId,
                        principalTable: "LocalizedStrings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyStorages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwningCompanyId = table.Column<long>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    StorageId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyStorages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyStorages_Companies_OwningCompanyId",
                        column: x => x.OwningCompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyStorages_Storages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(nullable: true),
                    Reviewer = table.Column<int>(nullable: false),
                    CompanyReviewerId = table.Column<long>(nullable: true),
                    PlayerReviewerId = table.Column<long>(nullable: true),
                    Rating = table.Column<short>(nullable: false),
                    ReputationId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Companies_CompanyReviewerId",
                        column: x => x.CompanyReviewerId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Players_PlayerReviewerId",
                        column: x => x.PlayerReviewerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Reputations_ReputationId",
                        column: x => x.ReputationId,
                        principalTable: "Reputations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShowcaseStorages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwningCompanyId = table.Column<long>(nullable: true),
                    StorageId = table.Column<long>(nullable: false),
                    CompanyId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowcaseStorages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShowcaseStorages_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShowcaseStorages_Companies_OwningCompanyId",
                        column: x => x.OwningCompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowcaseStorages_Storages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TruckGarages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<long>(nullable: true),
                    Capacity = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TruckGarages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TruckGarages_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonationPurchases",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Time = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    RelatedDonationId = table.Column<long>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    Currency = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonationPurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonationPurchases_Donations_RelatedDonationId",
                        column: x => x.RelatedDonationId,
                        principalTable: "Donations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyContractRequests",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    RequestedCompanyId = table.Column<long>(nullable: true),
                    RequestingCompanyId = table.Column<long>(nullable: true),
                    RequestingCompanyRelationSide = table.Column<int>(nullable: false),
                    ContractItemId = table.Column<long>(nullable: true),
                    ContractItemQuantity = table.Column<int>(nullable: false),
                    ContractOverallCost = table.Column<decimal>(nullable: false),
                    TerminationTermInDays = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyContractRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyContractRequests_Items_ContractItemId",
                        column: x => x.ContractItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyContractRequests_Companies_RequestedCompanyId",
                        column: x => x.RequestedCompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyContractRequests_Companies_RequestingCompanyId",
                        column: x => x.RequestingCompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyContracts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    OutcomeCompanyId = table.Column<long>(nullable: true),
                    IncomeCompanyId = table.Column<long>(nullable: true),
                    Issuer = table.Column<int>(nullable: false),
                    ConclusionDate = table.Column<DateTimeOffset>(nullable: false),
                    TerminationTermInDays = table.Column<short>(nullable: false),
                    ContractItemId = table.Column<long>(nullable: true),
                    DeliveredAmount = table.Column<int>(nullable: false),
                    ContractItemQuantity = table.Column<int>(nullable: false),
                    ContractOverallCost = table.Column<decimal>(nullable: false),
                    CompletionCheckerBackgroundJobId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyContracts_Items_ContractItemId",
                        column: x => x.ContractItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyContracts_Companies_IncomeCompanyId",
                        column: x => x.IncomeCompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyContracts_Companies_OutcomeCompanyId",
                        column: x => x.OutcomeCompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecipeId = table.Column<long>(nullable: true),
                    ResultItemQuantity = table.Column<int>(nullable: false),
                    LeadTimeInSeconds = table.Column<int>(nullable: false),
                    CanBeDoneBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipes_Items_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StorageItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<long>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    StorageId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorageItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StorageItems_Storages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemPrices",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Price = table.Column<decimal>(nullable: false),
                    ItemId = table.Column<long>(nullable: true),
                    OwningShowcaseId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemPrices_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemPrices_ShowcaseStorages_OwningShowcaseId",
                        column: x => x.OwningShowcaseId,
                        principalTable: "ShowcaseStorages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyWorkers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<long>(nullable: true),
                    RecipeId = table.Column<long>(nullable: true),
                    WorkingStorageId = table.Column<long>(nullable: true),
                    IsWorkingNow = table.Column<bool>(nullable: false),
                    SpeedMultiplier = table.Column<decimal>(nullable: false),
                    ProduceItemJobId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyWorkers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyWorkers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyWorkers_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyWorkers_CompanyStorages_WorkingStorageId",
                        column: x => x.WorkingStorageId,
                        principalTable: "CompanyStorages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<long>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    RecipeId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingredients_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ingredients_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyJobs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameId = table.Column<long>(nullable: true),
                    DescriptionId = table.Column<long>(nullable: true),
                    EmployeeId = table.Column<long>(nullable: true),
                    CompanyId = table.Column<long>(nullable: true),
                    Permissions = table.Column<decimal>(nullable: false),
                    Salary = table.Column<decimal>(nullable: false),
                    WorkerId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyJobs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyJobs_LocalizedStrings_DescriptionId",
                        column: x => x.DescriptionId,
                        principalTable: "LocalizedStrings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyJobs_Players_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyJobs_LocalizedStrings_NameId",
                        column: x => x.NameId,
                        principalTable: "LocalizedStrings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyJobs_CompanyWorkers_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "CompanyWorkers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductionRobots",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    CompanyId = table.Column<long>(nullable: true),
                    ProduceItemJobId = table.Column<string>(nullable: true),
                    WorkerId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionRobots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionRobots_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductionRobots_CompanyWorkers_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "CompanyWorkers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyJobApplications",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<long>(nullable: true),
                    ApplicantId = table.Column<long>(nullable: true),
                    Resume = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyJobApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyJobApplications_Players_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyJobApplications_CompanyJobs_JobId",
                        column: x => x.JobId,
                        principalTable: "CompanyJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrunkStorages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwningTruckId = table.Column<long>(nullable: true),
                    StorageId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrunkStorages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrunkStorages_Storages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trucks",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TruckGarageId = table.Column<long>(nullable: true),
                    TrunkId = table.Column<long>(nullable: true),
                    DeliveringSpeedInSeconds = table.Column<long>(nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    UnloadTruckJobId = table.Column<string>(nullable: true),
                    ReturnTruckJobId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trucks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trucks_TruckGarages_TruckGarageId",
                        column: x => x.TruckGarageId,
                        principalTable: "TruckGarages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trucks_TrunkStorages_TrunkId",
                        column: x => x.TrunkId,
                        principalTable: "TrunkStorages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_DescriptionId",
                table: "Companies",
                column: "DescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_OwnerId",
                table: "Companies",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_PurseId",
                table: "Companies",
                column: "PurseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_ReputationId",
                table: "Companies",
                column: "ReputationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyContractRequests_ContractItemId",
                table: "CompanyContractRequests",
                column: "ContractItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyContractRequests_RequestedCompanyId",
                table: "CompanyContractRequests",
                column: "RequestedCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyContractRequests_RequestingCompanyId",
                table: "CompanyContractRequests",
                column: "RequestingCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyContracts_ContractItemId",
                table: "CompanyContracts",
                column: "ContractItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyContracts_IncomeCompanyId",
                table: "CompanyContracts",
                column: "IncomeCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyContracts_OutcomeCompanyId",
                table: "CompanyContracts",
                column: "OutcomeCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyJobApplications_ApplicantId",
                table: "CompanyJobApplications",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyJobApplications_JobId",
                table: "CompanyJobApplications",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyJobs_CompanyId",
                table: "CompanyJobs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyJobs_DescriptionId",
                table: "CompanyJobs",
                column: "DescriptionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyJobs_EmployeeId",
                table: "CompanyJobs",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyJobs_NameId",
                table: "CompanyJobs",
                column: "NameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyJobs_WorkerId",
                table: "CompanyJobs",
                column: "WorkerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyStorages_OwningCompanyId",
                table: "CompanyStorages",
                column: "OwningCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyStorages_StorageId",
                table: "CompanyStorages",
                column: "StorageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyWorkers_CompanyId",
                table: "CompanyWorkers",
                column: "CompanyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyWorkers_RecipeId",
                table: "CompanyWorkers",
                column: "RecipeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyWorkers_WorkingStorageId",
                table: "CompanyWorkers",
                column: "WorkingStorageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CraftingCategories_DescriptionId",
                table: "CraftingCategories",
                column: "DescriptionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CraftingCategories_NameId",
                table: "CraftingCategories",
                column: "NameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CraftingSubCategories_DescriptionId",
                table: "CraftingSubCategories",
                column: "DescriptionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CraftingSubCategories_MainCategoryId",
                table: "CraftingSubCategories",
                column: "MainCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingSubCategories_NameId",
                table: "CraftingSubCategories",
                column: "NameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DonationPurchases_RelatedDonationId",
                table: "DonationPurchases",
                column: "RelatedDonationId");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_PlayerId",
                table: "Donations",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_ItemId",
                table: "Ingredients",
                column: "ItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_RecipeId",
                table: "Ingredients",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStorages_OwningPlayerId",
                table: "InventoryStorages",
                column: "OwningPlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStorages_StorageId",
                table: "InventoryStorages",
                column: "StorageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemPrices_ItemId",
                table: "ItemPrices",
                column: "ItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemPrices_OwningShowcaseId",
                table: "ItemPrices",
                column: "OwningShowcaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryId",
                table: "Items",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_NameId",
                table: "Items",
                column: "NameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_AboutId",
                table: "Players",
                column: "AboutId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_PurseId",
                table: "Players",
                column: "PurseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_ReputationId",
                table: "Players",
                column: "ReputationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_StatusId",
                table: "Players",
                column: "StatusId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionRobots_CompanyId",
                table: "ProductionRobots",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionRobots_WorkerId",
                table: "ProductionRobots",
                column: "WorkerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_RecipeId",
                table: "Recipes",
                column: "RecipeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CompanyReviewerId",
                table: "Reviews",
                column: "CompanyReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PlayerReviewerId",
                table: "Reviews",
                column: "PlayerReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReputationId",
                table: "Reviews",
                column: "ReputationId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowcaseStorages_CompanyId",
                table: "ShowcaseStorages",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowcaseStorages_OwningCompanyId",
                table: "ShowcaseStorages",
                column: "OwningCompanyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShowcaseStorages_StorageId",
                table: "ShowcaseStorages",
                column: "StorageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StorageItems_ItemId",
                table: "StorageItems",
                column: "ItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StorageItems_StorageId",
                table: "StorageItems",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_StringLocalizations_LocalizedStringId",
                table: "StringLocalizations",
                column: "LocalizedStringId");

            migrationBuilder.CreateIndex(
                name: "IX_TruckGarages_CompanyId",
                table: "TruckGarages",
                column: "CompanyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_TruckGarageId",
                table: "Trucks",
                column: "TruckGarageId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_TrunkId",
                table: "Trucks",
                column: "TrunkId");

            migrationBuilder.CreateIndex(
                name: "IX_TrunkStorages_OwningTruckId",
                table: "TrunkStorages",
                column: "OwningTruckId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrunkStorages_StorageId",
                table: "TrunkStorages",
                column: "StorageId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TrunkStorages_Trucks_OwningTruckId",
                table: "TrunkStorages",
                column: "OwningTruckId",
                principalTable: "Trucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_LocalizedStrings_DescriptionId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_LocalizedStrings_AboutId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_LocalizedStrings_StatusId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Players_OwnerId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Purses_PurseId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Reputations_ReputationId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_TruckGarages_Companies_CompanyId",
                table: "TruckGarages");

            migrationBuilder.DropForeignKey(
                name: "FK_TrunkStorages_Storages_StorageId",
                table: "TrunkStorages");

            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_TruckGarages_TruckGarageId",
                table: "Trucks");

            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_TrunkStorages_TrunkId",
                table: "Trucks");

            migrationBuilder.DropTable(
                name: "CompanyContractRequests");

            migrationBuilder.DropTable(
                name: "CompanyContracts");

            migrationBuilder.DropTable(
                name: "CompanyJobApplications");

            migrationBuilder.DropTable(
                name: "DonationPurchases");

            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "InventoryStorages");

            migrationBuilder.DropTable(
                name: "ItemPrices");

            migrationBuilder.DropTable(
                name: "Money");

            migrationBuilder.DropTable(
                name: "ProductionRobots");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "StorageItems");

            migrationBuilder.DropTable(
                name: "StringLocalizations");

            migrationBuilder.DropTable(
                name: "CompanyJobs");

            migrationBuilder.DropTable(
                name: "Donations");

            migrationBuilder.DropTable(
                name: "ShowcaseStorages");

            migrationBuilder.DropTable(
                name: "CompanyWorkers");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "CompanyStorages");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "CraftingSubCategories");

            migrationBuilder.DropTable(
                name: "CraftingCategories");

            migrationBuilder.DropTable(
                name: "LocalizedStrings");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Purses");

            migrationBuilder.DropTable(
                name: "Reputations");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Storages");

            migrationBuilder.DropTable(
                name: "TruckGarages");

            migrationBuilder.DropTable(
                name: "TrunkStorages");

            migrationBuilder.DropTable(
                name: "Trucks");
        }
    }
}
