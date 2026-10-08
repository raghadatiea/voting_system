using voting_system.DTOS.CategoryDTO;
using voting_system.DTOS.VoteDTOS;
using voting_system.Models;

namespace voting_system.DTOS.ExhibitorDTO
{
    public class ExhibitorDto
    {
        public long ExhibitorsId { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string PhotoUrl { get; set; } = null!;

        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();

    }
}
