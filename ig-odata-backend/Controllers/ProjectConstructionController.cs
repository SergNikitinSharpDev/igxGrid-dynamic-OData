using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectConstructionController : ControllerBase
    {
        private readonly AppContext _context;

        public ProjectConstructionController(AppContext context)
        {
            _context = context;
        }

        [HttpGet]
        [EnableQuery()]
        public ActionResult<IQueryable<ProjectConstructionEntity>> GetData(string text)
        {
            return Ok(_context.ProjectConstructionItems
                .Where(i => string.IsNullOrWhiteSpace(text) 
                    || i.address.Contains(text) 
                    || i.business_id.Contains(text) 
                    || i.name.Contains(text) 
                    || i.latitude.Contains(text) 
                    || i.longitude.Contains(text))
                .AsQueryable());
        }
    }
}