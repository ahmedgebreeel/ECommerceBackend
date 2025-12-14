using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.DTOs.Users;
using ECommerce.Core.Specifications;

namespace ECommerce.Business.Interfaces
{
    public interface IUserService
    {
        Task<PagedResponseDto<UserManagementDto>> GetAllUsersAsync(UserSpecParams specParams);
        //Admin
        //1. userId null => get current user details(Admin)
        //2. userId not null => get user details where id = userId

        //Not Admin (Customer)
        // userId null or not => get current user details(Customer)
        Task<UserDetailsDto> GetByIdAsync(string? userId = null);

    }
}
