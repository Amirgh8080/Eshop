using Application.Interface;
using Application.Security;
using Domain.Models.Order;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Areas.Profile.Controllers
{
    public class OrderController : BaseProfileController
    {
        IOrderService _orderService;
        IProductService _productService;

        public OrderController(IOrderService orderService, IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }
        [Route("Orders/{id}")]
        public async Task<IActionResult> Index(int id)
        {

            var res = await _orderService.GetOrderByUserId(id);
            var TotPrice = await _orderService.GetTotalPrice(res.Id);
            ViewBag.TotalPrice = TotPrice;
            return View(res);
        }
        [Route("Orders/{id}")]
        [HttpPost]
        public async Task<IActionResult> Index(Order model)
        {
            var order = await _orderService.GetOrderById(model.Id);
            var payment = new ZarinpalSandbox.Payment(await _orderService.GetTotalPrice(model.Id));

            var res = payment.PaymentRequest("پرداخت سفارش", "https://localhost:44348/OnlinePayment/" + model.Id);

            if (res.Result.Status == 100)
            {
                return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + res.Result.Authority);
            }
            return Content("salam");
        }

        [Route("BuyProduct/{id}/{productPriceId?}")]
        public async Task<IActionResult> BuyProduct(int id, int? productPriceId)
        {

            var orderId = await _orderService.AddOrderFromUser(User.GetUserId(), id, productPriceId);

            if (orderId == null)
            {
                return BadRequest();
            }

            return Redirect("/Profile/Orders/" + User.GetUserId());
        }

        [Route("OrderDetail/{id}")]
        public async Task<IActionResult> ShowOrderDetail(int id)
        {
            var orderDetails = await _orderService.GetListOrderDetailsByOrderId(id);
            if (orderDetails == null)
            {
                return BadRequest();
            }
            return View(orderDetails);
        }

        [Route("IncraeseCount/{id}")]
        public async Task<IActionResult> IncraeseCount(int id)
        {

            var orderDetail = await _orderService.GetOrderDetailById(id);
            if (orderDetail==null)
            {
                return NotFound();
            }

            orderDetail.Count += 1;

            var res = await _orderService.UpdateOrderDetail(orderDetail);

            return Redirect("/Profile/Orders/" + User.GetUserId());
        }

        [Route("DecreaseCount/{id}")]
        public async Task<IActionResult> DecreaseCount(int id)
        {

            var orderDetail = await _orderService.GetOrderDetailById(id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            orderDetail.Count -= 1;

            var res = await _orderService.UpdateOrderDetail(orderDetail);

            return Redirect("/Profile/Orders/" + User.GetUserId());
        }

        [Route("DeleteOrderDetail/{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {

            var orderDetail = await _orderService.GetOrderDetailById(id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            orderDetail.IsDelete =true;

            var res = await _orderService.UpdateOrderDetail(orderDetail);

            return Redirect("/Profile/Orders/" + User.GetUserId());
        }
    }
}
