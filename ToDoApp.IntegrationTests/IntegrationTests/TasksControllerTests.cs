using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Testing;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ToDoApp.API.DTO;
using ToDoApp.Core.DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using RegisterRequest = ToDoApp.API.DTO.RegisterRequest;

namespace ToDoApp.IntegrationTests.IntegrationTests
{
    public class TasksControllerTests : IClassFixture<CustomWebApplicationFactory>
    {

        private readonly WebApplicationFactory<Program> _factory;

        public TasksControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostRegistration_ReturnsOk()
        {
            
            var client = _factory.CreateClient();

            await RegisterUser(client);

        }

        [Fact]
        public async Task PostLogin()
        {
            
            var client = _factory.CreateClient();

            await RegisterUser(client);

            API.DTO.LoginRequest request = new API.DTO.LoginRequest();
            request.UserName = "Test";
            request.Password = "123456";
            

            string jsonRequest = JsonSerializer.Serialize<LoginRequest>(request);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/v1/auth/login", content);

            string token = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public async Task GetTasks_WithoutToken_ReturnsUnauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/v1/tasks");

            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetTasks_WithToken_ReturnsOk()
        {

            var client = _factory.CreateClient();

            await RegisterUser(client);

            API.DTO.LoginRequest request = new API.DTO.LoginRequest();
            request.UserName = "Test";
            request.Password = "123456";


            string jsonRequest = JsonSerializer.Serialize<LoginRequest>(request);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/v1/auth/login", content);

            string token = await response.Content.ReadAsStringAsync();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var responseBody = await client.GetAsync("/api/v1/tasks");

            Assert.Equal(HttpStatusCode.OK, responseBody.StatusCode);

            var data = await responseBody.Content.ReadFromJsonAsync<List<TaskResponseDto>>();
            Assert.NotNull(data);          
            Assert.Empty(data);

        }

        [Fact]
        public async Task PostTask_WithToken_CreatesTask()
        {
            
            var client = _factory.CreateClient();
            
            await RegisterUser(client);

            API.DTO.LoginRequest request = new API.DTO.LoginRequest();
            request.UserName = "Test";
            request.Password = "123456";


            string jsonRequest = JsonSerializer.Serialize<LoginRequest>(request);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/v1/auth/login", content);

            string token = await response.Content.ReadAsStringAsync();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            TaskCreateDto task = new TaskCreateDto();
            task.TaskName = "New Task";

            string jsonTask = JsonSerializer.Serialize<TaskCreateDto>(task);

            var stringContent = new StringContent(jsonTask, Encoding.UTF8, "application/json");

            var requestBody = await client.PostAsync("/api/v1/tasks", stringContent);

            Assert.Equal(HttpStatusCode.Created, requestBody.StatusCode);

            var createdTask = await requestBody.Content.ReadFromJsonAsync<TaskResponseDto>();
            Assert.NotNull(createdTask);
            Assert.Equal("New Task", createdTask.TaskName);
            Assert.True(createdTask.Id > 0);
        }

        [Fact]
        public async Task Login_WithWrongpassword_ReturnsUnauthorized()
        {
            var client = _factory.CreateClient();

            await RegisterUser(client);

            API.DTO.LoginRequest request = new API.DTO.LoginRequest();
            request.UserName = "Test";
            request.Password = "WrongPassword";

            string jsonRequest = JsonSerializer.Serialize<LoginRequest>(request);

            var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var requestBody = await client.PostAsync("/api/v1/auth/login", stringContent);

            Assert.Equal(HttpStatusCode.Unauthorized, requestBody.StatusCode);
        }


        [Fact]
        public async Task CreateTask_WithoutName_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();

            await RegisterUser(client);

            API.DTO.LoginRequest request = new API.DTO.LoginRequest();
            request.UserName = "Test";
            request.Password = "123456";


            string jsonRequest = JsonSerializer.Serialize<LoginRequest>(request);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/v1/auth/login", content);

            string token = await response.Content.ReadAsStringAsync();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            TaskCreateDto task = new TaskCreateDto();
            task.TaskName = "";
            task.IsCompleted = false;

            string jsonTask = JsonSerializer.Serialize<TaskCreateDto>(task);

            var stringContent = new StringContent(jsonTask, Encoding.UTF8, "application/json");

            var requestBody = await client.PostAsync("/api/v1/tasks", stringContent);

            Assert.Equal(HttpStatusCode.BadRequest, requestBody.StatusCode);

            string responseBody = await requestBody.Content.ReadAsStringAsync();

            Assert.Contains("TaskName", responseBody);
        }

