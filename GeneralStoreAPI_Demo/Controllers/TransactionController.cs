using GeneralStoreAPI_Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GeneralStoreAPI_Demo.Controllers
{
    public class TransactionController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        // POST (Create)
        // Tracking inventory
        [HttpPost]
        public IHttpActionResult PostTransaction([FromBody] Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (transaction == null)
            {
                return BadRequest("The request body cannot be empty");
            }

            Customer customer = _context.Customers.Find(transaction.CustomerId);
            Product product = _context.Products.Find(transaction.ProductId);

            if (customer is null || product == null)
            {
                return NotFound();
            }

            if (product.NumberInInventory < transaction.ItemCount)
            {
                return BadRequest("Not enough in inventory");
            }

            _context.Transactions.Add(transaction);
            transaction.Product.NumberInInventory -= transaction.ItemCount;

            _context.SaveChanges();
            return Ok("Transaction added");
        }

        // GET ALL TRANSACTIONS
        [HttpGet]
        public IHttpActionResult GetAllTransactions()
        {
            return Ok(_context.Transactions.ToList());
        }

        // GET BY TRANSACTION ID
        [HttpGet]
        public IHttpActionResult GetByTransactionId(int id)
        {
            Transaction transaction = _context.Transactions.Find(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        // GET BY CUSTOMER ID
        [HttpGet]
        [Route("api/Transactions/GetTransactionByCustomerId/{id}")]
        public IHttpActionResult GetTransactionByCustomerId(int id)
        {   
            // ".Where" method is a filter for the list in this instance
            List<Transaction> transactions = _context.Transactions.Where(t => t.CustomerId == id).ToList();
            
            if (transactions.Count > 0)
                return Ok(transactions);

            return BadRequest("The customer has no transactions");
        }

        // PUT (Update)
        [HttpPut]
        public IHttpActionResult UpdateTransaction(int id, Transaction updatedTransaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (updatedTransaction == null)
            {
                return BadRequest("Body cannot be empty");
            }

            Transaction oldTransaction = _context.Transactions.Find(id);
            Customer newCustomer = _context.Customers.Find(updatedTransaction.CustomerId);
            Product newProduct = _context.Products.Find(updatedTransaction.ProductId);

            if (oldTransaction is null || newCustomer is null || newProduct is null)
            {
                return NotFound();
            }

            oldTransaction.Product.NumberInInventory += oldTransaction.ItemCount;

            oldTransaction.CustomerId = updatedTransaction.CustomerId;
            oldTransaction.ProductId = updatedTransaction.ProductId;
            oldTransaction.ItemCount = updatedTransaction.ItemCount;

            newProduct.NumberInInventory -= oldTransaction.ItemCount;

            // ".SaveChanges()" method essentially returns how many rows were changed in the database
            int numberOfChanges = _context.SaveChanges();

            // probably safer to do > 0 here to see if there were any changes at all
            if (numberOfChanges > 0)
            {
                return Ok("Updated the transaction");
            }

            return InternalServerError();
        }

        // DELETE
        [HttpDelete]
        public IHttpActionResult DeleteTransactionById(int id)
        {
            Transaction transaction = _context.Transactions.Find(id);

            if (transaction is null)
            {
                return NotFound();
            }

            // "+=" basically means you're adding the transaction to itself
            transaction.Product.NumberInInventory += transaction.ItemCount;
            _context.Transactions.Remove(transaction);

            int numberOfChanges = _context.SaveChanges();

            // There will be two changes here because we altered both the Transaction and Product table
            if (numberOfChanges == 2)
            {
                return Ok("Transaction deleted");
            }

            return InternalServerError();
        }
    }
}
