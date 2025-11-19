using OrderApi.Application.Services;
using FakeItEasy;
using System.Net.Http;
using OrderApi.Application.Dtos;
using FluentAssertions;
using System.Net.Http.Json;

namespace UnitTest.OrderApi.Services
{
    public class OrderServiceTest
    {
        private readonly IOrderService orderServiceInterface;

        public OrderServiceTest()
        {
            orderServiceInterface = A.Fake<IOrderService>();
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
    }
}
