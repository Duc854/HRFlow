using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Dtos.DashboardDto;
using Shared.Wrappers;

namespace Business.Abstractions
{
    public interface IDashboardService
    {
        Task<ResponseDto<DashboardStatsDto>> GetDashboardStatsAsync();
    }
}
