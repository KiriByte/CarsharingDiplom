using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarsharingProject.Data;
using CarsharingProject.Models;

namespace CarsharingProject.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            return _context.RentCars != null ?
                        View(await _context.RentCars.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.RentCars'  is null.");
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.RentCars == null)
            {
                return NotFound();
            }

            var rentCarsModel = await _context.RentCars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rentCarsModel == null)
            {
                return NotFound();
            }

            return View(rentCarsModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.RentCars == null)
            {
                return NotFound();
            }

            var rentCarsModel = await _context.RentCars.FindAsync(id);
            if (rentCarsModel == null)
            {
                return NotFound();
            }
            return View(rentCarsModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Vin,NumberCar,IsRentNow,IsAvailableForUsers,PricePerKM,PricePerMinute")] RentCarsModel rentCarsModel)
        {
            if (id != rentCarsModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rentCarsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentCarsModelExists(rentCarsModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(rentCarsModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.RentCars == null)
            {
                return NotFound();
            }

            var rentCarsModel = await _context.RentCars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rentCarsModel == null)
            {
                return NotFound();
            }

            return View(rentCarsModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.RentCars == null)
            {
                return Problem("Entity set 'ApplicationDbContext.RentCars'  is null.");
            }
            var rentCarsModel = await _context.RentCars.FindAsync(id);
            if (rentCarsModel != null)
            {
                _context.RentCars.Remove(rentCarsModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentCarsModelExists(int id)
        {
            return (_context.RentCars?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
