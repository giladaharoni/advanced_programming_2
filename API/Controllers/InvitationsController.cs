using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class InvitationsController : ControllerBase
    {
        private static List<Invitation> _invitations = new List<Invitation>();


       [HttpPost]
        public void post(Invitation invitation)
        {
            _invitations.Add(invitation);

        }
    }
}
