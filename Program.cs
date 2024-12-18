using System.Text.Json;
using System.Xml.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(option=>{
option.SerializerOptions.PropertyNamingPolicy= JsonNamingPolicy.KebabCaseLower;
});
var app = builder.Build();

var person= new Person {UserName="manoj", Age=22 };

app.MapGet("/", () => "Hello World!");

app.MapGet("/manual-json", ()=>{
    var jsonString= JsonSerializer.Serialize(person);
    return TypedResults.Text(jsonString, "application/json");
});

app.MapGet("/custom-searializer", ()=>{
   var options= new JsonSerializerOptions{
    PropertyNamingPolicy= JsonNamingPolicy.SnakeCaseLower
   };
   var customJsonString= JsonSerializer.Serialize(person, options);
   return TypedResults.Text(customJsonString, "application/json");
});

app.MapGet("/json", ()=>{
    return TypedResults.Json(person);
});

app.MapGet("/auto", ()=>{
    return person;
});

app.MapGet("/xml", ()=>{
    var XmlSerializer= new  XmlSerializer(typeof(Person));
    var stringWriter= new StringWriter();
    XmlSerializer.Serialize(stringWriter, person);
    var xmlOutPut= stringWriter.ToString();
    return TypedResults.Text(xmlOutPut, "application/xml");
});


app.Run();

public class Person{
    required public string UserName{get;set;}
    required public int Age{get;set;}


}
