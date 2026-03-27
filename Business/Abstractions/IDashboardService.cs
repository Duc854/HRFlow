using Business.Dtos.DashboardDto;
using Shared.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstractions
{
    public interface IDashboardService
    {
        Task<ResponseDto<DashboardStatsDto>> GetDashboardStatsAsync();
    }
}
