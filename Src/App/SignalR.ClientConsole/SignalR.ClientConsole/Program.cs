using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using System.Net.Http.Json;

using SignalR.ClientConsole;

HttpClient client = new();

var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5003/eHub/notification", options =>
    {
        options.AccessTokenProvider = async () =>
        {
            var loginRequest = ReadLoginDetails();

            var token = await GetAccessTokenAsync(loginRequest);
            Console.WriteLine(JsonConvert.SerializeObject(token, JsonSettings()));
            return token.AccessToken;
        };

    })
    .Build();

connection.On<object>("AccountBalance", (message) =>
{
    Console.WriteLine(message);
});

connection.On<object>("Deposit", (message) =>
{
    Console.WriteLine(message);
});


connection.On<object>("Withdraw", (message) =>
{
    Console.WriteLine(message);
});


// Loop is here to wait until the server is running
while (true)
{
    try
    {
        await connection.StartAsync();

        Console.WriteLine(connection.ConnectionId);

        break;
    }
    catch (Exception exception)
    {
        Console.WriteLine(exception.Message);
        await Task.Delay(1000);
    }
}

Console.WriteLine("Client listening eHubb SignalR Message. Hit Ctrl-C to quit.");
Console.ReadLine();


JsonSerializerSettings JsonSettings()
{
    return new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        Formatting = Formatting.Indented,
        Culture = CultureInfo.CurrentUICulture

    };
}

async Task<AccessTokenModel> GetAccessTokenAsync(LoginRequestModel login)
{
    Console.WriteLine("Get AccessToken Async");
    HttpResponseMessage response = await client.PostAsJsonAsync(
        "http://localhost:5000/identity/Authenticate", login);
    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<AccessTokenModel>();
}

LoginRequestModel ReadLoginDetails()
{
    Console.WriteLine();
    Console.Write("Enter the user name: ");
    var username = Console.ReadLine();
    Console.Write("Enter the password: ");
    var password = Console.ReadLine();
    return new LoginRequestModel() { Username = username, Password = password };
}
