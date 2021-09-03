using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;

        public HomeController(MyContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.AllWeddings = _context.Weddings.ToList();
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(user => user.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");

                    return View("Index", newUser);
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

                _context.Users.Add(newUser);
                _context.SaveChanges();

                return RedirectToAction("dashboard");
            }
            return View("Index");
        }

        [HttpPost("checkLogin")]
        public IActionResult CheckLogin(LoginUser login)
        {
            if(ModelState.IsValid)
            {
                User userInDb = _context.Users.FirstOrDefault(user => user.Email == login.LoginEmail);

                if(userInDb == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid login!");

                    return View("Index", login);
                }
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();

                var result = hasher.VerifyHashedPassword(login, userInDb.Password, login.LoginPassword);

                if(result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid login!");

                    return View("Index", login);
                }

                HttpContext.Session.SetInt32("userId", userInDb.UserId);

                return RedirectToAction("dashboard");

            }

            return View("Index");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? loggedUserId = HttpContext.Session.GetInt32("userId");
            if(loggedUserId == null) return RedirectToAction("Index");

            ViewBag.LoggedUser = _context.Users.FirstOrDefault(user => user.UserId == loggedUserId);
            ViewBag.AllWeddings = _context.Weddings
                .Include(wed => wed.Guests)
                .ThenInclude(use => use.User)
                .ToList();

            return View("Dashboard");
        }
        

        [HttpGet("logout")]
        public IActionResult Logout ()
        {
            HttpContext.Session.Clear ();
            return View ("Index");
        }

        [HttpGet("/wedding/new")]
        public IActionResult NewWedding()
        {
            int? loggedUserId = HttpContext.Session.GetInt32("userId");
            if(loggedUserId == null) return RedirectToAction("Index");

            return View();
        }

        [HttpPost("/wedding/create")]
        public IActionResult CreateWedding(Wedding newWedding)
        {
            int? loggedUserId = HttpContext.Session.GetInt32("userId");
            if(loggedUserId == null) return RedirectToAction("Index");

            if(ModelState.IsValid)
            {
                newWedding.UserId = (int)loggedUserId;
                _context.Add(newWedding);
                _context.SaveChanges();
                
                return RedirectToAction("dashboard");
            }
            ViewBag.AllWeddings = _context.Weddings.ToList();
            return View("NewWedding");
        }

        [HttpGet("viewwedding/{weddingId}")]
        public IActionResult SingleWedding(int weddingId)
        {
            int? loggedUserId = HttpContext.Session.GetInt32("userId");
            if(loggedUserId == null) return RedirectToAction("Index");

            ViewBag.LoggedUser = _context.Users.FirstOrDefault(user => user.UserId == loggedUserId);
            ViewBag.SingleWedding = _context.Weddings.FirstOrDefault(wed => wed.WeddingId == weddingId);
            ViewBag.AllWeddings = _context.Weddings
                .Include(wed => wed.Guests)
                .ThenInclude(use => use.User)
                .ToList();

            return View();
        }

        [HttpGet("wedding/{weddingId}/delete")]
        public IActionResult DeleteWedding(int weddingId)
        {
            int? loggedUserId = HttpContext.Session.GetInt32("userId");
            if(loggedUserId == null) return RedirectToAction("Index");

            Wedding deleteMe = _context.Weddings
                .FirstOrDefault(wed => wed.WeddingId == weddingId);
            
            _context.Weddings.Remove(deleteMe);
            _context.SaveChanges();

            return RedirectToAction("dashboard");
        }

        [HttpGet("rsvp/{weddingId}")]
        public IActionResult RSVPWedding(int weddingId)
        {
            int? loggedUserId = HttpContext.Session.GetInt32("userId");
            if(loggedUserId == null) return RedirectToAction("Index");

            User loggedUser = _context.Users.FirstOrDefault(user => user.UserId == loggedUserId);
            Wedding singleWedding = _context.Weddings.FirstOrDefault(wed => wed.WeddingId == weddingId);
            RSVP newRSVP = new RSVP()
            {
                UserId = loggedUser.UserId,
                WeddingId = singleWedding.WeddingId,
                User = loggedUser,
                Wedding = singleWedding,
            };

            _context.RSVPs.Add(newRSVP);
            _context.SaveChanges();

            return Redirect("/dashboard");
        }

        [HttpGet("unrsvp/{weddingId}")]
        public IActionResult UNRSVPWedding(int weddingId)
        {
            int? loggedUserId = HttpContext.Session.GetInt32("userId");
            if(loggedUserId == null) return RedirectToAction("Index");

            User loggedUser = _context.Users.FirstOrDefault(user => user.UserId == loggedUserId);
            Wedding singleWedding = _context.Weddings.FirstOrDefault(wed => wed.WeddingId == weddingId);
            List<RSVP> AllRSVPs = _context.RSVPs
                .Where(r => r.WeddingId == singleWedding.WeddingId).ToList();

            RSVP theRSVP = AllRSVPs.FirstOrDefault(r => r.UserId == loggedUser.UserId);

            _context.Remove(theRSVP);
            _context.SaveChanges();

            return Redirect("/dashboard");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
