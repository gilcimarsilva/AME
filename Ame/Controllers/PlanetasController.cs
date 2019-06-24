using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ame.Models;
using System.Net.Http;
using System.Web.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanetasController : ControllerBase
    {

        private readonly PlanetaDbContext _context;

        public PlanetasController(PlanetaDbContext context)
        {
            _context = context;
            
        }


        // POST: api/Planetas
        // POST api/<controller>
        [HttpPost]        
        public ActionResult Post(PlanetaIncluir planetaIncluir)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Planeta planeta = new Planeta();
            
            planeta.nome = planetaIncluir.nome;
            planeta.clima = planetaIncluir.clima;
            planeta.terreno = planetaIncluir.terreno;

            List<ResultsPlanetas> planetasWebApi = BuscarDadosWeb();
            ResultsPlanetas planetaWebApi = new ResultsPlanetas();

            planetaWebApi = planetasWebApi.FindLast(p => p.name == planeta.nome);

            if (planetaWebApi != null)
            {
                planeta.qtdApareceuEmFilme = planetaWebApi.films.Count();
            }
            else
            {
                planeta.qtdApareceuEmFilme = 0;
            }

            _context.planetas.Add(planeta);
            _context.SaveChanges();

            return Ok(planeta);
        }


        //// POST api/<controller>
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        // GET: api/Planetas        
        public IQueryable<Planeta> Getplanetas()
        {
            if (_context.planetas.Count() > 0)
                return _context.planetas;
            else
                return null;
        }

        // GET: api/Planetas/5     
        [HttpGet("GetPlanetaPorId")]
        public ActionResult<Planeta> GetPlanetaPorId(int id)
        {
            Planeta planeta = _context.planetas.Find(id);
            if (planeta != null)
            {
                return Ok(planeta);
            }
            return NotFound(); ;
            
        }

        // GET: api/Planetas/5   
        [HttpGet("GetPlanetaPorNome")]
        public ActionResult<Planeta> GetPlanetaPorNome(string nome)
        {
            Planeta planeta = _context.planetas.FirstOrDefault(p => p.nome == nome);
            if (planeta == null)
            {
                return Ok(planeta); ;
            }

            return NotFound(); ;
        }



        // DELETE: api/Planetas/5
        [HttpDelete("DeletePlaneta")]
        public ActionResult DeletePlaneta(int id)
        {

            Planeta planeta = _context.planetas.Find(id);
            if (planeta == null)
            {
                return NotFound();
            }

            _context.planetas.Remove(planeta);
            _context.SaveChanges();

            return Ok(planeta);

        }

        private bool PlanetaExists(int id)
        {
            return _context.planetas.Count(e => e.id == id) > 0;
        }


        private List<ResultsPlanetas> BuscarDadosWeb()
        {

            Results result = new Results();
            List<ResultsPlanetas> planetasWebApi = new List<ResultsPlanetas>();

            do
            {
                result = BuscarDadosWebPlanetas(result.next);
                foreach (ResultsPlanetas resultado in result.results)
                {
                    planetasWebApi.Add(resultado);
                }

            } while (result.next != null);

            return planetasWebApi;

        }

        private Results BuscarDadosWebPlanetas(string next)
        {
            string url = "";

            if (next == null)
                url = "https://swapi.co/api/planets/";
            else
                url = next;

            string contentType = "application/json; charset=utf-8";
            System.Net.WebRequest webRequest;
            webRequest = System.Net.WebRequest.Create(url);
            webRequest.ContentType = contentType;

            var response = (System.Net.HttpWebResponse)webRequest.GetResponse();
            string resultadoJson = null;
            using (var streamReader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                resultadoJson = streamReader.ReadToEnd();
            }

            Results results = new Results();
            results = Deserialize<Results>(resultadoJson);
            return results;

        }


        private T Deserialize<T>(string stringJson)
        {
            T objeto = Activator.CreateInstance<T>();

            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(System.Text.Encoding.Unicode.GetBytes(stringJson));

            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(objeto.GetType());

            objeto = (T)serializer.ReadObject(memoryStream);

            memoryStream.Close();

            return objeto;
        }
    }
}
