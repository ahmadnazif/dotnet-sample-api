using SampleApi.Models;

namespace SampleApi.Services;

public interface IDbRepo
{
    Task<int> CountAllContactAsync();
    Task<Contact> GetContactAsync(int id);
    Task<List<Contact>> ListAllContactAsync();
    Task<ResponseBase> AddContactAsync(ContactBase c);
    Task<ResponseBase> EditContactAsync(int id, ContactBase c);
    Task<ResponseBase> DeleteContactAsync(int id);
}
