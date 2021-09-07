using Hospital.Data;
using Hospital.Models;
using Hospital.Services.Interfaces;
using System.Linq;

namespace Hospital.Services.Implementations
{
    public class AddressService : IAddressService
    {
        readonly Db db;
        public AddressService(Db db)
        {
            this.db = db;
        }
        public void AddAddress(Address address)
        {
            db.Addresses.Add(address);
            db.SaveChanges();
        }

        public Address AddAddress(string street, string locality, string region)
        {
            Address address;
            if (IsExist(street, locality, region))
            {
                address = GetAddress(street, locality, region);
            }
            else
            {
                address = new Address
                {
                    Street = street,
                    Locality = locality,
                    Region = region
                };
                db.Addresses.Add(address);
                db.SaveChanges();
            }
            return address;
        }

        public Address GetAddress(string street, string locality, string region)
        {
            return db.Addresses.SingleOrDefault(a => a.Street == street
                                           && a.Locality == locality
                                           && a.Region == region);
        }

        public bool IsExist(string street, string locality, string region)
        {
            return db.Addresses.Any(a => a.Street == street 
                                        && a.Locality == locality 
                                        && a.Region == region);
        }
    }
}
