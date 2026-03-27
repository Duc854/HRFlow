using Business.Persistence;
using Data.Context;
using Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Abstractions;
using Business.InternalServices;
using Application.Services;
using Business.Services;

namespace Infrastructure.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //Db context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SQLServer"),
                b => b.MigrationsAssembly("Data")
                )
            );
            //Service
            services.AddScoped<IPasswordHasher,BcryptPasswordHasher>();
            services.AddScoped<ITokenProvider, JwtTokenProvider>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ICommonService,CommonService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IContractService, ContractService>();

            //UoW
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //Other
            return services;
        }
    }
}
