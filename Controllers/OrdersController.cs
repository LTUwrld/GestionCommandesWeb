﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionCommandesWeb.Data;
using GestionCommandesWeb.Models;
using GestionCommandesWeb.Shared;
using System.Dynamic;
using static NuGet.Packaging.PackagingConstants;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Linq;
using NuGet.Protocol;
using Microsoft.AspNetCore.Http;

namespace GestionCommandesWeb.Controllers
{
    public class OrdersController : Controller
    {
        private readonly GestionCommandesWebContext _context;
        PersistOrdersViewModel persistOrdersViewModel = new PersistOrdersViewModel();
        public OrdersController(GestionCommandesWebContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(string searchString, string currentFilter, int? pageNumber)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var gestionCommandesWebContext = _context.Orders.Include(o => o.Customers);

            if (!string.IsNullOrEmpty(searchString))
            {
                gestionCommandesWebContext = gestionCommandesWebContext.Where(
                     el => el.OrderID.ToString().Contains(searchString) ||
                         el.Customers.CustomerID.ToString().Contains(searchString))
                         .Include(o => o.Customers);
            }

            int pageSize = 15;
            return View(await PaginatedList<Orders>.CreateAsync(gestionCommandesWebContext.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Orders? orders = await _context.Orders
                .Include(o => o.Customers)
                .Include(o => o.Order_Details)
                .ThenInclude(od => od.Products)
                .FirstOrDefaultAsync(m => m.OrderID == id);

            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            PersistOrdersViewModel? data = null;

            if (HttpContext.Session.GetString("persistOrdersViewModel") != null)
            {
                data = JsonConvert.DeserializeObject<PersistOrdersViewModel>(HttpContext.Session.GetString("persistOrdersViewModel"));
            }

            if (data != null)
            {
                persistOrdersViewModel = data;
            }

            if (persistOrdersViewModel.Update_Order_Detail != null &&
                persistOrdersViewModel.Update_Order_Detail.ProductID != null)
            {
                persistOrdersViewModel.Order_Detail = persistOrdersViewModel.Update_Order_Detail;

            }

            ViewData["CustomerID"] = new SelectList(_context.Set<Customers>(), "CustomerID", "ContactName");
            ViewData["ProductID"] = new SelectList(_context.Set<Products>(), "ProductID", "ProductName");

            if (persistOrdersViewModel.Orders.CustomerID != null)
            {
                ViewData["CustomerID"] = new SelectList(_context.Set<Customers>(), "CustomerID", "ContactName", persistOrdersViewModel.Orders.CustomerID);
            }

            HttpContext.Session.SetString("orderDetailReturnView", nameof(Create));

            return View(persistOrdersViewModel);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PersistOrdersViewModel model)
        {
            PersistOrdersViewModel? data = null;

            if (HttpContext.Session.GetString("persistOrdersViewModel") != null)
            {
                data = JsonConvert.DeserializeObject<PersistOrdersViewModel>(HttpContext.Session.GetString("persistOrdersViewModel"));
            }

            if (data != null)
            {
                persistOrdersViewModel = data;
            }

            foreach (var item in persistOrdersViewModel.Orders.Order_Details)
            {
                item.Products = null;
            }

            persistOrdersViewModel.Orders.CustomerID = model.Orders.CustomerID;

            if (string.IsNullOrEmpty(persistOrdersViewModel.Orders.CustomerID))
            {
                return RedirectToAction(HttpContext.Session.GetString("orderDetailReturnView"));
            }


            _context.Add(persistOrdersViewModel.Orders);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("persistOrdersViewModel");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrderDetail(PersistOrdersViewModel model)
        {
            PersistOrdersViewModel? data = null;

            if (HttpContext.Session.GetString("persistOrdersViewModel") != null)
            {
                data = JsonConvert.DeserializeObject<PersistOrdersViewModel>(HttpContext.Session.GetString("persistOrdersViewModel"));
            }

            if (data != null)
            {
                persistOrdersViewModel = data;
            }

            var potentialOrder_Details = persistOrdersViewModel.Orders.Order_Details.FirstOrDefault(el => el.ProductID == model.Order_Detail.ProductID);

            if (potentialOrder_Details != null)
            {
                potentialOrder_Details.Quantity = model.Order_Detail.Quantity;
                potentialOrder_Details.UnitPrice = model.Order_Detail.UnitPrice;
            }
            else
            {
                Products? products = await _context.Products.FirstOrDefaultAsync(i => i.ProductID == model.Order_Detail.ProductID);

                persistOrdersViewModel.Orders.Order_Details.Add(new Order_Details
                {
                    Products = products,
                    ProductID = model.Order_Detail.ProductID,
                    Quantity = model.Order_Detail.Quantity,
                    UnitPrice = model.Order_Detail.UnitPrice
                });
            }

            persistOrdersViewModel.Order_Detail = new Order_Details();
            persistOrdersViewModel.Update_Order_Detail = new Order_Details();

            if (model.Orders.CustomerID != null)
            {
                persistOrdersViewModel.Orders.CustomerID = model.Orders.CustomerID; 
            }

            HttpContext.Session.SetString("persistOrdersViewModel", JsonConvert.SerializeObject(persistOrdersViewModel));

            object? passedId = persistOrdersViewModel.Orders.OrderID != 0 ? new { id = persistOrdersViewModel.Orders.OrderID } : null;

            return RedirectToAction(HttpContext.Session.GetString("orderDetailReturnView"), passedId);
        }

        public IActionResult EditOrderDetail(int? id)
        {
            PersistOrdersViewModel? data = null;

            if (HttpContext.Session.GetString("persistOrdersViewModel") != null)
            {
                data = JsonConvert.DeserializeObject<PersistOrdersViewModel>(HttpContext.Session.GetString("persistOrdersViewModel"));
            }

            if (data != null)
            {
                persistOrdersViewModel = data;
            }


            Order_Details? order_Details = persistOrdersViewModel.Orders.Order_Details.FirstOrDefault(el => el.ProductID == id);

            persistOrdersViewModel.Update_Order_Detail = order_Details;

            HttpContext.Session.SetString("persistOrdersViewModel", JsonConvert.SerializeObject(persistOrdersViewModel));

            object? passedId = persistOrdersViewModel.Orders.OrderID != 0 ? new {id = persistOrdersViewModel.Orders.OrderID} : null;

            Debug.Print("passedId " + passedId.ToJson());

            return RedirectToAction(HttpContext.Session.GetString("orderDetailReturnView"), passedId);
        }

        public IActionResult DeleteOrderDetail(int? id)
        {
            PersistOrdersViewModel? data = null;

            if (HttpContext.Session.GetString("persistOrdersViewModel") != null)
            {
                data = JsonConvert.DeserializeObject<PersistOrdersViewModel>(HttpContext.Session.GetString("persistOrdersViewModel"));
            }

            if (data != null)
            {
                persistOrdersViewModel = data;
            }

            Order_Details? order_Details = persistOrdersViewModel.Orders.Order_Details.FirstOrDefault(el => el.ProductID == id);

            persistOrdersViewModel.Orders.Order_Details.Remove(order_Details);

            HttpContext.Session.SetString("persistOrdersViewModel", JsonConvert.SerializeObject(persistOrdersViewModel));

            object? passedId = persistOrdersViewModel.Orders.OrderID != 0 ? new { id = persistOrdersViewModel.Orders.OrderID } : null;

            return RedirectToAction(HttpContext.Session.GetString("orderDetailReturnView"), passedId);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PersistOrdersViewModel? data = null;

            if (HttpContext.Session.GetString("persistOrdersViewModel") != null)
            {
                data = JsonConvert.DeserializeObject<PersistOrdersViewModel>(HttpContext.Session.GetString("persistOrdersViewModel"));
            }

            if (data != null)
            {
                persistOrdersViewModel = data;
            }

            if (persistOrdersViewModel.Orders.OrderID == 0)
            {
                Orders? orders = await _context.Orders
                                .Include(o => o.Customers)
                                .Include(o => o.Order_Details)
                                .ThenInclude(od => od.Products)
                                .FirstOrDefaultAsync(m => m.OrderID == id);
                if (orders == null)
                {
                    return NotFound();
                }

                persistOrdersViewModel.Orders = new Orders
                {
                    OrderID = orders.OrderID,
                    OrderDate = orders.OrderDate,
                    Order_Details = orders.Order_Details,
                    CustomerID = orders.Customers.CustomerID
                };
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                HttpContext.Session.SetString("persistOrdersViewModel", JsonConvert.SerializeObject(persistOrdersViewModel, settings));
            }

            if (persistOrdersViewModel.Update_Order_Detail != null &&
                persistOrdersViewModel.Update_Order_Detail.ProductID != null)
            {
                persistOrdersViewModel.Order_Detail = persistOrdersViewModel.Update_Order_Detail;
            }

            ViewData["CustomerID"] = new SelectList(_context.Set<Customers>(), "CustomerID", "CustomerID", persistOrdersViewModel.Orders.CustomerID);
            ViewData["ProductID"] = new SelectList(_context.Set<Products>(), "ProductID", "ProductName");

            HttpContext.Session.SetString("orderDetailReturnView", nameof(Edit));

            return View(persistOrdersViewModel);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PersistOrdersViewModel model)
        {
            PersistOrdersViewModel? data = null;

            if (HttpContext.Session.GetString("persistOrdersViewModel") != null)
            {
                data = JsonConvert.DeserializeObject<PersistOrdersViewModel>(HttpContext.Session.GetString("persistOrdersViewModel"));
            }

            if (data != null)
            {
                persistOrdersViewModel = data;
            }

            if (id != persistOrdersViewModel.Orders.OrderID)
            {
                return NotFound();
            }

            foreach (var item in persistOrdersViewModel.Orders.Order_Details)
            {
                item.Products = null;
            }

            persistOrdersViewModel.Orders.CustomerID = model.Orders.CustomerID;

            if (string.IsNullOrEmpty(persistOrdersViewModel.Orders.CustomerID))
            {
                return RedirectToAction(HttpContext.Session.GetString("orderDetailReturnView"));
            }


            _context.Update(persistOrdersViewModel.Orders);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("persistOrdersViewModel");

            return RedirectToAction(nameof(Details), new { id = id });
            
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .Include(o => o.Customers)
                .Include(o => o.Order_Details)
                .ThenInclude(od => od.Products)
                .FirstOrDefaultAsync(m => m.OrderID == id);

            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orders = await _context.Orders
                .Include(o => o.Order_Details)
                .FirstOrDefaultAsync(m => m.OrderID == id);

            if (orders != null)
            {
                _context.Order_Details.RemoveRange(orders.Order_Details);
                _context.Orders.Remove(orders);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdersExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}
