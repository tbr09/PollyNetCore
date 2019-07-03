using Client.DTO;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Contracts
{
    public interface ICustomerService
    {
        [AllowAnyStatusCode]
        [Get("customer/list")]
        Task<IEnumerable<CustomerDTO>> List();

        [AllowAnyStatusCode]
        [Get("customer/{id}")]
        Task<CustomerDTO> Get([Path] int id);

        [AllowAnyStatusCode]
        [Get("customer/{term}")]
        Task<IEnumerable<CustomerDTO>> Find([Path] string term);
    }
}
