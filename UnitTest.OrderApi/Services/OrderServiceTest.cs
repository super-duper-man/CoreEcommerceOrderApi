using OrderApi.Application.Services;
using FakeItEasy;
using System.Net.Http;
using OrderApi.Application.Dtos;
using FluentAssertions;
using System.Net.Http.Json;
using OrderApi.Domain.Entities;
using System.Linq.Expressions;
using OrderApi.Application.Interfaces;

namespace UnitTest.OrderApi.Services
{
    public class OrderServiceTest
    {
        private readonly IOrderService orderServiceInterface;
        private readonly IOrder orderInterface;

        public OrderServiceTest()
        {
            orderServiceInterface = A.Fake<IOrderService>();
            orderInterface = A.Fake<IOrder>();
        }

        //Create Fake HttpMessageHandler

        public class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response = response;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(_response);
        }

        //Create Fake HttpClient

        private static HttpClient CreateFakeHttpClient(object o)
        {
            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = JsonContent.Create(o)
            };
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);
            var _httpClient = new HttpClient(fakeHttpMessageHandler)
            {
                BaseAddress = new Uri("http://localhost")
            };

            return _httpClient;
        }

        [Fact]
        public async Task GetProduct_ValidProductId_ReturnProduct()
        {
            //Arrange
            int productId = 1;
            var productDto = new ProductDto(1, "Product 1", 13, 56.76m);
            var _httpClient = CreateFakeHttpClient(productDto);

            //Only need httpClien to make call
            //Specify only httpClient and null to the rest
            var _orderService = new OrderService(null!, _httpClient, null!);

            //Act
            var result = await _orderService.GetProduct(productId);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
            result.Name.Should().Be("Product 1");
        }


        [Fact]
        public async Task GetProduct_InvalidProductId_ReturnNull()
        {
            //Arrange
            int productId = 1;
            var _httpClient = CreateFakeHttpClient(null!);

            //Only need httpClien to make call
            //Specify only httpClient and null to the rest
            var _orderService = new OrderService(null!, _httpClient, null!);

            //Act
            var result = await _orderService.GetProduct(productId);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUser_ValidUserId_ReturnUser()
        {
            //Arrange
            int userId = 1;
            var appUserDto = new AppUserDto(1, "testuser", "","","www@test.com", "123", "admin");
            var _httpClient = CreateFakeHttpClient(appUserDto);

            //Only need httpClien to make call
            //Specify only httpClient and null to the rest
            var _orderService = new OrderService(null!, _httpClient, null!);

            //Act
            var result = await _orderService.GetUser(userId);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
            result.Name.Should().Be("testuser");
        }

        [Fact]
        public async Task GetOrderByClientId_OrderExists_ReturnOrderDetails()
        {
            //Arrange
            int clinetId = 1;
            var orders = new List<Order>{
                new() {Id=1, ProductId=1, ClientId=clinetId, PurchaseQuantity = 2, OrderDate=DateTime.UtcNow },
                new() {Id=2, ProductId=2, ClientId=clinetId, PurchaseQuantity = 1, OrderDate=DateTime.UtcNow },
            };

            //mock the GetOrdersBy method
            A.CallTo(() => orderInterface.GetOrdersAsync(A<Expression<Func<Order, bool>>>.Ignored)).Returns(orders);
            var orderService = new OrderService(orderInterface, null!, null!);

            //Act 
            var result = await orderService.GetOrderByClientId(clinetId);

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(orders.Count);
        }
    }
}
