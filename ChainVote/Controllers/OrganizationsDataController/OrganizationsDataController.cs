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
            // Initialize database context
            _context = context;
        }

        // GET: List all organizations with their related events and positions
        public IActionResult Organizations()
        {
            // Fetch organizations with their event names and position titles
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

            // Retrieve all election events for possible filtering or display
            var events = _context.EventsData.ToList();

            // Compose the model combining organizations and events data
            var model = new OrganizationPageViewModel
            {
                Organizations = organizations,
                Events = events
            };

            // Render the Organizations view with the assembled model
            return View("~/Views/AdminView/Organizations.cshtml", model);
        }

        // POST: Create a new organization along with its positions from a DTO payload
        [HttpPost]
        public async Task<IActionResult> CreateWithPositions([FromBody] OrganizationWithPositionsDto dto)
        {
            // Validate required fields in the DTO
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.ElectionType))
                return BadRequest("Missing fields");

            // Create a new organization entity based on the DTO data
            var newOrg = new OrganizationsData
            {
                Name = dto.Name,
                Email = $"{dto.Name.ToLower().Replace(" ", "")}@fakeemail.com", // Generate a placeholder email
                EventId = dto.EventId,
                ElectionType = Enum.Parse<ElectionType>(dto.ElectionType) // Convert string to enum
            };

            // Add new organization to database context and save
            _context.OrganizationsData.Add(newOrg);
            await _context.SaveChangesAsync();

            // Map positions from DTO to entity and associate with the new organization
            var positions = dto.Positions?.Select(pos => new OrganizationPosition
            {
                Title = pos,
                OrganizationId = newOrg.Id
            }).ToList();

            // If there are positions, add them to the database and save changes
            if (positions != null && positions.Any())
            {
                _context.OrganizationPosition.AddRange(positions);
                await _context.SaveChangesAsync();
            }

            // Return success response
            return Ok();
        }

        // GET: Retrieve detailed information about a specific organization
        public async Task<IActionResult> Details(int id)
        {
            // Query organization with positions and associated event by id
            var organization = await _context.OrganizationsData
                .Include(o => o.Positions)
                .Include(o => o.Event)  // Include event details for display
                .FirstOrDefaultAsync(o => o.Id == id);

            // Return 404 if organization is not found
            if (organization == null)
                return NotFound();

            // Map entity data to a view model for presentation
            var viewModel = new OrganizationOverviewViewModel
            {
                Id = organization.Id,
                Name = organization.Name,
                Email = organization.Email,
                EventId = organization.EventId,
                EventName = organization.Event?.EventName,
                Positions = organization.Positions.Select(p => p.Title).ToList()
            };

            // Render the details view with the organization data
            return View(viewModel);
        }

        // POST: Delete an organization by id with error handling
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrganization(int id)
        {
            // Find the organization entity by id
            var orgToDelete = await _context.OrganizationsData.FindAsync(id);

            // If not found, show error and redirect
            if (orgToDelete == null)
            {
                TempData["ErrorMessage"] = "Organization not found.";
                return RedirectToAction("Organizations");
            }

            try
            {
                // Remove organization and save changes
                _context.OrganizationsData.Remove(orgToDelete);
                await _context.SaveChangesAsync();

                // Show success message
                TempData["SuccessMessage"] = "Organization deleted successfully.";
            }
            catch (Exception ex)
            {
                // Log and show error message on failure
                TempData["ErrorMessage"] = "Error deleting organization: " + ex.Message;
            }

            // Redirect back to organizations list
            return RedirectToAction("Organizations");
        }

        // GET: Retrieve organization data with its positions for editing or display
        [HttpGet("GetOrganizationWithPositions/{id}")]
        public async Task<IActionResult> GetOrganizationWithPositions(int id)
        {
            // Load organization including event and positions by id
            var org = await _context.OrganizationsData
                .Include(o => o.Event) // Include event to access election type if needed
                .Include(o => o.Positions)
                .FirstOrDefaultAsync(o => o.Id == id);

            // Return 404 if not found
            if (org == null) return NotFound();

            // Prepare a JSON result with organization's basic info and position list
            var result = new
            {
                id = org.Id,
                name = org.Name,
                positions = org.Positions.Select(p => new { id = p.Id, title = p.Title })
            };

            // Return JSON data for client-side use
            return Ok(result);
        }

        // GET: Get election events that are currently awaiting to be used in dropdowns or filters
        [HttpGet]
        public IActionResult GetAvailableElectionEvents()
        {
            // Query events with status 'Awaiting' and select relevant properties
            var events = _context.EventsData
                .Where(e => e.Status == ElectionStatus.Awaiting)
                .Select(e => new {
                    id = e.Id,
                    eventName = e.EventName
                })
                .ToList();

            // Return events as JSON for front-end consumption
            return Json(events);
        }
    }
}
