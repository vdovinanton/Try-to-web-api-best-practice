﻿using System;
using System.Collections.Generic;
using System.Linq;
using WebApplicationExercise.Models;
using System.Data.Entity;
using System.Threading.Tasks;
using WebApplicationExercise.Utils;
using WebApplicationExercise.Core.Interfaces;
using NLog;

namespace WebApplicationExercise.Core
{
    public class OrderService: IOrderService
    {
        private readonly DataContext _db;
        private readonly ICustomerService _customerService;

        private readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        public OrderService(DataContext db, ICustomerService customerService)
        {
            _db = db;
            _customerService = customerService;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var result = await _db.Orders
                .AsNoTracking()
                .Include(o => o.Products)
                .ToListAsync();
            //Logger.Instance.Information($"Get all orders [{result.Count()}]");
            _logger.Info($"Get all orders [{result.Count()}]");
            return result;
        }

        public async Task<Order> GetByIdAsync(int orderId)
        {
            //Logger.Instance.Information($"Get order by Id - {orderId}");
            _logger.Info($"Get order by Id - {orderId}");
            return await _db.Orders
                .AsNoTracking()
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        private IQueryable<Order> FilterByCustomer()
        {
            //Logger.Instance.Information($"Filtered orders by customer {Settings.Instance.CustomerName}");
            _logger.Info($"Filtered orders by customer {Settings.Instance.CustomerName}");
            return _db.Orders
                .AsNoTracking()
                .Where(o => _customerService.IsCustomerVisible(o.CustomerName));
        }

        private IQueryable<Order> FilterByCustomer(string customerName)
        {
            //Logger.Instance.Information($"Filtered orders by customer {customerName}");
            _logger.Info($"Filtered orders by customer {customerName}");
            return _db.Orders
                .AsNoTracking()
                .Where(o => o.CustomerName == customerName);
        }

        private IQueryable<Order> FilterByDate(DateTime from, DateTime to)
        {
            //Logger.Instance.Information($"Filtered orders by date");
            _logger.Info($"Filtered orders by date");
            return _db.Orders
                .AsNoTracking()
                .Where(o => o.CreatedDate.ConvertFromUnixTimestamp() >= DbFunctions.TruncateTime(from) && o.CreatedDate.ConvertFromUnixTimestamp() < DbFunctions.TruncateTime(to));
        }

        public async Task<IEnumerable<Order>> OrderFilterAsync(DateTime from, DateTime to, string customerName)
        {
            if (DateTime.Compare(from, to) >= 0)
                throw new ArgumentOutOfRangeException($"{nameof(to)} date can't be earlier than {nameof(from)}");

            IQueryable<Order> result = FilterByCustomer();
            if (from != null && to != null)
                result = result.Union(FilterByDate(from, to)); //todo: << Union remove

            if(customerName != null)
                result = result.Union(FilterByCustomer(customerName)); //todo: << Union remove

            return await result.ToListAsync();
        }

        public async Task<int> UpdateOrCreateOrderAsync(Order order)
        {
            var originalOrder = _db.Orders
                .AsNoTracking()
                .FirstOrDefault(_ => _.Id == order.Id);

            if (!_db.Orders.AsNoTracking().Any(_ => _.Id == order.Id))
            {
                _db.Orders.Add(order);
                //Logger.Instance.Information($"Added orderId - {order.Id} with {order.Products.Count} products");
                _logger.Info($"Added orderId - {order.Id} with {order.Products.Count} products");
            }
            else
            {
                _db.Entry(order).State = EntityState.Modified;
                foreach(var product in order.Products)
                    _db.Entry(product).State = EntityState.Modified;

                //Logger.Instance.Information($"Modified orderId - {order.Id} with {order.Products.Count} products");
                _logger.Info($"Modified orderId - {order.Id} with {order.Products.Count} products");
            }

            await _db.SaveChangesAsync();

            return order.Id;
        }


        public async Task RemoveAsync(int orderId)
        {
            if (!Equals(orderId, default(int)))
            {
                var order = GetByIdAsync(orderId);
                _db.Entry(order).State = EntityState.Deleted;
                await _db.SaveChangesAsync();
                //Logger.Instance.Information($"Remove order Id - {order.Id}");
                _logger.Info($"Remove order Id - {order.Id}");
            }
            else
            {
                throw new ArgumentNullException($"{nameof(orderId)} can't be empty");
            }
        }
    }
}