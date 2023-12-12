using Microsoft.AspNetCore.Mvc;
using SearchForDriversWebApp.Models;
using System.Diagnostics;
using SearchForDriversWebApp.ViewModels;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using SearchForDriversWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace SearchForDriversWebApp.Controllers
{
    [Authorize(Roles = "Admin, User, Driver")]

    public class HomeController : Controller
    {
        private DriverDbContext db;
       // private TripService _tripService;

        public HomeController(DriverDbContext db)
        {
            this.db = db;

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(TripModel model)
        {
            if (ModelState.IsValid)
            {
              var userId = db.Users.FirstOrDefault(x => x.Email == User.Identity.Name).Id;
                 var trip = new Trip
                {
                    DepartureLocation = model.DepartureLocation,
                    ArrivalLocation = model.ArrivalLocation,
                    Distance = model.Distance,
                    Status = "Опрацьовується оператором",
                    UserId = userId
                };
                db.Trips.Add(trip);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AssignmentTrips()
        {
            var assignments = db.Assignments.Include(x=>x.User).Include(x=>x.Trip).Where(x=>x.User.Email == User.Identity.Name && x.User.Role == "Driver").ToList(); 
            return View(assignments);
        }

        public IActionResult UserTrips()
        {

                var trips = db.Trips.Include(x => x.User).Where(x => x.User.Email == User.Identity.Name && x.User.Role == "User").ToList();
                return View(trips);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult NotFoundPage()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }

    }
}