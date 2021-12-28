#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Webbsäkerhet_upg2.Data;
using Webbsäkerhet_upg2.Models;

namespace Webbsäkerhet_upg2.Controllers
{
    public class SavedFilesController : Controller
    {
        private readonly Webbsäkerhet_upg2Context _context;

        public SavedFilesController(Webbsäkerhet_upg2Context context)
        {
            _context = context;
        }

        // GET: SavedFiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.SavedFile.ToListAsync());
        }

        // GET: SavedFiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savedFile = await _context.SavedFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (savedFile == null)
            {
                return NotFound();
            }

            return View(savedFile);
        }

        // GET: SavedFiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SavedFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UntrustedName,TimeStamp,Size,Content")] SavedFile savedFile)
        {
            if (ModelState.IsValid)
            {
                savedFile.Id = Guid.NewGuid();
                _context.Add(savedFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(savedFile);
        }

        // GET: SavedFiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savedFile = await _context.SavedFile.FindAsync(id);
            if (savedFile == null)
            {
                return NotFound();
            }
            return View(savedFile);
        }

        // POST: SavedFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UntrustedName,TimeStamp,Size,Content")] SavedFile savedFile)
        {
            if (id != savedFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(savedFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SavedFileExists(savedFile.Id))
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
            return View(savedFile);
        }

        // GET: SavedFiles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savedFile = await _context.SavedFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (savedFile == null)
            {
                return NotFound();
            }

            return View(savedFile);
        }

        // POST: SavedFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var savedFile = await _context.SavedFile.FindAsync(id);
            _context.SavedFile.Remove(savedFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SavedFileExists(Guid id)
        {
            return _context.SavedFile.Any(e => e.Id == id);
        }
    }
}
