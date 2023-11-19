using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _1FinalCapstone.Models;
using static _1FinalCapstone.Controllers.HomeController;

namespace _1FinalCapstone.Controllers
{
    public class CompletedOrdersController : Controller
    {
        private Entities db = new Entities();

        // GET: CompletedOrders
        [Authorize(Roles = "Admin, Owner")]
        public ActionResult ListofCompletedOrders(int page = 1, int pageSize = 5)
        {
            IQueryable<CompletedOrders> query = db.CompletedOrders.AsQueryable();

            // Apply sorting by CompletedDate (you can change this to the appropriate property)
            query = query.OrderBy(order => order.CompletedDate);

            var paginatedOrders = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalItems = query.Count();

            var model = new PaginatedList<CompletedOrders>(paginatedOrders, page, pageSize, totalItems);

            return View(model);
        }

        public ActionResult CalculateTotalPrice()
        {
            try
            {
                decimal totalPrice = 0;
                decimal totalCost = 0;

                // Calculate total price from CompletedOrders
                foreach (var order in db.CompletedOrders)
                {
                    totalPrice += (decimal)order.TotalPrice; // Cast to decimal
                }

                // Calculate total cost from the Product table
                foreach (var product in db.Product)
                {
                    totalCost += (decimal)(product.ItemCost * product.ItemQuantity);
                }

                // Calculate total cost from the BikeBuilder table
                foreach (var bike in db.BikeBuilder)
                {
                    totalCost += (decimal)bike.Bprice1;
                }

                // Calculate the difference between totalPrice and totalCost
                decimal difference = totalCost - totalPrice;
                decimal earned = totalPrice - totalCost;

                // Create a JSON object containing all three totals
                var result = new
                {
                    TotalPrice = totalPrice,
                    TotalCost = totalCost,
                    Difference = difference,
                    Earned = earned
                };

                // Return the JSON object as the result
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Handle any exceptions here and return an error message if necessary
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
