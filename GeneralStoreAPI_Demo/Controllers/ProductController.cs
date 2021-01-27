using GeneralStoreAPI_Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GeneralStoreAPI_Demo.Controllers
{
    public class ProductController : ApiController
    {
        private ApplicationDbContext _context = new ApplicationDbContext();

        //POST
        public IHttpActionResult Post(Product product)
        {
            if (product == null)
            {
                return BadRequest("Your request body cannot be empty.");
            }

            //if the ModelState is not Valid
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest(ModelState);
        }

        //GET ALL
        public IHttpActionResult Get()
        {
            List<Product> products = _context.Products.ToList();
            if (products.Count != 0)
            {
                return Ok(products);
            }
            return BadRequest("Your database contains no Products");
        }

        //GET BY ID
        public IHttpActionResult Get(int id)
        {

            Product product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        //PUT
        public IHttpActionResult Put(int id, Product newProduct)
        {
            if (ModelState.IsValid)
            {
                Product product = _context.Products.Find(id);
                if (product != null)
                {
                    product.Name = newProduct.Name;
                    product.Cost = newProduct.Cost;
                    product.NumberInInventory = newProduct.NumberInInventory;
                    _context.SaveChanges();
                    return Ok("Product Updated");
                }
                return NotFound();
            }
            return BadRequest(ModelState);
        }

        //DELETE
        public IHttpActionResult Delete(int id)
        {
            Product product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            if (_context.SaveChanges() == 1)
            {
                return Ok("Product Deleted");
            }
            return InternalServerError();
        }
    }
}

