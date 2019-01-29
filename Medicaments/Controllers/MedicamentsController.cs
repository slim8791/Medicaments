using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using HtmlAgilityPack;
using Medicaments.Models;

namespace Medicaments.Controllers
{
    public class MedicamentsController : ApiController
    {
        private MedicamentsContext db = new MedicamentsContext();

        private string a = "";
        // GET: api/Medicaments
        public IQueryable<Medicament> GetMedicaments()
        {
            return db.Medicaments;
        }

        // GET: api/Medicaments/5
        [ResponseType(typeof(Medicament))]
        public IHttpActionResult GetMedicament(int id)
        {
            Medicament medicament = db.Medicaments.Find(id);
            if (medicament == null)
            {
                return NotFound();
            }

            return Ok(medicament);
        }

        // PUT: api/Medicaments/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMedicament(int id, Medicament medicament)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != medicament.MedicamentId)
            {
                return BadRequest();
            }

            db.Entry(medicament).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicamentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Medicaments
        [ResponseType(typeof(Medicament))]
        public IHttpActionResult PostMedicament(Medicament medicament)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Medicaments.Add(medicament);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = medicament.MedicamentId }, medicament);
        }

        // DELETE: api/Medicaments/5
        [ResponseType(typeof(Medicament))]
        public IHttpActionResult DeleteMedicament(int id)
        {
            Medicament medicament = db.Medicaments.Find(id);
            if (medicament == null)
            {
                return NotFound();
            }

            db.Medicaments.Remove(medicament);
            db.SaveChanges();

            return Ok(medicament);
        }
        [Route("api/medicaments/delete/all")]
        [HttpDelete]
        [ResponseType(typeof(Medicament))]
        public IHttpActionResult DeleteMedicament()
        {


            db.Medicaments.RemoveRange(db.Medicaments);
            db.SaveChanges();

            return Ok();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MedicamentExists(int id)
        {
            return db.Medicaments.Count(e => e.MedicamentId == id) > 0;
        }
        [HttpGet]
        [Route("api/Download")]
        public async Task<IHttpActionResult> Index()
        {
            try
            {
                await Parsing("http://www.phct.com.tn/index.php/medicaments-humain?limitstart22=0");
                // return View();
                return Ok("a");

            }
            catch (Exception e)
            {
                return BadRequest(a);

            }

        }
        public async Task/*<List<string>>*/ Parsing(string url)
        {
           
            HttpClient http = new HttpClient();
            var response = await http.GetByteArrayAsync(url);
            String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
            source = WebUtility.HtmlDecode(source);
            HtmlDocument resultat = new HtmlDocument();
            resultat.LoadHtml(source);
            List<HtmlNode> toftitle = resultat.DocumentNode.Descendants().Where
            (x => (x.Name == "span" && x.Attributes["class"] != null &&
                   x.Attributes["class"].Value.Contains("add-on"))).ToList();
            HtmlNode node = toftitle.First();
            var pos = -1;
            foreach (var item in toftitle)
            {
                a = "boucle 1 ";
                if (item.InnerHtml.Contains("sur"))
                {
                    pos = item.InnerHtml.IndexOf("sur");
                    node = item;
                    break;
                }

            }


            string number = "";
            var total = node.InnerHtml.Substring(pos + 4);
            for (int i = 0; i < total.Length; i++)
            {
                a = "boucle 2 ";

                if (Char.IsDigit(total[i]))
                    number += total[i];
                else
                    break;
            }

            int final = int.Parse(number);
            for (int i =/* 0*/0; i <= final; i += 4)
            {
                a = "boucle 3 ";

                HttpClient ht = new HttpClient();
                var rep = await ht.GetByteArrayAsync("http://www.phct.com.tn/index.php/medicaments-humain?limitstart22=" + i);

                String sc = Encoding.GetEncoding("utf-8").GetString(rep, 0, rep.Length - 1);
                sc = WebUtility.HtmlDecode(sc);
                HtmlDocument rst = new HtmlDocument();
                rst.LoadHtml(sc);
                List<HtmlNode> listMed = rst.DocumentNode.Descendants().Where
                (x => (x.Name == "div" && x.Attributes["class"] != null &&
                       x.Attributes["class"].Value.Contains("modal-body"))).ToList();
                int cmpt = 0;
                List<HtmlNode> listName = rst.DocumentNode.Descendants().Where
                (x => (x.Name == "h2" && x.Attributes["class"] != null &&
                       x.Attributes["class"].Value.Contains("header-card"))).ToList();
                foreach (var item in listMed)
                {
                    a = "boucle 4 ";

                    //    HtmlDocument htmlDoc = new HtmlDocument();
                    //    string innerHtml =
                    //        "<html prefix='og: http://ogp.me/ns#' lang='fr-fr' dir='ltr'><head><base href = 'http://www.phct.com.tn/index.php/medicaments-humain'/><meta http-equiv = 'content-type' content = 'text/html; charset=utf-8' /></head><body> " +
                    //        item.InnerHtml + "</body></html>";
                    //    htmlDoc.Load(innerHtml);
                    //    var obj = htmlDoc.DocumentNode.Descendants().Where
                    //    (x => (x.Name == "div" && x.Attributes["class"] != null &&
                    //           x.Attributes["class"].Value.Contains("modal-body") )).ToList();
                    //    var aa = obj;
                    HtmlDocument htmlD = new HtmlDocument();
                    htmlD.LoadHtml(item.InnerHtml);
                    var ls = htmlD.DocumentNode.Descendants().Where
                    (x => (x.Name == "div" && x.Attributes["class"] != null &&
                           x.Attributes["class"].Value.Contains("col-md-6") &&
                           x.Attributes["class"].Value.Contains("col-lg-8"))).ToList();
                    
                        a = "boucle 5 ";

                    try
                    {
                        var aMM = "";
                        try
                        {
                            aMM = ls[ls.Count - 4].InnerHtml.Substring(/*"<b>", ""*/2);
                            aMM = ls[ls.Count - 4].InnerHtml.Replace("</b>", "");

                        }
                        catch (Exception e)
                        {


                        }

                        var codeProduit = "";
                        try
                        {
                            codeProduit = ls[0].InnerHtml.Replace("<b>", "");
                            codeProduit = ls[0].InnerHtml.Replace("</b>", "");

                        }
                        catch (Exception e)
                        {

                        }

                        a = a + codeProduit;
                        var dCi1 = "";

                        try
                        {
                            dCi1 = ls[2].InnerHtml.Replace("<b>", "");
                            dCi1 = ls[2].InnerHtml.Replace("</b>", "");

                        }
                        catch (Exception e)
                        {


                        }

                        var designation = "";
                        try
                        {
                            designation = ls[ls.Count - 2].InnerHtml.Replace("<b>", "");
                            designation = ls[ls.Count - 2].InnerHtml.Replace("</b>", "");

                        }
                        catch (Exception e)
                        {

                        }

                        var fournisseur = ls[1].InnerHtml.Replace("<b>", "");
                        fournisseur = ls[1].InnerHtml.Replace("</b>", "");

                        var prix = "";
                        try
                        {
                            prix = ls[ls.Count-1].InnerHtml.Replace("<b>", "");
                            prix = ls[ls.Count - 1].InnerHtml.Replace("</b>", "");
                            if (!prix.Contains("DT"))
                                throw new Exception();
                        }
                        catch (Exception e)
                        {
                            prix = "1";

                        }


                        var dateAmm = ls[4].InnerHtml.Replace("<b>", "");
                        dateAmm = ls[ls.Count - 3].InnerHtml.Replace("</b>", "");
                        fournisseur = fournisseur.Substring(3);
                        aMM = aMM.Substring(3);
                        codeProduit = codeProduit.Substring(3);
                        dCi1 = dCi1.Substring(3);
                        designation = designation.Substring(3);
                        prix = prix.Substring(3);
                        dateAmm = dateAmm.Substring(3);
                        Medicament medicament = new Medicament
                        {
                            AMM = aMM.Substring(3),
                            CodeProduit = codeProduit,
                            DCI1 = dCi1,
                            Designation = designation,
                            Fournisseur = fournisseur,
                            Prix = prix,
                            dateamm = dateAmm,
                            Nom = listName[cmpt].InnerHtml
                        };
                        PostMedicament(medicament);
                        

                    }
                    catch (Exception e)
                    {
                        

                    }
                    cmpt++;

                }


                    //}
                    //    resultat.LoadHtml(node.InnerHtml);

                    //    toftitle = resultat.DocumentNode.Descendants().Where(x => (x.Name == "option")).ToList();
                    //    List<string> list = new List<string>();
                    //    foreach (var element in toftitle)
                    //    {
                    //      list.Add(element.InnerHtml);
                    //    }

                    //    return list;
                }
           
        }
    }
}