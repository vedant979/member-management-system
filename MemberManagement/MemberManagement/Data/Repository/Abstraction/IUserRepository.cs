using Project5.DTOs;
using Project5.Models;

namespace Project5.Data.Repository.Abstraction
{
    public interface IUserRepository
    { 

        Task<bool> RegisterUserAsync(Member member);
        Task<bool> UpdateMember(Guid id, Member member);
        Task<bool> CheckUserInDb(string email);
        Task<bool> UpdateAddress(Guid id, Address address, Memberaddress memberAddress);
        Task<Member> GetUserByEmail(string email);
        Task<bool> AddAddress(Guid id, Address address, Memberaddress memberAddress);
        Task<bool> UpdatePassword(Guid id, string password);
        Task<bool> CheckUserSession(Guid memberId);
        Task<List<Member>> GetUserData();
        Task<bool> InvalidateToken(Guid id);
        Task<bool> AddContact(Contact contact);
        Task<bool> UpdateContact(Contact contact, int? phoneNumber);
        Task<bool> CheckUserWithContact(int? phoneNumber);
        Task<bool> AddUserSession(Guid memberId,string Token);
    }
}
