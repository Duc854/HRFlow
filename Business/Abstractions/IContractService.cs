using Business.Dtos.ContractDtos;
using Shared.Models;
using Shared.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstractions
{
    public interface IContractService
    {
        Task<ResponseDto<ContractDetailDto>> GetContractDetailAsync(UserIdentity identity, int id);
        Task<ResponseDto<IEnumerable<ContractDetailDto>>> SearchContractsAsync(UserIdentity identity, ContractFilterDto filter);
        Task<ResponseDto<int>> CreateContractAsync(UserIdentity identity, CreateContractDto dto);
        Task<ResponseDto<bool>> UpdateContractAsync(UserIdentity identity, int id, UpdateContractDto dto);
        Task<ResponseDto<bool>> TerminateContractAsync(UserIdentity identity, int id, DateTime? terminateDate);
    }
}
