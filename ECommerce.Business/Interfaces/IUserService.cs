using ECommerce.Business.DTOs.Auth.Responses;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.DTOs.Users.Requests;
using ECommerce.Business.DTOs.Users.Responses;
using ECommerce.Core.Specifications.Users;

namespace ECommerce.Business.Interfaces
{
    public interface IUserService
    {
        //Customers
        Task<UserDetailsResponse> GetDetailsAsync();
        Task<UserDetailsResponse> UpdateImageAsync(UploadImageRequest uploadImageRequest);
        Task DeleteImageAsync();
        Task<AuthResponse> UpdatePersonalInfoAsync(UpdateUserRequest updateUserRequest);
        Task<(AuthResponse, string, DateTime)> UpdatePasswordAsync(UpdatePasswordRequest updatePasswordRequest);

        //Admin
        Task<PagedResponse<AdminUserSummaryDto>> GetAllUsersAdminAsync(AdminUserSpecParams specParams);
        Task<AdminUserDetailsResponse> GetUserDetailsAdminAsync(string userId);
        Task<AdminUserDetailsResponse> UpdateUserRoleAdminAsync(string userId, string role);
        Task DeleteUserAdminAsync(string userId);


    }
}
