using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Project5.Data.Repository.Abstraction;
using Project5.DTOs;
using Project5.Models;
using System.ComponentModel;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Transactions;

namespace Project5.Data.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly MyDbContext dbContext;
        public UserRepository(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //ADDRESS
        public async Task<bool> UpdateAddress(Guid id, Address address, Memberaddress memberAddress)
        {
            using(var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int countAddress = await dbContext.Addresses.CountAsync();
                    if (countAddress==0)
                    {
                        return false;
                    }
                    else
                    {
                        var res = await dbContext.Addresses.FirstOrDefaultAsync(add => add.AddressId == id);
                        if (res != null)
                        {
                            res.Street = address.Street ?? res.Street;
                            res.PostalCode = address.PostalCode ?? res.PostalCode;
                            res.City = address.City ?? res.City;
                            res.State = address.State ?? res.State;
                            res.Country = address.Country ?? res.Country;
                        }
                    }
                    int countMemAddress = await dbContext.Memberaddresses.CountAsync();

                    if (countMemAddress == 0)
                    {
                        return false;
                    }
                    else
                    {
                        var res = await dbContext.Memberaddresses.FirstOrDefaultAsync(add => add.AddressId == id);
                        if (res != null)
                        {

                            res.AddressType = memberAddress.AddressType ?? res.AddressType;
                        }
                    }
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch(Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.InnerException);
                }
            }
            return true;
        }
        public async Task<bool> AddAddress(Guid memberId, Address address, Memberaddress memberAddress)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    string? addressType = memberAddress.AddressType;
                    if(addressType == "permanent" || addressType=="current")
                    {
                        var res = await dbContext.Memberaddresses
                     .Include(ma => ma.Address) // Include related Address entity
                     .FirstOrDefaultAsync(mem => mem.MemberId == memberId && mem.AddressType == addressType);
                        if (res != null)
                        {
                            res.Address.HouseNo = address.HouseNo;
                            res.Address.Street = address.Street;
                            res.Address.City = address.City;
                            res.Address.Country = address.Country;
                            res.Address.State = address.State;
                            res.Address.PostalCode = address.PostalCode;
                            res.AddressType = memberAddress.AddressType;
                            await dbContext.SaveChangesAsync();
                            await transaction.CommitAsync();
                            return true;
                        }
    
                    }
                    await dbContext.Addresses.AddAsync(address);
                    await dbContext.Memberaddresses.AddAsync(memberAddress);
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.InnerException);
                    return false;
                }
            }
            return true;
        }

        //USER
        public async Task<bool> RegisterUserAsync(Member member)
        {
            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {

                    await dbContext.Members.AddAsync(member);
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.InnerException);
                    return false;
                }
            }

        }
        public async Task<Member> GetUserByEmail(string email)
        {
            try
            {
                var user = await dbContext.Members.FirstOrDefaultAsync(m => m.Email == email);

                if (user == null)
                {
                    return null;
                }
                else
                {
                    return user;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
            return null;
        }
        public async Task<bool> UpdateMember(Guid id, Member member)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var res = await dbContext.Members.FirstOrDefaultAsync(mem => mem.MemberId == member.MemberId);
                if (res == null)
                {
                    Console.WriteLine($"user not found");
                    return false;
                }
                res.FirstName = member.FirstName ?? res.FirstName;
                res.LastName = member.LastName ?? res.LastName;
                res.MiddleName = member.MiddleName ?? res.MiddleName;
                res.Dob = member.Dob ?? res.Dob;
                res.Gender = member.Gender ?? res.Gender;
                //res.Email = member.Email ?? res.Email;
                if (String.IsNullOrEmpty(member.Email))
                {
                    member.Email = res.Email;
                }
                else
                {

                    var existingMember = await dbContext.Members
                        .FirstOrDefaultAsync(mem => mem.Email == member.Email && mem.MemberId != id);
                    if (existingMember != null)
                    {
                        Console.WriteLine("Duplicate email found.");
                        return false;
                    }
                    else
                    {
                        res.Email = member.Email;
                    }
                }
                var res1 = await dbContext.Members.FirstOrDefaultAsync(mem => mem.MemberId == member.MemberId);
                if (res1.MemberId == id)
                {
                    Console.WriteLine("yes" + " " + res.MemberId + " " + member.MemberId);
                }
                else
                {
                    return false;
                }
                member.HashPassword = res.HashPassword;
                //dbContext.Entry(dbContext.Members).CurrentValues.SetValues(member);
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                await transaction.RollbackAsync();
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }
        public async Task<bool> CheckUserInDb(string email)
        {
            Member res = await dbContext.Members.FirstOrDefaultAsync(mem => mem.Email == email);

            if (res!=null)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> UpdatePassword(Guid id, string password)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var user = await dbContext.Members.FirstOrDefaultAsync(mem => mem.MemberId == id);
                    user.HashPassword = password;
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException);
                    return false;
                }
            }
            return true;
        }
        public async Task<List<Member>> GetUserData()
        {
            try
            {
                var userDetails =await dbContext.Members
                    .Include(member => member.Memberaddresses)
                    .ThenInclude(member=>member.Address)
                    .Include(member => member.Contacts).ToListAsync();
                return userDetails;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return null;
            }
        }

        //SESSION
        public async Task<bool> AddUserSession(Guid memberId, string token)
        {
            try
            {
                Guid sessionId = Guid.NewGuid();
                Sessionlog session = new()
                {
                    Token = token,
                    SessionlogId = sessionId,
                    MemberId = memberId,
                    IsValid = "true"
                };
                await dbContext.Sessionlogs.AddAsync(session);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }
        public async Task<bool> InvalidateToken(Guid id)
        {
            try
            {
                var resp = await dbContext.Sessionlogs.FirstOrDefaultAsync(session=>session.IsValid=="true" && session.MemberId==id);
                if (resp == null)
                {
                    return false;
                }
                resp.IsValid = "false";
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }
        public async Task<bool> CheckUserSession(Guid memberId)
        {
            try
            {
                var user = await dbContext.Sessionlogs.Where(session => session.MemberId == memberId && session.IsValid == "true").FirstOrDefaultAsync();
                if (user == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }

        //Contact 
        public async Task<bool> AddContact(Contact contact)
        {
            try
            { 
                await dbContext.Contacts.AddAsync(contact);

                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }
        public async Task<bool> UpdateContact(Contact contact, int? phoneNumber)
        {
            try
            {
                var res = await dbContext.Contacts.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
                if (res == null)
                {
                    return false;
                }
                res.PhoneNumber = contact.PhoneNumber;
                res.ContactType = contact.ContactType ?? res.ContactType;
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }
        public async Task<bool> CheckUserWithContact(int? phoneNumber)
        {
            try
            {
                var res = await dbContext.Contacts.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
                if (res==null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return false;
            }

        }
    }
}
