using ContactManagerApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContactManagerApp.Controllers
{
    public class UserController : Controller
    {
        ApplicationContext _context;
        private IWebHostEnvironment _appEnvironment;

        public UserController(ApplicationContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        [HttpGet]

        public IActionResult Index()
        {

            return View(_context.Users.ToList());
        }

        [HttpPost]
        public IActionResult Delete(int? id)
        {
            if (id != null)
            {
                User? user =  _context.Users.FirstOrDefault(p => p.Id == id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                     _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }

        public IActionResult Edit(int? id)
        {
            if (id != null)
            {
                User? user = _context.Users.FirstOrDefault(p => p.Id == id);
                if (user != null) return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
