using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi( );
var app = builder.Build( );

// Configure the HTTP request pipeline.
if( app.Environment.IsDevelopment( ) ) {
    app.MapOpenApi( );
}
app.UseHttpsRedirection( );

app.MapPut("/test",
           (MsgMovimentacaoLocaisTrabalho msg) => Results.Ok( ))
   .WithName("GetWeatherForecast");

app.Run( );


public class MsgMovimentacaoLocaisTrabalho {
    public IList<InfoGeral>? LocaisRemover { get; set; }
    public InfoGeral? LocaisAssociar { get; set; }
}

public class InfoGeral {
    public Guid GuidDirecao { get; set; }
    public IEnumerable<int> Locais { get; set; } = Enumerable.Empty<int>();
}