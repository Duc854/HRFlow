using Data.Abstractions.Repositories;
using Data.Context;
using Data.Entities.HR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Persistence.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly ApplicationDbContext _context;
        public PositionRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Position>> GetAllActiveAsync()
        {
            return await _context.Positions
                .Where(p => !p.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
