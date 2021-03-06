using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Pintxos.Models;
using Pintxos.Services;

namespace Pintxos.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ContestController : Controller
    {
        private readonly IContestService _contestService;
        public ContestController(IContestService contestService)
        {
            _contestService = contestService;
        }
        public async Task<IActionResult> Index()
        {
            var contests = await _contestService.GetContestsAsync();

            var model = new ContestViewModel()
            {
                Items = contests
            };

            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddContest(ContestModel newContest)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            var model = await _contestService.AddContestAsync(newContest);
            if (model == null)
            {
                return BadRequest("Could not add Contest");
            }

            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkActive(Guid id)
        {
            if (id == Guid.Empty)
            {
                return RedirectToAction("Index");
            }

            var successful = await _contestService.MarkContestAsActive(id);
            if(!successful)
            {
                return BadRequest("Could not activate contest");
            }
            return RedirectToAction("Index");
        }
    }
}