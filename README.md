> *Read this document in different languages : [Türkçe](https://github.com/yoldascevik/ARConsistency/blob/master/README.tr.md)*

# ARConsistency

![.NET Core](https://github.com/yoldascevik/ARConsistency/workflows/.NET%20Core/badge.svg?branch=master)
![Nuget](https://img.shields.io/nuget/v/arconsistency)
![GitHub](https://img.shields.io/github/license/yoldascevik/ARConsistency)

ARConsistency maintains response consistency by ensuring that all responses sent to the client for .Net Core Web Api projects are forwarded with the same template.

## Setup

#### 1. Install the Nuget Package

You can include this library in your project with the [Nuget package]([https://www.nuget.org/packages/ARConsistency/](https://www.nuget.org/packages/ARConsistency/)).

Add the nuget package to your project using *Package Manager Console* or *Nuget Package Manager*.

```powershell
PM> Install-Package ARConsistency
```
Or using .Net Cli

```powershell
> dotnet add package ARConsistency
```
#### 2. Settings

Add the following configuration to the "*appsettings.json*" file of your project.
```json
"ApiConsistency": {
  "IsDebug": true,
  "ShowStatusCode": true,
  "ShowApiVersion": false,
  "ApiVersion": "1.0",
  "IgnoreNullValue": true,
  "UseCamelCaseNaming": true,
  "EnableExceptionLogging": true
}
```
> **Note:** If you wish, you can assign these settings manually in the next step without reading them from the config file. **This step is optional**.

#### 3. Startup Implementation
Add the following into "*Startup.cs*".
```csharp
public void ConfigureServices(IServiceCollection services)
{
  // ...
  services.AddControllers()
      .AddApiResponseConsistency(options =>
      {
          Configuration.GetSection("ApiConsistency").Bind(options.ResponseOptions);
      });
}
```
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
  app.UseApiResponseConsistency();
  // ...
}
```

## Sample #1
```csharp
public IActionResult Get()
{
  return Ok(_astronauts.FirstOrDefault());
}
```
### Response
```json
{
  "statusCode":  200,
  "isError":  false,
  "payload":  {
    "Id":  1,
    "Name":  "Neil",
    "Surname":  "Armstrong"
  }
}
```
## Sample #2
```csharp
public IActionResult Get()
{
  return new ApiResponse("Request successful.", _astronauts.FirstOrDefault(), 200, "2.0");
}
```
### Response
```json
{
  "statusCode":  200,
  "version":  "2.0",
  "message":  "Request successful.",
  "isError":  false,
  "payload":  {
    "Id":  1,
    "Name":  "Neil",
    "Surname":  "Armstrong"
  }
}
```

## Response Types

ARConsistency works with three types of responses of its own. The basic information of these classes are as follows.

 Class Name		  |HTTP Status Code				      |		Description		
----------------|-----------------------------|----------------------
ApiResponse		  | 200 (OK)		  				      | It provides data return for successful response.						
ApiError		    | 400 (BadRequest)				    | Report any error to the client
ApiException	  | 500 (Internal Server Error)	| Throws errors and terminates the operation of the current procedure.

> In addition to these response types, ARConsistency also supports basic web api return types such as **Ok()**, **BadRequest()**. But **Ok()** return type contains only the data **ApiResponse** can contain information such as Message in addition to the returned data.

> The HTTP Status Codes in the table above are default values and can be changed during use.  

## Exception Handler
 
With the ARConsistency, you can use the **ApiException** response type to return an error with the HTTP status code you set.  

All types of exceptions handled except **ApiException** are converted to **ApiException** and returned to the client with 500 (**Internal Server Exception**) HTTP status code.
> The **Enable Exception Logging** setting must be turned on to support all types of exception. (see:Error Logging)

In custom exception types, you can use **ExceptionStatusCodeHandler** to return a different status code instead of 500.  

To do so, define **ExceptionStatusCodeHandler** in "*Startup.cs*".
```csharp
public void ConfigureServices(IServiceCollection services)
{
  // ...
  services.AddControllers()
      .AddApiResponseConsistency(options =>
      {
          // other options... (see:Startup Implemantation)
          options.ExceptionStatusCodeHandler.RegisterStatusCodedExceptionBaseType<IStatusCodedException>(type => type.StatusCode);
      });
}
```
> **IStatusCodedException** Interface must contain a status code property of type int, and the classes implementing this interface must derive from the System.Exception class. (Interface and status code property names can be freely determined)
#### Sample:

```csharp
public class ItemNotFoundException: Exception, IStatusCodedException
{
    public ItemNotFoundException(string message)
        : base(message)
    {
    }

    public int StatusCode => 400;
}
```

When the **ItemNotFoundException** exception is thrown, ARConsistency will convert this exception to ApiException and return the 400 status code specified in the error class.

## Exception Logging

ARConsistency has the ability to log errors it captures with the ILogger interface. This setting (**EnableExceptionLogging**) is enabled by default unless you change it.

While this setting is on, errors occurring in the pipeline are transmitted to the logging mechanism via the ILogger interface and the summary of the error message is returned to the user as **ApiExcepiton** type api response. If the **IsDebug** option is on, the details of the error message are also included in the response.

When logging is turned off, error messages captured are thrown without processing.

## Test Api Documentation

There is a .Net Core Web Api project in the repository where you can test the project. 
You can find the Postman documentation of the test api [here](https://documenter.getpostman.com/view/1473309/SzS5vS8p).
