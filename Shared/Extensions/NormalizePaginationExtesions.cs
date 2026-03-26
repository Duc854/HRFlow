using Shared.Constants;
using Shared.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Extensions
{
    public class NormalizePaginationExtesions
    {
        public static void NormalizePagination(PageRequest input)
        {
            if (input.PageSize > AppConstants.MaxPageSize) input.PageSize = AppConstants.MaxPageSize;
            if (input.PageSize < AppConstants.DefaultPageSize) input.PageSize = AppConstants.DefaultPageSize;
            if (input.PageIndex < AppConstants.DefaultPageIndex) input.PageIndex = AppConstants.DefaultPageIndex;
        }
    }
}
