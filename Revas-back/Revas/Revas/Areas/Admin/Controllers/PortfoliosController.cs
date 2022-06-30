using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Revas.DAL;
using Revas.Models;

namespace Revas.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PortfoliosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PortfoliosController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Portfolios.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolio = await _context.Portfolios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (portfolio == null)
            {
                return NotFound();
            }

            return View(portfolio);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Portfolio portfolio)
        {
            if (!portfolio.Img.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("","Please upload image");
            }

            if (portfolio.Img.Length/1024>1000)
            {
                ModelState.AddModelError("","Image is too large");
            }


            string path = _env.WebRootPath + @"\img";
            string filename = Guid.NewGuid().ToString() + portfolio.Img.FileName;
            string final = Path.Combine(path, filename);

            using (FileStream fs = new FileStream(final, FileMode.Create))
            {
                await portfolio.Img.CopyToAsync(fs);
            }

            portfolio.Image = filename;

            if (ModelState.IsValid)
            {
                _context.Add(portfolio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(portfolio);
        }

        // GET: Admin/Portfolios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var portfolio = await _context.Portfolios.FindAsync(id);
            if (portfolio == null)
            {
                return NotFound();
            }
            return View(portfolio);
        }

        // POST: Admin/Portfolios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Portfolio portfolio)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (id != portfolio.Id)
            {
                return NotFound();
            }

            if (!portfolio.Img.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("", "Please upload image");
            }

            if (portfolio.Img.Length / 1024 > 1000)
            {
                ModelState.AddModelError("", "Image is too large");
            }

            string path = _env.WebRootPath + @"\img";
            string filename = Guid.NewGuid().ToString() + portfolio.Img.FileName;
            string final = Path.Combine(path, filename);

            if (System.IO.File.Exists(final))
            {
                System.IO.File.Delete(final);
            }

            using (FileStream fs = new FileStream(final, FileMode.Create))
            {
                await portfolio.Img.CopyToAsync(fs);
            }

            portfolio.Image = filename;


            _context.Update(portfolio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: Admin/Portfolios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolio = await _context.Portfolios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (portfolio == null)
            {
                return NotFound();
            }

            return View(portfolio);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var portfolio = await _context.Portfolios.FindAsync(id);

            if (System.IO.File.Exists(Path.Combine(_env.WebRootPath + @"\img", portfolio.Image)))
            {
                System.IO.File.Delete(Path.Combine(_env.WebRootPath + @"\img", portfolio.Image));
            }
            _context.Portfolios.Remove(portfolio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PortfolioExists(int id)
        {
            return _context.Portfolios.Any(e => e.Id == id);
        }
    }
}
