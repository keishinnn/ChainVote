using ChainVote.Data;
using ChainVote.Models.DatabaseEntities;
using ChainVote.Models.Dto;
using ChainVote.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChainVote.Controllers.OrganizationsController
{
    [Authorize(Roles = "Admin")]
    public class OrganizationsDataController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrganizationsDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: List of organizations
        public IActionResult Organizations()
        {
            var organizations = _context.OrganizationsData
                .Select(o => new OrganizationOverviewViewModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    Email = o.Email,
                    EventId = o.EventId,
                    EventName = o.Event != null ? o.Event.EventName : null,
                    Positions = o.Positions.Select(p => p.Title).ToList()
                }).ToList();

            var events = _context.EventsData.ToList();

            var model = new OrganizationPageViewModel
            {
                Organizations = organizations,
                Events = events
            };

            return View("~/Views/AdminView/Organizations.cshtml", model);
        }


            // POST: Create organization with positions
            [HttpPost]
            public async Task<IActionResult> CreateWithPositions([FromBody] OrganizationWithPositionsDto dto)
            {
                if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.ElectionType))
                    return BadRequest("Missing fields");

                var newOrg = new OrganizationsData
                {
                    Name = dto.Name,
                    Email = $"{dto.Name.ToLower().Replace(" ", "")}@fakeemail.com",
                    EventId = dto.EventId
                };

                _context.OrganizationsData.Add(newOrg);
                await _context.SaveChangesAsync();

                var positions = new List<OrganizationPosition>();

                // CSG or USG predefined positions
                string[] predefined = dto.ElectionType switch
                {
                    "CSG" => new[] {
                "President", "Vice President", "Senator",
                "Communication Secretary", "Information Secretary", "Treasurer", "Auditor"
            },
                    "USG" => new[] {
                "President", "Vice President", "Secretary",
                "Chairperson for Student Programs and Services",
                "Chairperson for Student Welfare", "Chairperson for Student Development"
            },
                    _ => Array.Empty<string>()
                };

                foreach (var predefinedTitle in predefined)
                {
                    var matched = dto.PositionsWithCandidates?.FirstOrDefault(p => p.PositionName == predefinedTitle);
                    var position = new OrganizationPosition
                    {
                        Title = predefinedTitle,
                        OrganizationId = newOrg.Id,
                        CandidateId = matched?.AssignedCandidate?.Id
                    };
                    positions.Add(position);
                }

                // Additional custom positions
                var additional = dto.PositionsWithCandidates?
                    .Where(p => !predefined.Contains(p.PositionName))
                    .ToList();

                if (additional != null)
                {
                    foreach (var pos in additional)
                    {
                        positions.Add(new OrganizationPosition
                        {
                            Title = pos.PositionName,
                            OrganizationId = newOrg.Id,
                            CandidateId = pos.AssignedCandidate?.Id
                        });
                    }
                }

                _context.OrganizationPosition.AddRange(positions);
                await _context.SaveChangesAsync();

                return Ok();
            }


        // GET: Details of one organization
        public async Task<IActionResult> Details(int id)
        {
            var organization = await _context.OrganizationsData
                .Include(o => o.Positions)
                .Include(o => o.Event)  // Include Event data if needed
                .FirstOrDefaultAsync(o => o.Id == id);

            if (organization == null)
                return NotFound();

            // Map organization data to OverviewViewModel for displaying
            var viewModel = new OrganizationOverviewViewModel
            {
                Id = organization.Id,
                Name = organization.Name,
                Email = organization.Email,
                EventId = organization.EventId,
                EventName = organization.Event?.EventName, // Adjust this to the exact event property you want
                Positions = organization.Positions.Select(p => p.Title).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrganization(int id)
        {
            var orgToDelete = await _context.OrganizationsData.FindAsync(id);
            if (orgToDelete == null)
            {
                TempData["ErrorMessage"] = "Organization not found.";
                return RedirectToAction("Organizations");
            }

            try
            {
                _context.OrganizationsData.Remove(orgToDelete);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Organization deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting organization: " + ex.Message;
            }

            return RedirectToAction("Organizations");
        }

        [HttpGet("GetOrganizationWithPositions/{id}")]
        public async Task<IActionResult> GetOrganizationWithPositions(int id)
        {
            var org = await _context.OrganizationsData
                .Include(o => o.Event) // ✅ Include Event to access ElectionType
                .Include(o => o.Positions)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (org == null) return NotFound();

            var result = new
            {
                id = org.Id,
                name = org.Name,
                positions = org.Positions.Select(p => new { id = p.Id, title = p.Title })
            };

            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetCandidates(int orgId)
        {
            // Fetch candidates not yet assigned to any organization (i.e., OrganizationId is null)
            var availableCandidates = _context.CandidatesData
                .Where(c => c.OrganizationId == null) // not yet linked to any org
                .Select(c => new {
                    id = c.Id,
                    name = c.ApplicationUser.FirstName + " " + c.ApplicationUser.LastName
                })
                .ToList();

            return Json(new { data = availableCandidates });
        }

        [HttpGet]
        public IActionResult GetAvailableElectionEvents()
        {
            var events = _context.EventsData
                .Where(e => e.Status == ElectionStatus.Awaiting)
                .Select(e => new {
                    id = e.Id,
                    eventName = e.EventName
                })
                .ToList();

            return Json(events);
        }
    }
}
