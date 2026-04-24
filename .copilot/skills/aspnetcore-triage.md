# ASP.NET Core Issue Triage

Triage an issue from the dotnet/aspnetcore GitHub repository by reading it, understanding the bug or feature request, scaffolding a repro project, writing code that demonstrates the issue, and committing it.

## Trigger

Use this skill when the user asks to "triage" an aspnetcore issue, provides a dotnet/aspnetcore issue number, or provides a GitHub URL to a dotnet/aspnetcore issue.

## Instructions

### Step 1: Read the issue

Use the GitHub MCP tools to read the issue from `dotnet/aspnetcore`. Extract:

- The issue number
- The title and description
- Any code snippets, repro steps, or expected vs actual behavior
- What ASP.NET Core area is involved (minimal APIs, MVC/controllers, Blazor, SignalR, gRPC, Razor Pages, middleware, authentication, OpenAPI, etc.)

### Step 2: Pick the right project template

Based on the issue's area, choose the appropriate `dotnet new` template:

| Area | Template | Notes |
|------|----------|-------|
| Minimal APIs | `webapi` | Default for most API issues |
| OpenAPI / Swashbuckle / OpenApi | `webapi` | |
| MVC / Controllers | `webapi --use-controllers` | Scaffolds with controllers |
| Blazor | `blazor` | |
| Razor Pages | `webapp` | |
| gRPC | `grpc` | |
| General / middleware / other | `web` | Bare-bones empty project |

If unsure, default to `webapi`.

### Step 3: Scaffold the project

Run the following commands from the repository root:

```bash
dotnet new <template> -o issue-<number>
```

Where `<number>` is the issue number and `<template>` is from Step 2.

### Step 4: Fix line endings

Strip Windows carriage returns from all generated files:

```bash
cd issue-<number>
find . -type f | grep -v 'obj/' | grep -v 'bin/' | grep -v .git | while read f; do sed -i '' 's/\r//' "$f"; done
cd ..
```

### Step 5: Write the repro code

Modify the scaffolded project to demonstrate the issue:

- Edit `Program.cs` (and other files as needed) to reproduce the bug or demonstrate the requested behavior.
- The code should be minimal — only include what's needed to show the issue.
- Add comments in the code explaining what the expected vs actual behavior is.
- If the issue includes code snippets, incorporate them into the repro.
- If the issue is about OpenAPI, keep the `app.MapOpenApi()` call and add endpoints that demonstrate the problem.
- Create or update the `.http` file (`issue-<number>.http`) with sample requests that exercise the repro endpoints.

### Step 6: Write a README

Create a `README.md` in the `issue-<number>/` directory with the following sections:

- **Title**: `Issue <number>: <issue title>`
- **Problem**: A clear description of the bug or feature request, in your own words.
- **Repro**: How to run the repro project and what to observe (include a table of requests/expected/actual if applicable).
- **Root Cause**: Your analysis of why the issue occurs — trace it to the relevant code paths or architectural decisions. Link to related issues if you find any.
- **Analysis of Potential Solutions**: Enumerate 2–4 potential approaches to fix the issue. For each, describe the approach, list pros (✅) and cons (❌), and end with a recommendation.

Search for related issues in dotnet/aspnetcore to provide context and cross-references.

### Step 7: Build and verify

Run `dotnet build` on the project to ensure it compiles:

```bash
dotnet build issue-<number>/issue-<number>.csproj
```

Fix any compilation errors before proceeding.

### Step 8: Git commit

```bash
git add issue-<number>
git commit -m "Add repro for dotnet/aspnetcore#<number>"
```

### Output

After completing all steps, provide a brief summary:

1. **Issue**: Title and one-line summary of the bug/request
2. **Template used**: Which `dotnet new` template was chosen and why
3. **Repro approach**: What the repro code does to demonstrate the issue
4. **How to run**: Instructions to run the repro and observe the behavior (e.g., `dotnet run --project issue-<number>` and what URL/request to hit)
