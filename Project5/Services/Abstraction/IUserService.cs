using Project5.Data.Repository.Abstraction;
using Project5.DTOs;
using Project5.DTOs.Project5.DTOs;
using Project5.Models;

namespace Project5.Services.Abstraction
{
    public interface IUserService
    {

        Task<bool> UpdateContactAsync(UpdateContactDTO updateContact);
        Task<bool> AddContactAsync(AddContactDTO updateContact);
        Task<bool> RegisterUserAsync(RegisterUserDTO registerUser);
        Task<string> LoginUserAsync(LoginUserDTO loginUserDto);
        Task<List<UserResponseDTO>> GetUserData();
        Task<bool> IsUserSessionValid(Guid userId);
        Task<Member> updateUserDetailsAsync(UpdateMemberDTO updateMember);
        Task<Address> updateAddressDetailsAsync(Guid id, UpdateAddressDTO updateAddress);
        Task<Address> addAddressDetailsAsync(DTOs.AddressDTO addAddress);
        Task<bool> RecoverPasswordAsync(RecoverPasswordDTO recoverPassword);
        Task<bool> ResetPasswordAsync(ChangeUserCredentialDTO changeUserCredential);
        Task<bool> LogoutAsync();
        Task<bool> CheckUserSessionAsync(Guid id);
    }
}
