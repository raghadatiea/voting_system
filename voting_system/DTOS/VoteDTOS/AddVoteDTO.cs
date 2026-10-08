namespace voting_system.DTOS.VoteDTOS
{
    public class AddVoteDTO
    {
        public long VisitorId { get; set; }
        public long ExhibitorId { get; set; }
        public long CategoryId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
