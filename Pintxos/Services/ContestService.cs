using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pintxos.Data;
using Pintxos.Models;

namespace Pintxos.Services
{
    public class ContestService : IContestService
    {
        private readonly ApplicationDbContext _context;

        public ContestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ContestModel[]> GetContestsAsync()
        {
            var items = _context.Contests.ToArray();

            return items;
        }

        public async Task<ContestModel> AddContestAsync(ContestModel newContest)
        {
            if (newContest.IsActive)
            {
                await MarkAllAsInactive();                
            }

            newContest.Id = Guid.NewGuid();
            _context.Contests.Add(newContest);

            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1 ? newContest : null;
        }

        public async Task<bool> MarkContestAsActive(Guid id)
        {
            var marked = await MarkAllAsInactive();
            if (!marked) return false;

            var item = await _context.Contests
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (item == null) return false;

            item.IsActive = true;

            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        private async Task<bool> MarkAllAsInactive()
        {
            _context.Contests
                .ToList()
                .ForEach( x => x.IsActive = false );

            var activeUpdate = await _context.SaveChangesAsync();

            return activeUpdate == 1;
        }
    }
}