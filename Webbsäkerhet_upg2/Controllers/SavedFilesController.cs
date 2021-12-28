#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Webbsäkerhet_upg2.Data;
using Webbsäkerhet_upg2.Models;
using Webbsäkerhet_upg2.Utilities;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;

namespace Webbsäkerhet_upg2.Controllers
{
    public class SavedFilesController : Controller
    {
        private readonly Webbsäkerhet_upg2Context _context;
        private readonly long fileSizeLimit = 10 * 1048576;
        private readonly string[] permittedExtensions = { ".jpg" };

        public SavedFilesController(Webbsäkerhet_upg2Context context)
        {
            _context = context;
        }

        // GET: ApplicationFiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.SavedFile.ToListAsync());
        }


        [HttpPost]
        [Route(nameof(UploadFile))]
        public async Task<IActionResult> UploadFile()
        {
            var theWebRequest = HttpContext.Request;

            // validation of Content-Type
            // 1. first, it must be a form-data request
            // 2. a boundary should be found in the Content-Type
            if (!theWebRequest.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(theWebRequest.ContentType, out var theMediaTypeHeader) ||
                string.IsNullOrEmpty(theMediaTypeHeader.Boundary.Value))
            {
                return new UnsupportedMediaTypeResult();
            }

            var reader = new MultipartReader(theMediaTypeHeader.Boundary.Value, theWebRequest.Body);
            var section = await reader.ReadNextSectionAsync();

            // This sample try to get the first file from request and save it
            // Make changes according to your needs in actual use
            while (section != null)
            {
                var DoesItHaveContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out var theContentDisposition);

                if (DoesItHaveContentDispositionHeader && theContentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(theContentDisposition.FileName.Value))
                {
                    // Don't trust any file name, file extension, and file data from the request unless you trust them completely
                    // Otherwise, it is very likely to cause problems such as virus uploading, disk filling, etc
                    // In short, it is necessary to restrict and verify the upload
                    // Here, we just use the temporary folder and a random file name

                    SavedFile savedFile = new SavedFile();
                    savedFile.UntrustedName = HttpUtility.HtmlEncode(theContentDisposition.FileName.Value);
                    savedFile.TimeStamp = DateTime.UtcNow;

                    savedFile.Content =
                            await FileHelpers.ProcessStreamedFile(section, theContentDisposition,
                                ModelState, permittedExtensions, fileSizeLimit);
                    if (savedFile.Content.Length == 0)
                    {
                        return RedirectToAction("Index", "savedFiles");
                    }
                    savedFile.Size = savedFile.Content.Length;

                    await _context.SavedFile.AddAsync(savedFile);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "savedFiles");

                }

                section = await reader.ReadNextSectionAsync();
            }

            // If the code runs to this location, it means that no files have been saved
            return BadRequest("No files data in the request.");
        }


        // GET: ApplicationFiles/Download/5
        public async Task<IActionResult> Download(Guid? id)
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

            return File(savedFile.Content, MediaTypeNames.Application.Octet, savedFile.UntrustedName);
        }

        // GET: ApplicationFiles/Delete/5
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

        // POST: ApplicationFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var savedFile = await _context.SavedFile.FindAsync(id);
            _context.SavedFile.Remove(savedFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationFileExists(Guid id)
        {
            return _context.SavedFile.Any(e => e.Id == id);
        }
    }
}
