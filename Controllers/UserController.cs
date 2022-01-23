using System;
using System.Threading.Tasks;
using BrokenCode.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrokenCode.Controllers
{
    public class UserController : Controller
    {
        private readonly UserDbContext _userDb;

        public UserController(UserDbContext userDb)
        {
            _userDb = userDb;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _userDb.Users.ToDictionaryAsync(u => u.Id, u => u);
            return View(model);
        }

        public async Task<IActionResult> AddUserPage()
        {
            return View(new User());
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            _userDb.Users.Add(user);

            await _userDb.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var entityToRemove = await _userDb.Users.FindAsync(userId);
            _userDb.Users.Remove(entityToRemove);

            await _userDb.SaveChangesAsync();
            
            return RedirectToAction("Index");
        }
    }
}