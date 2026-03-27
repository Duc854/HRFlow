using Business.Abstractions;
using Business.Dtos.CommonDtos;
using Data.Abstractions;
using Shared.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class CommonService : ICommonService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CommonService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<ResponseDto<IEnumerable<CategoryDto>>> GetDepartmentsAsync()
        {
            try
            {
                var depts = await _unitOfWork.Departments.GetAllActiveAsync();
                var result = depts.Select(d => new CategoryDto { Id = d.Id, Name = d.Name });
                return ResponseDto<IEnumerable<CategoryDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResponseDto<IEnumerable<CategoryDto>>.FailResult("500", ex.Message);
            }
        }

        public async Task<ResponseDto<IEnumerable<CategoryDto>>> GetPositionsAsync()
        {
            try
            {
                var positions = await _unitOfWork.Positions.GetAllActiveAsync();
                var result = positions.Select(p => new CategoryDto { Id = p.Id, Name = p.Name });
                return ResponseDto<IEnumerable<CategoryDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResponseDto<IEnumerable<CategoryDto>>.FailResult("500", ex.Message);
            }
        }
    }
}
