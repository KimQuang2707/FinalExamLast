using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalExamLast.Data;
using FinalExamLast.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using static System.Net.Mime.MediaTypeNames;
using System.Net.NetworkInformation;

namespace FinalExamLast.Controllers
{
    public class JobApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public JobApplicationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: JobApplications
        [Authorize(Roles ="Candidate")]
        public async Task<IActionResult> Index()
        {
            // Lấy thông tin người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User Not Found.");
            }

            // Tìm CandidateId dựa trên UserId của người dùng hiện tại
            var candidate = await _context.Candidate.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (candidate == null)
            {
                return NotFound("Information of Candidate Not Found.");
            }

            // Lấy danh sách các Application mà Candidate này đã đăng ký
            var JobApplications = await _context.JobApplication
                                        .Include(a => a.JobListing) // Bao gồm thông tin JobListing nếu cần
                                        .Where(a => a.CandidateId == candidate.Id)
                                        .ToListAsync();

            return View(JobApplications);
        }

        // GET: JobApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplication
                .Include(j => j.Candidate)
                .Include(j => j.JobListing)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        // GET: JobApplications/Create
        //public IActionResult Create()
        //{
        //    ViewData["CandidateId"] = new SelectList(_context.Candidate, "Id", "Id");
        //    ViewData["JobListingId"] = new SelectList(_context.JobListing, "Id", "Id");
        //    return View();
        //}
        [HttpGet]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobListingExists = await _context.JobListing.FindAsync(id);
            if (jobListingExists == null)
            {
                return NotFound("Job listing not found.");
            }

            // Lấy thông tin về người dùng đã xác thực
            var currentUser = await _userManager.GetUserAsync(User);

            int? candidateId = null;
            int result;
            if (int.TryParse(currentUser.Id, out result))
            {
                candidateId = result;
            }

            var applicationModel = new JobApplication
            {
                JobListingId = id.Value,
                CandidateId = candidateId
            };


            return View(applicationModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int JobListingId, [Bind("name,RequiredQualifications,JobListingId,CandidateId")] JobApplication jobApplication)
        {
            if (!ModelState.IsValid)
            {
                return View(jobApplication);
            }

            var user = await _userManager.GetUserAsync(User);
            var candidate = await _context.Candidate.FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (candidate == null)
            {
                return NotFound("Candidate not found.");
            }

            var jobListExists = await _context.JobListing.AnyAsync(jl => jl.Id == JobListingId);
            if (!jobListExists)
            {
                return NotFound("Job listing not found.");
            }

            jobApplication.CandidateId = candidate.Id;
            jobApplication.JobListingId = JobListingId; 

           

            _context.Add(jobApplication);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: JobApplications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplication.FindAsync(id);
            if (jobApplication == null)
            {
                return NotFound();
            }
            return View(jobApplication);
        }

        // POST: JobApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,RequiredQualifications,JobListingId,CandidateId")] JobApplication jobApplication)
        {
            if (id != jobApplication.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    var existingApplication = await _context.JobApplication.FindAsync(id);

                    // Kiểm tra xem có tìm thấy ứng dụng không
                    if (existingApplication == null)
                    {
                        return NotFound("NOT Found Application.");
                    }

                    // Cập nhật ứng dụng vào database
                    _context.Update(existingApplication);
                  
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobApplicationExists(jobApplication.Id))
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
            return View(jobApplication);
        }

        // GET: JobApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {

                return NotFound();
            }

            var jobApplication = await _context.JobApplication
                .Include(j => j.Candidate)
                .Include(j => j.JobListing)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        // POST: JobApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobApplication = await _context.JobApplication.FindAsync(id);
            if (jobApplication != null)
            {
                _context.JobApplication.Remove(jobApplication);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobApplicationExists(int id)
        {
            return _context.JobApplication.Any(e => e.Id == id);
        }
        [Authorize(Roles = "Candidate")]
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> CandidateFullJobs(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var jobListQuery = _context.JobListing.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                jobListQuery = jobListQuery.Where(j => j.Name.Contains(searchString));
            }

            var jobList = await jobListQuery.ToListAsync();
            return View(jobList);
        }
        public async Task<IActionResult> ViewAP(int jobListId)
        {
            var JobApplications = await _context.JobApplication
                                    .Where(a => a.JobListingId == jobListId)
                                    .Include(a => a.Candidate) // Bao gồm thông tin Candidate
                                    .ToListAsync();

            // Bạn có thể cần một ViewModel để bao gồm cả thông tin JobList nếu cần
            // Ví dụ: var viewModel = new JobListApplicationsViewModel { Applications = applications, JobList = ... };

            return View(JobApplications); // hoặc return View(viewModel);
        }
    }
}
