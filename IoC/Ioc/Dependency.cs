using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Application.Services;
using DataLayer.Repositories;
using Domain.Interfaces;
using Domain.IRepositories;
using Microsoft.Extensions.DependencyInjection;
using TopLearn.Core.Convertor;

namespace IoC.Ioc
{
    public class Dependency
    {
        public static void RegisterServices(IServiceCollection service)
        {
            service.AddTransient<IUserService, UserService>();
            service.AddTransient<IUserRepository, UserRepository>();
            service.AddTransient<IViewRenderService, RenderViewToString>();
            service.AddTransient<IContactUssRepository, ContactUssRepository>();
            service.AddTransient<IContactUssService, ContactUssService>();
            service.AddTransient<ITicketRepository, TicketRepository>();
            service.AddTransient<ITicketService, TicketServices>();
            service.AddTransient<IProductRepository, ProductRepository>();
            service.AddTransient<IProductService, ProductService>();
            service.AddTransient<IOrderRepository, OrderRepository>();
            service.AddTransient<IOrderService, OrderService>();
            service.AddTransient<IDynamicLinkService, DynamicLinkService>();
            service.AddTransient<IDynamicRepository, DynamicLinkRepository>();
            service.AddTransient<ISocialMediaRepository, SocialMediaRepository>();
            service.AddTransient<ISocialMediaService, SocialMediaService>();
            service.AddTransient<IPermissionRoleRepository, PermissionRoleRepository>();
            service.AddTransient<IPermissionService, PermissionService>();

        }
    }
}
