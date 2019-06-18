using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using WebApplicationContato.DAL;
using WebApplicationContato.Models;

namespace WebApplicationContato.Controllers
{


    public class PlanetasController : ApiController
    {

        private PlanetaDbContext db = new PlanetaDbContext();


        // POST: api/Planetas
        [ResponseType(typeof(PlanetaIncluir))]
        public IHttpActionResult PostPlaneta(PlanetaIncluir planetaIncluir)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Planeta planeta = new Planeta();

            planeta.id = db.planetas.Max(p => p.id) + 1;
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

            db.planetas.Add(planeta);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = planeta.id }, planetaIncluir);
        }
            // GET: api/Planetas
            public IQueryable<Planeta> Getplanetas()
        {
            return db.planetas;
        }

        // GET: api/Planetas/5
        [ResponseType(typeof(Planeta))]
        public IHttpActionResult GetPlaneta(int id)
        {
            Planeta planeta = db.planetas.Find(id);
            if (planeta == null)
            {
                return NotFound();
            }

            return Ok(planeta);
        }

        // GET: api/Planetas/5
        [ResponseType(typeof(Planeta))]
        public IHttpActionResult GetPlaneta(string nome)
        {
            Planeta planeta = db.planetas.FirstOrDefault(p => p.nome == nome);
            if (planeta == null)
            {
                return NotFound();
            }

            return Ok(planeta);
        }

        // DELETE: api/Planetas/5
        [ResponseType(typeof(Planeta))]
        public IHttpActionResult DeletePlaneta(int id)
        {
            Planeta planeta = db.planetas.Find(id);
            if (planeta == null)
            {
                return NotFound();
            }

            db.planetas.Remove(planeta);
            db.SaveChanges();

            return Ok(planeta);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PlanetaExists(int id)
        {
            return db.planetas.Count(e => e.id == id) > 0;
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
