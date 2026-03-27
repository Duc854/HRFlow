using Business.Dtos.CommonDtos;
using Shared.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstractions
{
    public interface ICommonService
    {
        Task<ResponseDto<IEnumerable<CategoryDto>>> GetDepartmentsAsync();
        Task<ResponseDto<IEnumerable<CategoryDto>>> GetPositionsAsync();
    }
}
