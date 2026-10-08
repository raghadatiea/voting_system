using voting_system.DTOS.ExhibitorDTO;
using voting_system.DTOS.VoteDTOS;
using voting_system.Models;

namespace voting_system.DTOS.CategoryDTO
{
    public class CategoryDto
    {
        public long CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public List<VoteDto> Votes;
        public List<ExhibitorDto> Exhibitors;
    }
}
