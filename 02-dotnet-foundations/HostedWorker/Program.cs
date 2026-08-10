using HostedWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services
    .AddOptions<WorkerOptions>()
    .Bind(builder.Configuration.GetSection(WorkerOptions.SectionName))
    .Validate(options => !string.IsNullOrWhiteSpace(options.Message),
        "Worker:Message must not be empty.")
    .Validate(options => options.DelayMilliseconds is >= 100 and <= 60_000,
        "Worker:DelayMilliseconds must be between 100 and 60000.")
    .ValidateOnStart();

builder.Services.AddSingleton<SingletonId>();
builder.Services.AddScoped<ScopedId>();
builder.Services.AddTransient<TransientId>();

var host = builder.Build();
host.Run();
