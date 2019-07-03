using CustomerService.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace CustomerService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerDTO[] customers;

        public CustomerController()
        {
            customers = new CustomerDTO[]
            {
                new CustomerDTO
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Smith",
                    BirthDate = DateTime.UtcNow,
                    Address = "Chicago St. Avenue 64/2"
                },
                new CustomerDTO
                {

                    Id = 2,
                    FirstName = "Patricia",
                    LastName = "Trump",
                    BirthDate = DateTime.UtcNow,
                    Address = "Washington DC. Holy St. 53/115"
                },
                new CustomerDTO
                {

                    Id = 3,
                    FirstName = "Frank",
                    LastName = "Sinatra",
                    BirthDate = DateTime.UtcNow,
                    Address = "Los Angeles CA, 44 st. 25/141"
                },
            };
        }

        [HttpGet("list")]
        public IEnumerable<CustomerDTO> List()
        {
            throw new TimeoutException();
        }

        [HttpGet("{id}")]
        public CustomerDTO Get(int id) 
            => customers.Where(c => c.Id == id).FirstOrDefault();

        [HttpGet("find/{term}")]
        public IEnumerable<CustomerDTO> Find(string term)
            => customers.Where(c => c.FirstName.Contains(term) || c.LastName.Contains(term)).ToList();
    }
}
