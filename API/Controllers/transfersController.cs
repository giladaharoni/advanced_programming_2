using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/transfer")]
    [ApiController]
    public class transfersController : ControllerBase
    {
        private static List<transfer> _transfers = new List<transfer>();


        [HttpPost]
        public void post(transfer transfer)
        {
            _transfers.Add(transfer);

        }

    }
}
