using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project5.Data.Repository.Abstraction;
using Project5.DTOs;
using Project5.DTOs.Project5.DTOs;
using Project5.Helper;
using Project5.Models;
using Project5.Services.Abstraction;
using System.Net;
using System.Security.Claims;


namespace Project5.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly PasswordHasher<Object> passwordHasher;
        private readonly JwtTokenGenerator jwtTokenGenerator;
        private readonly IHttpContextAccessor httpContextAccessor;
        private SByte SessionStartTime {  get; set; }
        //private bool check = false;
        public UserService(IUserRepository userRepository, PasswordHasher<object> passwordHasher, JwtTokenGenerator tokenGenerator, IHttpContextAccessor httpContextAccessor)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
            this.jwtTokenGenerator = tokenGenerator;
            this.httpContextAccessor = httpContextAccessor;

        }
        public async Task<string> LoginUserAsync(LoginUserDTO loginUserDTO)
        {
            try
            {
                Member user =await userRepository.GetUserByEmail(loginUserDTO.Email);
             
                if (user == null)
                {
                    throw new UnauthorizedAccessException("Invalid Credential");
                }
                else
                {
                    //Initializing session time
                    SessionStartTime = Convert.ToSByte(DateTime.Now.Hour);

                    var result = passwordHasher.VerifyHashedPassword(null,user.HashPassword, loginUserDTO.Password);
                    if(result!= PasswordVerificationResult.Success)
                    {
                        throw new UnauthorizedAccessException("Invalid credentials");
                    }
                }
                
                string token = await jwtTokenGenerator.GenerateToken(user.MemberId,"user", user.Email);
                var resp = await userRepository.AddUserSession(user.MemberId,token);
                if (!resp)
                {
                    return "Please try again";
                }
                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
            return null;
        }
        public async Task<bool> RegisterUserAsync(RegisterUserDTO registerUser)
        {
            try
            {
                //Console.WriteLine(registerUser.Email);
                var check = await userRepository.CheckUserInDb(registerUser.Email);
                
                if (check)
                {
                    throw new Exception("User already registered");
                }


                
                //Adding Member details
                Guid memberId = Guid.NewGuid();
                Member member = new Member();
                member.MemberId = memberId;
                member.FirstName = registerUser.FirstName;
                member.LastName = registerUser.LastName;
                member.Email = registerUser.Email;
                member.HashPassword = passwordHasher.HashPassword(null, registerUser.Password);



                bool result =await userRepository.RegisterUserAsync(member);
                if (!result)
                {
                    throw new Exception("User not registered.");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return false;
            }

        }
        public async Task<List<UserResponseDTO>> GetUserData()
        {
            try
            {
                var members = await userRepository.GetUserData();
                //UserResponseDTO userResponse = new UserResponseDTO();
                string userEmail = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
                var userDTOs = members.Where(member => member.Email == userEmail).Select(member => new UserResponseDTO
                {
                    FirstName = member.FirstName,
                    MiddleName = member.MiddleName,
                    LastName = member.LastName,
                    DateOfBirth = member.Dob,
                    Gender = member.Gender,
                    Email = member.Email,
                    Contacts = member.Contacts.Select(x => new ContactDTO
                    {
                        PhoneNumber = x.PhoneNumber,
                        ContactType = x.ContactType
                    }
                       ).ToList(),
                    Addresses = member.Memberaddresses.Select(x => new DTOs.Project5.DTOs.AddressDTO
                    {
                        HouseNo = x.Address.HouseNo,
                        Street = x.Address.Street,
                        PinCode = x.Address.PostalCode,
                        City = x.Address.City,
                        State = x.Address.State,
                        Country = x.Address.Country,
                        AddressType = x.AddressType
                    }).ToList()
                }).ToList();
                    return userDTOs;
                }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
            return null;
        }
        public async Task<Member> updateUserDetailsAsync(UpdateMemberDTO updateMember)
        {
            try
            {
                Guid memId =Guid.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                //Updating Member/User Data
                Member member = new Member();
                member.MemberId = memId;
                member.FirstName = updateMember.FirstName ?? member.FirstName;
                member.LastName = updateMember.LastName ?? member.LastName;
                member.MiddleName = updateMember.MiddleName ?? member.MiddleName;
                member.Dob = updateMember.DateOfBirth ?? member.Dob;
                member.Gender = updateMember.Gender ?? member.Gender;
                
                bool response = await userRepository.UpdateMember(memId,member);

                if (!response)
                {
                    throw new Exception("Update failed!");
                }
                return member;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return null;

            }
        }
        public async Task<Address> updateAddressDetailsAsync(Guid id, UpdateAddressDTO updateAddress)
        {
            try
            {
                //Updating Address table
                Address address = new Address();
                address.AddressId = id;
                address.PostalCode = updateAddress.PinCode;
                address.Street = updateAddress.Street;
                address.City = updateAddress.City;
                address.State = updateAddress.State;
                address.Country = updateAddress.Country;


                //Updating MemberAddress junction table
                Guid memId = Guid.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                Guid memberAddressId = Guid.NewGuid();

                Memberaddress memberAddress = new Memberaddress();
                memberAddress.MemberAddressId = memberAddressId;
                memberAddress.AddressType = updateAddress.AddressType;
                memberAddress.MemberId = memId;
                memberAddress.AddressId = id;


                var response =await userRepository.UpdateAddress(id,address, memberAddress);
                if (!response)
                {
                    throw new Exception("Update failed!");
                }
                return address;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return null;
        }
        public async Task<Address?> addAddressDetailsAsync( DTOs.AddressDTO addAddress)
        {
            try
            {
                //Updating Address table
                Guid addressId = Guid.NewGuid();
                Address address = new Address();
                address.AddressId = addressId;
                address.HouseNo = addAddress.Houseno;
                address.PostalCode = addAddress.PinCode;
                address.Street = addAddress.Street;
                address.City = addAddress.City;
                address.State = addAddress.State;
                address.Country = addAddress.Country;

                //Updating MemberAddress junction table
                Guid memId = Guid.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                Guid memberAddressId = Guid.NewGuid();
                Memberaddress memberAddress = new Memberaddress();
                memberAddress.MemberAddressId = memberAddressId;
                memberAddress.AddressType = addAddress.AddressType;
                memberAddress.MemberId = memId;
                memberAddress.AddressId = addressId;

                var response = await userRepository.AddAddress(memId, address, memberAddress);
                if (!response)
                {
                    throw new Exception("Update failed!");
                }
                return address;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return null;
        }
        public async Task<bool> RecoverPasswordAsync(RecoverPasswordDTO recoverPassword)
        {
            try
            {
                var user = await userRepository.GetUserByEmail(recoverPassword.Email);
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
        public async Task<bool> ResetPasswordAsync(ChangeUserCredentialDTO changeUserCredential)
        {
            try
            {
                string userEmail = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
                Guid id = Guid.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                Member user = await userRepository.GetUserByEmail(userEmail);
                
                var checkPass = passwordHasher.VerifyHashedPassword(null,user.HashPassword, changeUserCredential.OldPassword);
                
                if (checkPass != PasswordVerificationResult.Success)
                {
                    return false;
                }
                //Comparing if new pasword is same as old password
                var checkNewPassWithOldPass = passwordHasher.VerifyHashedPassword(null,user.HashPassword, changeUserCredential.NewPassword);

                if (checkNewPassWithOldPass == PasswordVerificationResult.Success)
                { 
                    return false;
                }
                var HashPassword = passwordHasher.HashPassword(null, changeUserCredential.NewPassword);
                var res = await userRepository.UpdatePassword(id, HashPassword);
                if (!res)
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

        public async Task<bool> AddContactAsync(AddContactDTO addContact)
        {
            try
            {
                var memberId = Guid.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                //var res = await userRepository
                Contact contact = new Contact();
                contact.ContactId = Guid.NewGuid();
                contact.MemberId = memberId;
                contact.PhoneNumber = addContact.PhoneNumber;
                contact.ContactType = addContact.ContactType;
                var res = await userRepository.AddContact(contact);
                if (!res)
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
        public async Task<bool> UpdateContactAsync(UpdateContactDTO updateContact)
        {
            try
            {
                var memberId = Guid.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                Contact contact = new Contact();
                contact.ContactId = Guid.NewGuid();
                contact.MemberId = memberId;
                contact.PhoneNumber = updateContact.NewPhoneNumber;
                contact.ContactType = updateContact.ContactType;
                var repoRes = await userRepository.UpdateContact(contact, updateContact.OldPhoneNumber);
                if (!repoRes)
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
        public async Task<bool> IsUserSessionValid(Guid userId)
        {
            try
            {
                var user = await userRepository.CheckUserSession(userId);
                if (user)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }
        public async Task<bool> LogoutAsync()
        {
            try
            {
                Guid id = Guid.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var resp = await userRepository.InvalidateToken(id);
                if (!resp)
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

        public async Task<bool> CheckUserSessionAsync(Guid id)
        {
            try
            {
                var resp = await userRepository.CheckUserSession(id);
                if (resp)
                {
                    return true;
                }
                return false;
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
