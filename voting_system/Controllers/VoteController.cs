using Microsoft.AspNetCore.Mvc;
using voting_system.DTOS.VoteDTOS;
using voting_system.Models;

namespace voting_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public VoteController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult AddVote(AddVoteDTO dto)
        {
            // Get voting settings
            var setting = _dbContext.VotingSettings.FirstOrDefault();

            if (setting == null)
                return BadRequest("Voting settings not found");


            // Check voting time
            var now = DateTime.Now;

            if (now < setting.StartTime || now > setting.EndTime)
                return BadRequest("Voting is currently closed");


            // Check visitor
            var visitor = _dbContext.Visitors
                .FirstOrDefault(x => x.VisitorId == dto.VisitorId);

            if (visitor == null)
                return NotFound("Visitor not found");


            // Check phone verification
            if (!visitor.IsPhoneVerified)
                return BadRequest("Phone number is not verified");


            // Check GPS location
            var distance = CalculateDistance(
                dto.Latitude,
                dto.Longitude,
                setting.Latitude,
                setting.Longitude
            );

            if (distance > setting.Radius)
            {
                return BadRequest(
                    $"You are outside the allowed voting area. Distance: {distance:F2} meters");
            }


            // Check category
            var category = _dbContext.Categories
                .FirstOrDefault(x => x.CategoryId == dto.CategoryId);

            if (category == null)
                return NotFound("Category not found");


            // Check exhibitor
            var exhibitor = _dbContext.Exhibtiors
                .FirstOrDefault(x => x.ExhibitorsId == dto.ExhibitorId);

            if (exhibitor == null)
                return NotFound("Exhibitor not found");


            // Check if exhibitor belongs to category
            var belongsToCategory = _dbContext.Exhibtiors
                .Where(x => x.ExhibitorsId == dto.ExhibitorId)
                .SelectMany(x => x.Categories)
                .Any(x => x.CategoryId == dto.CategoryId);

            if (!belongsToCategory)
                return BadRequest(
                    "This exhibitor does not belong to this category");


            // Check duplicate vote
            var existingVote = _dbContext.Votes
                .FirstOrDefault(x =>
                    x.VisitorId == dto.VisitorId &&
                    x.CategoryId == dto.CategoryId);

            if (existingVote != null)
                return BadRequest(
                    "Visitor has already voted in this category");


            // Create vote
            var vote = new Vote
            {
                VisitorId = dto.VisitorId,
                ExhibitorId = dto.ExhibitorId,
                CategoryId = dto.CategoryId,
                CreatedAt = DateTime.Now
            };


            // Save vote
            _dbContext.Votes.Add(vote);
            _dbContext.SaveChanges();


            // Success response
            return Ok(new
            {
                Message = "Vote registered successfully",
                VoteId = vote.VoteId,
                VisitorId = vote.VisitorId,
                ExhibitorId = vote.ExhibitorId,
                CategoryId = vote.CategoryId,
                Distance = Math.Round(distance, 2),
                CreatedAt = vote.CreatedAt
            });
        }


        // Calculate distance between two GPS coordinates
        private double CalculateDistance(
            double userLatitude,
            double userLongitude,
            double allowedLatitude,
            double allowedLongitude)
        {
            const double earthRadius = 6371000;

            var latitude1 = DegreesToRadians(userLatitude);
            var latitude2 = DegreesToRadians(allowedLatitude);

            var latitudeDifference =
                DegreesToRadians(allowedLatitude - userLatitude);

            var longitudeDifference =
                DegreesToRadians(allowedLongitude - userLongitude);

            var a =
                Math.Sin(latitudeDifference / 2) *
                Math.Sin(latitudeDifference / 2)
                +
                Math.Cos(latitude1) *
                Math.Cos(latitude2) *
                Math.Sin(longitudeDifference / 2) *
                Math.Sin(longitudeDifference / 2);

            var c =
                2 * Math.Atan2(
                    Math.Sqrt(a),
                    Math.Sqrt(1 - a));

            return earthRadius * c;
        }


        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}