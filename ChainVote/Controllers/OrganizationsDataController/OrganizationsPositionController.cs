using ChainVote.Data;
using ChainVote.Models.DatabaseEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChainVote.Controllers.AdminControllers
{
    [Authorize(Roles = "Admin")]
    public class OrganizationPositionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrganizationPositionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: All positions for a specific org
        public async Task<IActionResult> ByOrganization(int orgId)
        {
            var positions = await _context.OrganizationPosition
                .Where(p => p.OrganizationId == orgId)
                .Include(p => p.Candidate)
                .ToListAsync();

            return View(positions);
        }

        // POST: Link candidate to a position
        [HttpPost]
        public async Task<IActionResult> LinkCandidate(int positionId, int candidateId)
        {
            var position = await _context.OrganizationPosition.FindAsync(positionId);
            if (position == null)
                return NotFound();

            position.CandidateId = candidateId;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: Remove a position
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var position = await _context.OrganizationPosition.FindAsync(id);
            if (position == null)
                return NotFound();

            _context.OrganizationPosition.Remove(position);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }

}
