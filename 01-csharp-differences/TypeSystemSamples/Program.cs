string? name = null;
Console.WriteLine(name?.Length ?? 0);

var classFirst = new PersonClass("Ada");
var classSecond = new PersonClass("Ada");

var recordFirst = new PersonRecord("Ada");
var recordSecond = new PersonRecord("Ada");

Console.WriteLine(classFirst == classSecond);
Console.WriteLine(recordFirst == recordSecond);
Console.WriteLine(recordFirst.Equals(recordSecond));

var structFirst = new PersonStruct("Ada");
var structSecond = structFirst;
structSecond.Name = "Grace";

var recordStructFirst = new PersonRecordStruct("Ada");
var recordStructSecond = recordStructFirst;
recordStructSecond.Name = "Grace";

Console.WriteLine(structFirst.Name);
Console.WriteLine(structSecond.Name);
Console.WriteLine(recordStructFirst.Name);
Console.WriteLine(recordStructSecond.Name);

Console.WriteLine(new PersonStruct("Ada").Equals(new PersonStruct("Ada")));
Console.WriteLine(new PersonRecordStruct("Ada").Equals(new PersonRecordStruct("Ada")));

// PropertySample
Console.WriteLine("[Property Sample]");

var autoPropertyPerson = new AutoPropertyPerson();
autoPropertyPerson.Name = "Ada";
Console.WriteLine(autoPropertyPerson.Name);

var initOnlyPerson = new InitOnlyPerson { Name = "Grace" };
Console.WriteLine(initOnlyPerson.Name);

var primaryConstructorPerson = new PrimaryConstructorPerson("Lin");
Console.WriteLine(primaryConstructorPerson.Name);

// ResourceSamples
Console.WriteLine("[Resource Sample]");

using (var syncResource = new SyncResource("sync"))
{
    syncResource.Use();
}

await using (var asyncResource = new AsyncResource("async"))
{
    asyncResource.Use();
}

// LINQ Sample
Console.WriteLine("[LINQ Sample]");

var tasks = new List<LearningTask>
{
    new("C#", "nullable reference types", true),
    new("C#", "LINQ", false),
    new(".NET", "DI", false),
    new(".NET", "Configuration", true),
};

var unfinishedTitles = tasks
    .Where(task =>
    {
        Console.WriteLine($"filtering: {task.Title}");
        return !task.IsDone;
    })
    .Select(task => task.Title);

Console.WriteLine("query created");

foreach (var title in unfinishedTitles)
{
    Console.WriteLine($"unfinished: {title}");
}

var tasksByCategory = tasks.GroupBy(task => task.Category);

foreach (var group in tasksByCategory)
{
    Console.WriteLine($"{group.Key}: {group.Count()} tasks");
}

var firstDotnetTask = tasks.FirstOrDefault(task => task.Category == ".NET");
Console.WriteLine(firstDotnetTask?.Title ?? "not found");

Console.WriteLine("[IEnumerable / IQueryable Sample]");

IEnumerable<LearningTask> enumerableTasks = tasks;
var enumerableQuery = enumerableTasks.Where(task => !task.IsDone);

IQueryable<LearningTask> queryableTasks = tasks.AsQueryable();
var queryableQuery = queryableTasks.Where(task => !task.IsDone);

Console.WriteLine(string.Join(", ", enumerableQuery.Select(task => task.Title)));
Console.WriteLine(string.Join(", ", queryableQuery.Select(task => task.Title)));
Console.WriteLine(queryableQuery.Expression);


Console.WriteLine("[Async Sample]");

var messageTask = AsyncSamples.ReadMessageAsync();
Console.WriteLine("task created");

var message = await messageTask;
Console.WriteLine(message);

Console.WriteLine("[Cancellation Sample]");

using var cancellationSource = new CancellationTokenSource();
cancellationSource.CancelAfter(TimeSpan.FromMilliseconds(100));

try
{
    var result = await AsyncSamples.WaitForMessageAsync(cancellationSource.Token);
    Console.WriteLine(result);
}
catch (OperationCanceledException)
{
    Console.WriteLine("cancelled");
}
