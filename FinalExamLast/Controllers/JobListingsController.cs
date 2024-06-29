using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalExamLast.Data;
using FinalExamLast.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FinalExamLast.Controllers
{
    [Authorize(Roles = "Employee")]
    public class JobListingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public JobListingsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: JobListings
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Not Found User");
            }
            var employee = await _context.Employee.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (employee == null)
            {
                return NotFound("Information of User Not Found");
            }
            // lay danh sach cac Application ma Candidate nay da dang ky
            var joblistings = await _context.JobListing
                .Include(a => a.JobApplication)
                .Where(a => a.EmployeeId == employee.Id)
                .ToListAsync();
            return View(joblistings);
        }

        // GET: JobListings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobListing = await _context.JobListing
                .Include(j => j.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobListing == null)
            {
                return NotFound();
            }

            return View(jobListing);
        }
        // GET: JobListings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JobListings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] JobListing jobList)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    // Nếu không tìm thấy thông tin người dùng, trả về lỗi
                    return NotFound("Not found User.");
                }
                var employee = await _context.Employee.FirstOrDefaultAsync(e => e.UserId == user.Id);
                if (employee == null)
                {
                    return NotFound("Not found Employee.");
                }
                jobList.EmployeeId = employee.Id; // Gán EmployerId vao cong viec vua tao
                _context.JobListing.Add(jobList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jobList);
        }
    


        // GET: JobListings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobListing = await _context.JobListing.FindAsync(id);
            if (jobListing == null)
            {
                return NotFound();
            }
            return View(jobListing);
        }

        // POST: JobListings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,EmployeeId")] JobListing jobListing)
        {
            if (id != jobListing.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm và tải thông tin ứng dụng hiện có từ database
                    var existingJobList = await _context.JobListing.FindAsync(id);

                    // Kiểm tra xem có tìm thấy ứng dụng không
                    if (existingJobList == null)
                    {
                        return NotFound("Không tìm thấy ứng dụng.");
                    }

               
                    existingJobList.Name = jobListing.Name;
                    existingJobList.Description = jobListing.Description;


                    // Cập nhật ứng dụng vào database
                    _context.Update(existingJobList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobListingExists(jobListing.Id))
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
            return View(jobListing);
        }

        // GET: JobListings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobListing = await _context.JobListing
                .Include(j => j.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobListing == null)
            {
                return NotFound();
            }

            return View(jobListing);
        }

        // POST: JobListings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobListing = await _context.JobListing.FindAsync(id);
            if (jobListing != null)
            {
                _context.JobListing.Remove(jobListing);
                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
            }

            return RedirectToAction(nameof(Index));
        }

        private bool JobListingExists(int id)
        {
            return _context.JobListing.Any(e => e.Id == id);
        }
    }
}
