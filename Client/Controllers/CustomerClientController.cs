using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client.HttpClients;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class CustomerClientController : Controller
    {
        private readonly CustomerClient customerClient;

        public CustomerClientController(CustomerClient customerClient)
        {
            this.customerClient = customerClient;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}