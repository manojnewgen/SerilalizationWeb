using System.Text.Json;
using System.Xml.Serialization;
using Swashbuckle.AspNetCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.ConfigureHttpJsonOptions(option =>
        {
            option.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower;
        });
        var app = builder.Build();

        var person = new Person { UserName = "manoj", Age = 22 };

        List<Person> personList = new List<Person>{
    new Person {UserName="Manoj", Age=21}
    ,new Person {UserName="Rahul", Age=22}
    ,new Person {UserName="Shivani", Age=30}
    ,new Person {UserName="Suraj", Age=30}
    };


        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapGet("/", () => "Hello World!");

        app.MapGet("/person", Results<Ok<Person>,NotFound> (string name) =>
        {
            if(!string.IsNullOrWhiteSpace(name)){
                var person=personList.Find(x => x.UserName == name);
                if(person!=null)
                return TypedResults.Ok<Person>(person); 
                else
                 return TypedResults.NotFound();

            }
            return TypedResults.NotFound();

        }).WithOpenApi(operations=> {
            operations.Parameters[0].Description="";
            operations.Summary="Get a single record based on user name";
            operations.Description="returns a single values";
            return operations;
        });

        app.MapGet("/manual-json", () =>
        {
            var jsonString = JsonSerializer.Serialize(person);
            return TypedResults.Text(jsonString, "application/json");
        });

        app.MapGet("/custom-searializer", () =>
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };
            var customJsonString = JsonSerializer.Serialize(person, options);
            return TypedResults.Text(customJsonString, "application/json");
        });

        app.MapGet("/json", () =>
        {
            return TypedResults.Json(person);
        });

        app.MapGet("/auto", () =>
        {
            return person;
        });

        app.MapGet("/xml", () =>
        {
            var XmlSerializer = new XmlSerializer(typeof(Person));
            var stringWriter = new StringWriter();
            XmlSerializer.Serialize(stringWriter, person);
            var xmlOutPut = stringWriter.ToString();
            return TypedResults.Text(xmlOutPut, "application/xml");
        });


        app.Run();
    }
}

public class Person{
    required public string UserName{get;set;}
    required public int Age{get;set;}


}
