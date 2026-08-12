using System.ComponentModel.DataAnnotations;

namespace ControllerApiSample.Models;

public sealed record CreateTaskRequest(
    [Required, StringLength(100)] string Title);
