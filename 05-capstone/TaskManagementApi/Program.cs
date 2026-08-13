using TaskManagementApi.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var tasks = new List<TaskSummary>
{
    new(1, "要件を確認する", true),
    new(2, "ページングを実装する", false),
    new(3, "テストを書く", false),
    new(4, "READMEを更新する", false),
    new(5, "変更をレビューする", false),
};

app.MapGet("/tasks", IResult (int page = 1, int pageSize = 20) =>
{
    var errors = new Dictionary<string, string[]>();

    if (page < 1)
    {
        errors[nameof(page)] = ["pageは1以上を指定してください。"];
    }

    if (pageSize is < 1 or > 100)
    {
        errors[nameof(pageSize)] = ["pageSizeは1以上100以下を指定してください。"];
    }

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var items = tasks
        .OrderBy(task => task.Id)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    return Results.Ok(new PagedResponse<TaskSummary>(
        items,
        page,
        pageSize,
        tasks.Count));
});

app.Run();

public partial class Program;
