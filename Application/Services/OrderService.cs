using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Interfaces;
using Domain.Models.Order;
using Domain.ViewModels.Order;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        IOrderRepository _repository;
        IProductRepository _productRepository;

        public OrderService(IOrderRepository repository, IProductRepository productRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
        }


        public async Task<OrderDetailForAdminViewModel> GetOrderDetailForAdminById(int orderId)
        {
            var order = await _repository.GetOrderDetailByOrderId(orderId);
            int totalPrice = 0;
            foreach (var orderDetail in order)
            {
                totalPrice += orderDetail.Price * orderDetail.Count;
            }
            var res = new OrderDetailForAdminViewModel()
            {
                OrderDetails = order,
                TotalPriceOfOrder = totalPrice

            };

            return res;
        }

        public async Task<OrderListPartialViewModel> GetFinalizedOrdersForAdmin()
        {
            var orders = await _repository.GetAllFinalizedOrders();

            var res = new OrderListPartialViewModel() { Orders = orders };

            return res;
        }


  
        public async Task<int> GetTotalPrice(int orderId)
        {
            return await _repository.GetTotalPrice(orderId);
        }

        public async Task<Order> GetOrderByUserId(int userId)
        {
            return await _repository.GetOrderByUserId(userId);
        }


        public async Task<int> AddOrderFromUser(int userId, int productId, int? productPriceId)
        {
            var getOrder = await _repository.GetOrderByUserId(userId);
            var product = await _productRepository.GetProductById(productId);
            var productPrice = product.productPrices.FirstOrDefault(c => c.Id == productPriceId);

            var OrderPrdouctFeature = productPrice.productSelectedFeatures
                .Where(c => c.ProductPriceId == productPriceId).Select(c => c.Feature.Title).ToList();

            var OrderProductFeatureValue = productPrice.productSelectedFeatures
                .Where(c => c.ProductPriceId == productPriceId).Select(c => c.featureValue.Value).ToList();

            var productFeatureAndValues =
                new Tuple<List<string>, List<string>>(OrderPrdouctFeature, OrderProductFeatureValue);
            if (getOrder == null)
            {
                var addOrder = new Order()
                {
                    CreatDate = DateTime.Now,
                    IsDelete = false,
                    IsFinally = false,
                    UserId = userId,
                    OrderDetails = new List<OrderDetail>()
                    {
                        new OrderDetail()
                        {
                            IsDelete = false,
                            Count = 1,
                            CreatDate = DateTime.Now,
                            ProductId = product.Id,
                            Price = (productPriceId==null)?product.Price:productPrice.Price,
                            ProductPriceId = (productPriceId != null ? productPriceId.Value : null)
                        }
                    }
                };

                foreach (var item1 in productFeatureAndValues.Item1)
                {
                    foreach (var item2 in productFeatureAndValues.Item2)
                    {
                        var addOrderProductFeature = new OrderDetailProductFeature()
                        {
                            CreatDate = DateTime.Now,
                            IsDelete = false,
                            FeatureTitle = item1,
                            FeatureValue = item2,
                            OrderDetailId = addOrder.OrderDetails.FirstOrDefault(c=>c.OrderId==addOrder.Id).Id
                        };
                    }
                }

                return await _repository.AddOrderFromUser(addOrder);
            }

            else
            {
                var orderDetial = await _repository.GetOrderDetailByOrderId(getOrder.Id, product.Id, productPrice.Id);
                if (orderDetial != null)
                {
                    orderDetial.Count += 1;
                    var result = await _repository.EditOrderDetail(orderDetial);


                    foreach (var item1 in productFeatureAndValues.Item1)
                    {
                        foreach (var item2 in productFeatureAndValues.Item2)
                        {
                            var addOrderProductFeature = new OrderDetailProductFeature()
                            {
                                CreatDate = DateTime.Now,
                                IsDelete = false,
                                FeatureTitle = item1,
                                FeatureValue = item2,
                                OrderDetailId = orderDetial.Id
                            };
                            int orderDetailProductFeatureId = await _repository.AddOrderDetailProductFeature(addOrderProductFeature);
                        }
                    }
                }
                else
                {
                    var addDetail = new OrderDetail()
                    {
                        Count = 1,
                        CreatDate = DateTime.Now,
                        IsDelete = false,
                        OrderId = getOrder.Id,
                        Price = (productPriceId == null) ? product.Price : productPrice.Price,
                        ProductId = product.Id,
                        ProductPriceId = productPriceId
                    };

                       var res= await _repository.AddOrderDetialFromUser(addDetail);

                    foreach (var item1 in productFeatureAndValues.Item1)
                    {
                        foreach (var item2 in productFeatureAndValues.Item2)
                        {
                            var addOrderProductFeature = new OrderDetailProductFeature()
                            {
                                CreatDate = DateTime.Now,
                                IsDelete = false,
                                FeatureTitle = item1,
                                FeatureValue = item2,
                                OrderDetailId = addDetail.Id
                            };

                           int orderDetailProductFeatureId= await _repository.AddOrderDetailProductFeature(addOrderProductFeature);
                        }
                    }

                    return res;
                }

            }

            return getOrder.Id;

        }

        public async Task<List<OrderDetail>> GetListOrderDetailsByOrderId(int orderId)
        {
            return await _repository.GetListOrderDetailsByOrderId(orderId);
        }

        public async Task<OrderDetail> GetOrderDetailById(int orderDetailId)
        {
            var orderDetail = await _repository.GetOrderDetailById(orderDetailId);
            return orderDetail;
        }

        public async Task<List<OrderDetail>> GetOrderDetailByOrderId(int orderId)
        {
            return await _repository.GetOrderDetailByOrderId(orderId);
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _repository.GetOrderById(orderId);
        }

        public async Task<bool> UpdateOrder(Order order)
        {
            return await _repository.UpdateOrder(order);
        }

        public async Task<int> AddOrderDetailProductFeature(OrderDetailProductFeature model)
        {
            return await _repository.AddOrderDetailProductFeature(model);
        }

        public async Task<bool> UpdateOrderDetail(OrderDetail model)
        {
            return await _repository.UpdateOrderDetail(model);
        }

        public async Task<FilterUserOrdersForAdmin> GetAllOrdersOfUserForAdmin(FilterUserOrdersForAdmin filter)
        {
            return await _repository.GetAllOrdersOfUserById(filter);
        }

        public async Task<SalesOrderChartViewModel> GetSalesOrderChartForAdmin()
        {
            var res = new SalesOrderChartViewModel()
            {
                ChartData = await _repository.GetWeeklySalesOrderForChart()
            };
            return res;
        }
    }
}
