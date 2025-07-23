using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFeedbackAPI.Data;
using SmartFeedbackAPI.Models;

namespace SmartFeedbackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics()
        {
            var byCategory = await _context.Feedbacks
                .GroupBy(f => f.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToListAsync();

            var bySentiment = await _context.Feedbacks
                .GroupBy(f => f.Sentiment)
                .Select(g => new { Sentiment = g.Key, Count = g.Count() })
                .ToListAsync();

            var byDate = await _context.Feedbacks
                .GroupBy(f => f.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            return Ok(new { byCategory, bySentiment, byDate });
        }

        [HttpGet("all-feedback")]
        public async Task<IActionResult> GetAllFeedback()
        {
            var all = await _context.Feedbacks.Include(f => f.User).ToListAsync();
            return Ok(all);
        }
    }
}
