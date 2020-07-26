namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company
{
    public class ProductionRobotCreationParams
    {
        public string Name { get; set; }

        public long CompanyId { get; set; }
        public long RecipeId { get; set; }
        public long CompanyStorageId { get; set; }
    }
}
