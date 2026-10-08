namespace voting_system.DTOS.ExhibitorDTOS
{
    public class AddExhibitorDto
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string PhotoUrl { get; set; } = null!;

        public List<long> CategoryIds { get; set; } = new List<long>();
    }
}
