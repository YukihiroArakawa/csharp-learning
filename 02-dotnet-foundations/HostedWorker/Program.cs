using HostedWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton<SingletonId>();
builder.Services.AddScoped<ScopedId>();
builder.Services.AddTransient<TransientId>();

var host = builder.Build();
host.Run();
