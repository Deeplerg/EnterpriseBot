namespace EnterpriseBot.Api.Models.ModelCreationParams.Business
{
    public class JobCreationParams
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public decimal Salary { get; set; }
        public long CompanyId { get; set; }

        public long RecipeId { get; set; }
    }
}
