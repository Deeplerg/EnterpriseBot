using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

namespace EnterpriseBot.Api.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CraftingCategories",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftingCategories", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "CandidatesForJob",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<long>(nullable: true),
                    PotentialEmployeeId = table.Column<long>(nullable: true),
                    HiringCompanyId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidatesForJob", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractInfos",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ContractIncomeBusinessType = table.Column<int>(nullable: false),
                    OutcomeCompanyId = table.Column<long>(nullable: false),
                    IncomeCompanyId = table.Column<long>(nullable: true),
                    IncomeShopId = table.Column<long>(nullable: true),
                    ContractItemId = table.Column<long>(nullable: false),
                    TerminationTermInWeeks = table.Column<short>(nullable: false),
                    ContractItemQuantity = table.Column<long>(nullable: false),
                    ContractOverallCost = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractRequests",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractInfoId = table.Column<long>(nullable: false),
                    RequestedCompanyId = table.Column<long>(nullable: true),
                    RequestedShopId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractRequests_ContractInfos_ContractInfoId",
                        column: x => x.ContractInfoId,
                        principalTable: "ContractInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ContractIncomeBusinessType = table.Column<int>(nullable: false),
                    OutcomeCompanyId = table.Column<long>(nullable: false),
                    IncomeCompanyId = table.Column<long>(nullable: true),
                    IncomeShopId = table.Column<long>(nullable: true),
                    ConclusionDate = table.Column<DateTimeOffset>(nullable: false),
                    TerminationTermInWeeks = table.Column<short>(nullable: false),
                    ContractItemId = table.Column<long>(nullable: false),
                    DeliveredAmount = table.Column<long>(nullable: false),
                    ContractItemQuantity = table.Column<long>(nullable: false),
                    ContractOverallCost = table.Column<decimal>(nullable: false),
                    CompletionCheckerBackgroundJobId = table.Column<string>(nullable: true),
                    BreakerBackgroundJobId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncomeStorages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Capacity = table.Column<int>(nullable: false),
                    OwningCompanyId = table.Column<long>(nullable: true),
                    OwningShopId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeStorages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    CategoryName = table.Column<string>(nullable: true),
                    Space = table.Column<int>(nullable: false),
                    CompanyId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_CraftingCategories_CategoryName",
                        column: x => x.CategoryName,
                        principalTable: "CraftingCategories",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResultItemId = table.Column<long>(nullable: false),
                    LeadTimeInSeconds = table.Column<int>(nullable: false),
                    ResultItemQuantity = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipes_Items_ResultItemId",
                        column: x => x.ResultItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<long>(nullable: false),
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsOccupied = table.Column<bool>(nullable: false),
                    Salary = table.Column<decimal>(nullable: false),
                    WorkerId = table.Column<long>(nullable: true),
                    BotId = table.Column<long>(nullable: true),
                    CompanyId = table.Column<long>(nullable: false),
                    IsBot = table.Column<bool>(nullable: true),
                    RecipeId = table.Column<long>(nullable: false),
                    SpeedModifier = table.Column<decimal>(nullable: false),
                    IsWorkingNow = table.Column<bool>(nullable: true),
                    ItemsAmountMadeThisWeek = table.Column<int>(nullable: false),
                    SalaryPayerJobId = table.Column<string>(nullable: true),
                    ProduceItemJobId = table.Column<string>(nullable: true),
                    StopWorkingJobId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bots",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    JobId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bots_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
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
                    PasswordHash = table.Column<string>(nullable: true),
                    PasswordSaltBase64 = table.Column<string>(nullable: true),
                    VkConnected = table.Column<bool>(nullable: false),
                    VkId = table.Column<long>(nullable: true),
                    Units = table.Column<decimal>(nullable: false),
                    HasJob = table.Column<bool>(nullable: false),
                    JobId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
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
                    Description = table.Column<string>(nullable: true),
                    GeneralManagerId = table.Column<long>(nullable: false),
                    CompanyUnits = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_Players_GeneralManagerId",
                        column: x => x.GeneralManagerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalStorages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Capacity = table.Column<int>(nullable: false),
                    PlayerId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalStorages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalStorages_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shops",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ShopUnits = table.Column<decimal>(nullable: false),
                    GeneralManagerId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shops_Players_GeneralManagerId",
                        column: x => x.GeneralManagerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutcomeStorages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Capacity = table.Column<int>(nullable: false),
                    OwningCompanyId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutcomeStorages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutcomeStorages_Companies_OwningCompanyId",
                        column: x => x.OwningCompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TruckGarages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<long>(nullable: false),
                    Capacity = table.Column<byte>(nullable: false)
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
                name: "WorkerStorages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Capacity = table.Column<int>(nullable: false),
                    OwningCompanyId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerStorages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerStorages_Companies_OwningCompanyId",
                        column: x => x.OwningCompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShowcaseStorages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Capacity = table.Column<int>(nullable: false),
                    OwningShopId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowcaseStorages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShowcaseStorages_Shops_OwningShopId",
                        column: x => x.OwningShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trucks",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TruckGarageId = table.Column<long>(nullable: false),
                    TrunkStorageId = table.Column<long>(nullable: false),
                    DeliveringSpeedInSeconds = table.Column<int>(nullable: false),
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
                });

            migrationBuilder.CreateTable(
                name: "StorageItemsWithPrice",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShowcaseStorageId = table.Column<long>(nullable: false),
                    Price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageItemsWithPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorageItemsWithPrice_ShowcaseStorages_ShowcaseStorageId",
                        column: x => x.ShowcaseStorageId,
                        principalTable: "ShowcaseStorages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrunkStorages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Capacity = table.Column<int>(nullable: false),
                    TruckId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrunkStorages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrunkStorages_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StorageItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<long>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    IncomeStorageId = table.Column<long>(nullable: true),
                    OutcomeStorageId = table.Column<long>(nullable: true),
                    PersonalStorageId = table.Column<long>(nullable: true),
                    StorageItemWithPriceId = table.Column<long>(nullable: true),
                    TrunkStorageId = table.Column<long>(nullable: true),
                    WorkerStorageId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorageItems_IncomeStorages_IncomeStorageId",
                        column: x => x.IncomeStorageId,
                        principalTable: "IncomeStorages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StorageItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StorageItems_OutcomeStorages_OutcomeStorageId",
                        column: x => x.OutcomeStorageId,
                        principalTable: "OutcomeStorages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StorageItems_PersonalStorages_PersonalStorageId",
                        column: x => x.PersonalStorageId,
                        principalTable: "PersonalStorages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StorageItems_StorageItemsWithPrice_StorageItemWithPriceId",
                        column: x => x.StorageItemWithPriceId,
                        principalTable: "StorageItemsWithPrice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StorageItems_TrunkStorages_TrunkStorageId",
                        column: x => x.TrunkStorageId,
                        principalTable: "TrunkStorages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StorageItems_WorkerStorages_WorkerStorageId",
                        column: x => x.WorkerStorageId,
                        principalTable: "WorkerStorages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bots_JobId",
                table: "Bots",
                column: "JobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidatesForJob_HiringCompanyId",
                table: "CandidatesForJob",
                column: "HiringCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatesForJob_JobId",
                table: "CandidatesForJob",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatesForJob_PotentialEmployeeId",
                table: "CandidatesForJob",
                column: "PotentialEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_GeneralManagerId",
                table: "Companies",
                column: "GeneralManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractInfos_ContractItemId",
                table: "ContractInfos",
                column: "ContractItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractInfos_IncomeCompanyId",
                table: "ContractInfos",
                column: "IncomeCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractInfos_IncomeShopId",
                table: "ContractInfos",
                column: "IncomeShopId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractInfos_OutcomeCompanyId",
                table: "ContractInfos",
                column: "OutcomeCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractRequests_ContractInfoId",
                table: "ContractRequests",
                column: "ContractInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractRequests_RequestedCompanyId",
                table: "ContractRequests",
                column: "RequestedCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractRequests_RequestedShopId",
                table: "ContractRequests",
                column: "RequestedShopId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractItemId",
                table: "Contracts",
                column: "ContractItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_IncomeShopId",
                table: "Contracts",
                column: "IncomeShopId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_OutcomeCompanyId",
                table: "Contracts",
                column: "OutcomeCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeStorages_OwningCompanyId",
                table: "IncomeStorages",
                column: "OwningCompanyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncomeStorages_OwningShopId",
                table: "IncomeStorages",
                column: "OwningShopId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_ItemId",
                table: "Ingredients",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_RecipeId",
                table: "Ingredients",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryName",
                table: "Items",
                column: "CategoryName");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CompanyId",
                table: "Items",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CompanyId",
                table: "Jobs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_RecipeId",
                table: "Jobs",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeStorages_OwningCompanyId",
                table: "OutcomeStorages",
                column: "OwningCompanyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonalStorages_PlayerId",
                table: "PersonalStorages",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_JobId",
                table: "Players",
                column: "JobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_ResultItemId",
                table: "Recipes",
                column: "ResultItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Shops_GeneralManagerId",
                table: "Shops",
                column: "GeneralManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowcaseStorages_OwningShopId",
                table: "ShowcaseStorages",
                column: "OwningShopId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StorageItems_IncomeStorageId",
                table: "StorageItems",
                column: "IncomeStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageItems_ItemId",
                table: "StorageItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageItems_OutcomeStorageId",
                table: "StorageItems",
                column: "OutcomeStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageItems_PersonalStorageId",
                table: "StorageItems",
                column: "PersonalStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageItems_StorageItemWithPriceId",
                table: "StorageItems",
                column: "StorageItemWithPriceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StorageItems_TrunkStorageId",
                table: "StorageItems",
                column: "TrunkStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageItems_WorkerStorageId",
                table: "StorageItems",
                column: "WorkerStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageItemsWithPrice_ShowcaseStorageId",
                table: "StorageItemsWithPrice",
                column: "ShowcaseStorageId");

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
                name: "IX_TrunkStorages_TruckId",
                table: "TrunkStorages",
                column: "TruckId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkerStorages_OwningCompanyId",
                table: "WorkerStorages",
                column: "OwningCompanyId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidatesForJob_Jobs_JobId",
                table: "CandidatesForJob",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidatesForJob_Companies_HiringCompanyId",
                table: "CandidatesForJob",
                column: "HiringCompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidatesForJob_Players_PotentialEmployeeId",
                table: "CandidatesForJob",
                column: "PotentialEmployeeId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractInfos_Companies_IncomeCompanyId",
                table: "ContractInfos",
                column: "IncomeCompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractInfos_Companies_OutcomeCompanyId",
                table: "ContractInfos",
                column: "OutcomeCompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractInfos_Items_ContractItemId",
                table: "ContractInfos",
                column: "ContractItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractInfos_Shops_IncomeShopId",
                table: "ContractInfos",
                column: "IncomeShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractRequests_Companies_RequestedCompanyId",
                table: "ContractRequests",
                column: "RequestedCompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractRequests_Shops_RequestedShopId",
                table: "ContractRequests",
                column: "RequestedShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Companies_IncomeShopId",
                table: "Contracts",
                column: "IncomeShopId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Companies_OutcomeCompanyId",
                table: "Contracts",
                column: "OutcomeCompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Items_ContractItemId",
                table: "Contracts",
                column: "ContractItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Shops_IncomeShopId",
                table: "Contracts",
                column: "IncomeShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeStorages_Companies_OwningCompanyId",
                table: "IncomeStorages",
                column: "OwningCompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeStorages_Shops_OwningShopId",
                table: "IncomeStorages",
                column: "OwningShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Companies_CompanyId",
                table: "Items",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Companies_CompanyId",
                table: "Jobs",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Jobs_JobId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "Bots");

            migrationBuilder.DropTable(
                name: "CandidatesForJob");

            migrationBuilder.DropTable(
                name: "ContractRequests");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "StorageItems");

            migrationBuilder.DropTable(
                name: "ContractInfos");

            migrationBuilder.DropTable(
                name: "IncomeStorages");

            migrationBuilder.DropTable(
                name: "OutcomeStorages");

            migrationBuilder.DropTable(
                name: "PersonalStorages");

            migrationBuilder.DropTable(
                name: "StorageItemsWithPrice");

            migrationBuilder.DropTable(
                name: "TrunkStorages");

            migrationBuilder.DropTable(
                name: "WorkerStorages");

            migrationBuilder.DropTable(
                name: "ShowcaseStorages");

            migrationBuilder.DropTable(
                name: "Trucks");

            migrationBuilder.DropTable(
                name: "Shops");

            migrationBuilder.DropTable(
                name: "TruckGarages");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "CraftingCategories");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
