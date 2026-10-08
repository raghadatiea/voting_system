using Microsoft.AspNetCore.Mvc;
using voting_system.DTOS.VisitorDTO;
using voting_system.Models;

namespace voting_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitorController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        private static Dictionary<string, OtpData> _otpStorage = new();

        public VisitorController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        // Step 1: Request OTP
        [HttpPost("Register")]
        public IActionResult Register(RegisterVisitorDto dto)
        {
            // Check if phone already registered
            var existingVisitor = _dbContext.Visitors
                .FirstOrDefault(x => x.PhoneNumber == dto.PhoneNumber);

            if (existingVisitor != null)
            {
                return BadRequest("Phone number is already registered");
            }


            // Generate 6 digit OTP
            var random = new Random();

            string otp = random.Next(100000, 1000000).ToString();


            // Store OTP temporarily
            _otpStorage[dto.PhoneNumber] = new OtpData
            {
                Name = dto.Name,
                Otp = otp,
                ExpirationTime = DateTime.Now.AddMinutes(5)
            };


            // For testing only
            return Ok(new
            {
                Message = "OTP generated successfully",
                OTP = otp
            });
        }


        // Step 2: Verify OTP
        [HttpPost("VerifyOtp")]
        public IActionResult VerifyOtp(VerifyOtpDto dto)
        {
            // Check if OTP exists
            if (!_otpStorage.ContainsKey(dto.PhoneNumber))
            {
                return BadRequest("No OTP request found for this phone number");
            }


            // Get stored OTP
            var otpData = _otpStorage[dto.PhoneNumber];


            // Check expiration
            if (DateTime.Now > otpData.ExpirationTime)
            {
                _otpStorage.Remove(dto.PhoneNumber);

                return BadRequest("OTP has expired");
            }


            // Check OTP
            if (otpData.Otp != dto.Otp)
            {
                return BadRequest("Invalid OTP");
            }


            // Check again before inserting
            var existingVisitor = _dbContext.Visitors
                .FirstOrDefault(x => x.PhoneNumber == dto.PhoneNumber);

            if (existingVisitor != null)
            {
                _otpStorage.Remove(dto.PhoneNumber);

                return BadRequest("Phone number is already registered");
            }


            // Create Visitor
            var visitor = new Visitor
            {
                Name = otpData.Name,
                PhoneNumber = dto.PhoneNumber,
                IsPhoneVerified = true
            };


            _dbContext.Visitors.Add(visitor);

            _dbContext.SaveChanges();


            // Remove OTP after successful verification
            _otpStorage.Remove(dto.PhoneNumber);


            return Ok(new
            {
                Message = "Visitor registered successfully",
                VisitorId = visitor.VisitorId,
                Name = visitor.Name,
                PhoneNumber = visitor.PhoneNumber,
                IsPhoneVerified = visitor.IsPhoneVerified
            });
        }
    }


    public class OtpData
    {
        public string Name { get; set; } = null!;

        public string Otp { get; set; } = null!;

        public DateTime ExpirationTime { get; set; }
    }
}