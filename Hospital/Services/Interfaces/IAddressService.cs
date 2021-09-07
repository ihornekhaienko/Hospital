using Hospital.Models;

namespace Hospital.Services.Interfaces
{
    interface IAddressService
    {
        public Address AddAddress(string street, string locality, string region);
        public Address GetAddress(string street, string locality, string region);
        public bool IsExist(string street, string locality, string region);
    }
}