        [Fact]
        public async Task Get_UnexistingTask_ReturnsNotFound()
        {
            var client = _factory.CreateClient();

            await RegisterUser(client);

            API.DTO.LoginRequest request = new API.DTO.LoginRequest();
            request.UserName = "Test";
            request.Password = "123456";


            string jsonRequest = JsonSerializer.Serialize<LoginRequest>(request);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/v1/auth/login", content);

            string token = await response.Content.ReadAsStringAsync();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var responseBody = await client.GetAsync("/api/v1/tasks/99999");

            Assert.Equal(HttpStatusCode.NotFound, responseBody.StatusCode);

        }

        [Fact]
        public async Task GetTasks_OnlyForUserId_ReturnsOkOrNotFound()
        {
            // ========== USER 1 ==========
            var client1 = _factory.CreateClient();

            var request1 = new RegisterRequest
            {
                UserName = "User1",
                Password = "password1",
                Email = "1@test.com"
            };
            await client1.PostAsJsonAsync("/api/v1/auth/register", request1);

           
            var user1Login = new LoginRequest
            {
                UserName = "User1",
                Password = "password1"
            };
            var loginResponse1 = await client1.PostAsJsonAsync("/api/v1/auth/login", user1Login);
            var token1 = await loginResponse1.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(token1))
            {
                throw new Exception("User1 token is empty!");
            }

            var authClient1 = _factory.CreateClient();
            authClient1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token1);

            // Создание задачи User1
            var task = new TaskCreateDto
            {
                TaskName = "Task 1",
                IsCompleted = false
            };
            var createResponse = await authClient1.PostAsJsonAsync("/api/v1/tasks", task);


            var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskResponseDto>();
            var taskId = createdTask.Id;

            // ========== USER 2 ==========
            var client2 = _factory.CreateClient();

            var request2 = new RegisterRequest
            {
                UserName = "User2",
                Password = "password2",
                Email = "2@test.com"
            };
            await client2.PostAsJsonAsync("/api/v1/auth/register", request2);

            var user2Login = new LoginRequest
            {
                UserName = "User2",
                Password = "password2"
            };
            var loginResponse2 = await client2.PostAsJsonAsync("/api/v1/auth/login", user2Login);
            var token2 = await loginResponse2.Content.ReadAsStringAsync();

            var authClient2 = _factory.CreateClient();
            authClient2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token2);

            // User2 запрашивает все задачи
            var allTasksResponse = await authClient2.GetAsync("/api/v1/tasks");
            var allTasks = await allTasksResponse.Content.ReadFromJsonAsync<List<TaskResponseDto>>();

            // User2 запрашивает задачу User1 по ID
            var oneTaskResponse = await authClient2.GetAsync($"/api/v1/tasks/{taskId}");

            Assert.Equal(HttpStatusCode.OK, allTasksResponse.StatusCode);
            Assert.NotNull(allTasks);
            Assert.Empty(allTasks);

            Assert.Equal(HttpStatusCode.NotFound, oneTaskResponse.StatusCode);
        }

        private async Task RegisterUser(HttpClient client)
        {
            var request = new RegisterRequest
            {
                UserName = "Test",
                Password = "123456",
                Email = "test@test.com"
            };

            await client.PostAsJsonAsync("/api/v1/auth/register", request);

        }
    }
}
  