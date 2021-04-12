using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == id).ToList();
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(e => e.Name == name);
            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(celestialObjects.ToList());
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject model)
        {
            _context.CelestialObjects.Add(model);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = model.Id }, model);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject model)
        {
            var current = _context.CelestialObjects.Find(id);

            if (current == null)
            {
                return NotFound();
            }

            current.Name = model.Name;
            current.OrbitalPeriod = model.OrbitalPeriod;
            current.OrbitedObjectId = model.OrbitedObjectId;
            _context.CelestialObjects.Update(current);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var current = _context.CelestialObjects.Find(id);
            if (current == null)
            {
                return NotFound();
            }
            current.Name = name;
            _context.CelestialObjects.Update(current);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var current = _context.CelestialObjects.Where(co => co.Id == id || co.OrbitedObjectId == id).ToList();
            if (current.Count == 0)
            {
                return NotFound();
            }
            _context.CelestialObjects.RemoveRange(current);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
