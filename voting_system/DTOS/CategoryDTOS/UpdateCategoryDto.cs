namespace voting_system.DTOS.CategoryDTOS
{
    public class UpdateCategoryDto
    {
        public long CategoryId { get; set; }

        public string Name { get; set; } = null!;
    }
}
