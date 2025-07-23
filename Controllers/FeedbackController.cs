using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFeedbackAPI.Data;
using SmartFeedbackAPI.Models;
using System.Security.Claims;

namespace SmartFeedbackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class FeedbackController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedbackController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
       

[HttpPost]
public async Task<IActionResult> SubmitFeedback([FromBody] Feedback model)
{
    try
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Invalid token or user ID.");

        model.UserId = userId; 
        model.CreatedAt = DateTime.UtcNow;
        model.Sentiment = "Pending";

        // ✅ Prevent EF Core from trying to re-insert the User object
        model.User = null;

        _context.Feedbacks.Add(model);
        await _context.SaveChangesAsync();

        return Ok(model);
    }
    catch (DbUpdateException dbEx)
    {
        return StatusCode(500, new
        {
            message = "DB error",
            error = dbEx.InnerException?.Message ?? dbEx.Message
        });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            message = "Error submitting feedback",
            error = ex.Message
        });
    }
}

        [HttpGet("my-feedback")]
        public async Task<IActionResult> GetMyFeedback()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var feedbacks = await _context.Feedbacks
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return Ok(feedbacks);
        }
    }
}
