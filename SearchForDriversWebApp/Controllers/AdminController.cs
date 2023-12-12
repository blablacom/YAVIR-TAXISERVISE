using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SearchForDriversWebApp.Models;
using SearchForDriversWebApp.ViewModels;

namespace SearchForDriversWebApp.Controllers
{
    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        private DriverDbContext _db;
        // private TripService _tripService;

        public AdminController(DriverDbContext db)
        {
            this._db = db;

        }

        public IActionResult UserManagement()
        {
            var users = _db.Users.ToList();
            return View(users);
        }

        public IActionResult DeleteUser(int id)
        {
            var user = _db.Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(int id)
        {
            var user = await _db.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(UserManagement));
        }

        public IActionResult Trips()
        {
            var trips = _db.Trips.Include(x=>x.User).Where(x=>x.Status == "Опрацьовується оператором").ToList();
            return View(trips);
        }

        // AdminController.cs

        public IActionResult AssignDriver(int tripId)
        {
            var trip = _db.Trips.FirstOrDefault(t => t.Id == tripId);

            if (trip == null)
            {
                return NotFound();
            }

            var availableDrivers = _db.Users.Where(u => u.Role == "Driver").ToList();

            var viewModel = new AssignDriverViewModel
            {
                TripId = tripId,
                DepartureLocation = trip.DepartureLocation,
                ArrivalLocation = trip.ArrivalLocation,
                Distance = trip.Distance.ToString(),
                AvailableDrivers = availableDrivers
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDriver(int tripId, int driverId, int price)
        {
            var trip = await _db.Trips.FindAsync(tripId);
            var driver = await _db.Users.FindAsync(driverId);
            var assignmentCheck = await _db.Assignments.FirstOrDefaultAsync(x=>x.TripId == tripId);

            if (trip == null || driver == null || assignmentCheck != null)
            {
                return NotFound();
            }

            // Створення нового асайнменту
            var assignment = new Assignment
            {
                TripId = tripId,
                DriverId = driverId,
                Price = price,
            };
            trip.Status = "Водій в дорозі";
            _db.Trips.Update(trip);
            // Оновлення відносин в базі даних
            _db.Assignments.Add(assignment);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Trips));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(int id, string newRole)
        {
            var user = await _db.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Оновлення ролі користувача
            user.Role = newRole;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return RedirectToAction("UserManagement");
        }
    }
}