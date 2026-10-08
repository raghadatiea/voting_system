using voting_system.Models;

namespace voting_system.DTOS.VoteDTOS
{
    public class VoteDto
    {
        public long VoteId { get; set; }

        public long ExhibitorId { get; set; }

        public long VisitorId { get; set; }

        public DateTime CreatedAt { get; set; }

        public long CategoryId { get; set; }

 
    }
}
