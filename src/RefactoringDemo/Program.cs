using NSubstitute;
using RefactoringDemo.Application.ECommerce.Orders;
using RefactoringDemo.Domain.ECommerce.Orders;
using System;
using System.Collections.Generic;

namespace RefactoringDemo
{
    public class Program
    {
        private static ICustomerRepository _customerRepository;
        private static IProductRepository _productRepository;
        private static IOrderRepository _orderRepository;

        public static void Main(string[] args)
        {
            InitializeFake();

            var command = new CreateOrderCommand
            {
                CustomerId = Guid.Parse("418be026-b301-4696-b062-08d70cfeca04"),
                DF = 4,
                Discount = 10,
                Items = new List<CreateOrderItemCommand>
                {
                    new CreateOrderItemCommand
                    {
                        ProductId =  Guid.Parse("608f1b52-07ed-42ec-a1a3-55c4e73a8755"),
                        Quantity = 6
                    },
                    new CreateOrderItemCommand
                    {
                        ProductId =  Guid.Parse("36d8130d-608f-45ff-a177-8137ca8bc7b6"),
                        Quantity = 2
                    }
                }
            };

            var appService = new OrderApplicationService(_customerRepository, _productRepository, _orderRepository);
            var result = (CreateOrderCommandResult)appService.Handle(command);

            Render(result);
        }

        private static void Render(CreateOrderCommandResult result)
        {
            Console.WriteLine($"The order {result.Number} was successfully created.");
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine($"     DateTime: {result.CreatedDateTime}");
            Console.WriteLine($"     SubTotal: {result.SubTotal.ToString("C")}");
            Console.WriteLine($" Delivery Fee: {result.DeliveryFee.ToString("C")}");
            Console.WriteLine($"     Discount: {result.Discount.ToString("C")}");
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine($"        TOTAL: {result.Total.ToString("C")}");

            Console.ReadKey();
        }

        private static void InitializeFake()
        {
            _customerRepository = Substitute.For<ICustomerRepository>();
            _customerRepository.Get(Arg.Any<Guid>()).Returns(new Customer
            {
                Name = "Maicon Heck",
                BirthDate = new DateTime(1985, 12, 9),
                Email = "contato@maiconheck.com",
                Nif = "537234789"
            });

            _productRepository = Substitute.For<IProductRepository>();

            _productRepository.Get(Guid.Parse("608f1b52-07ed-42ec-a1a3-55c4e73a8755")).Returns(new Product
            {
                Name = "T-Shirt",
                Price = 20m,
            });
            _productRepository.Get(Guid.Parse("36d8130d-608f-45ff-a177-8137ca8bc7b6")).Returns(new Product
            {
                Name = "Pants",
                Price = 10m,
            });

            _orderRepository = Substitute.For<IOrderRepository>();
        }
    }
}