using Repository.Implementations;
using Repository.Interfaces;
using Repository.Libraries;
using Fleck;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System.Text;

// WebSocket server setup
List<IWebSocketConnection> clients = new List<IWebSocketConnection>();
object lockObject = new object();

// RabbitMQ setup
const string queueName = "UserRegistrations";
var factory = new ConnectionFactory()
{
    HostName = "localhost",
    UserName = "guest",
    Password = "guest"
};

using var rabbitConnection = factory.CreateConnection();
using var rabbitChannel = rabbitConnection.CreateModel();
rabbitChannel.QueueDeclare(queue: "UserRegistrations", durable: true, exclusive: false, autoDelete: false, arguments: null);

//rabbitChannel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);	

// Redis setup
// ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis-14974.c305.ap-south-1-1.ec2.redns.redis-cloud.com:14974,password=XfdMLTe9w0uaJ18VRsijz03amtGzpk9Z");
IDatabase redisDb = redis.GetDatabase();

// Start WebSocket server
var server = new WebSocketServer("ws://0.0.0.0:8181");
server.Start(socket =>
{
    socket.OnOpen = () =>
    {
        lock (lockObject)
        {
            clients.Add(socket); // Add client to the list
        }
        Console.WriteLine("Client connected");

        // Retrieve all stored messages and unseen count from Redis
        lock (lockObject)
        {
            var messages = redisDb.ListRange("notifications");

            // Check unseen count and default to "0" if it doesn't exist
            var unseenCountValue = redisDb.StringGet("unseenCount");
            var unseenCount = !unseenCountValue.IsNullOrEmpty ? (string)unseenCountValue : "0";

            // Send unseen count first
            socket.Send($"{{ \"unseenCount\": {unseenCount} }}");

            // Send all notifications
            foreach (var msg in messages)
            {
                socket.Send((string)msg);
            }
        }
    };

    socket.OnClose = () =>
    {
        lock (lockObject)
        {
            clients.Remove(socket); // Remove client from the list
        }
        Console.WriteLine("Client disconnected");
    };

    socket.OnMessage = message =>
    {
        Console.WriteLine($"Received message: {message}");
        // Optionally handle messages from WebSocket clients if needed
    };
});

// RabbitMQ consumer to handle user registrations
var consumer = new EventingBasicConsumer(rabbitChannel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received from RabbitMQ: {message}");

    lock (lockObject)
    {
        // Store message in Redis
        redisDb.ListRightPush("notifications", message);

        // Increment unseen notification count
        var unseenCountValue = redisDb.StringGet("unseenCount");
        int unseenCount = !unseenCountValue.IsNullOrEmpty ? int.Parse(unseenCountValue) : 0;
        unseenCount++;
        redisDb.StringSet("unseenCount", unseenCount.ToString());

        // Broadcast the message to all connected WebSocket clients
        foreach (var client in clients.ToList())
        {
            try
            {
                client.Send($"{{ \"unseenCount\": {unseenCount} }}");
                client.Send(message);
            }
            catch
            {
                clients.Remove(client); // Remove disconnected clients
            }
        }
    }
};
rabbitChannel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

//below Ayush code=====================

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(policy =>
{
    policy.AddPolicy("CorsPolicy", rules =>
    {
        rules.AllowAnyOrigin();
        rules.AllowAnyMethod();
        rules.AllowAnyHeader();

    });
});

builder.Services.AddSingleton<IAccountRepository, AccountRepository>(provider =>
{
    IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
    return new AccountRepository(configuration);
});

builder.Services.AddSingleton(provider =>
{
    IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
    return new MailerService(configuration);
});

builder.Services.AddSingleton(provider =>
{
    IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
    return new PropertyRepository(configuration);
});

builder.Services.AddSingleton<IPropertyRepository, PropertyRepository>(provider =>
{
    IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
    return new PropertyRepository(configuration);
});

builder.Services.AddSingleton<IProjectRepository, ProjectRepository>(provider =>
{
    IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
    return new ProjectRepository(configuration);
});

builder.Services.AddSingleton(provider =>
{
    IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
    return new ProjectRepository(configuration);
});

builder.Services.AddSingleton<IReviewsRepository, ReviewsRepository>(provider =>
{
    IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
    return new ReviewsRepository(configuration);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.MapControllers();

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.Run();
