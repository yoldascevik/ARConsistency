> *Bu dokümanı farklı dillerde okuyun : [English](https://github.com/yoldascevik/ARConsistency/blob/master/README.md)*

# ARConsistency

![.NET Core](https://github.com/yoldascevik/ARConsistency/workflows/.NET%20Core/badge.svg?branch=master)
![Nuget](https://img.shields.io/nuget/v/arconsistency)
![GitHub](https://img.shields.io/github/license/yoldascevik/ARConsistency)

ARConsistency .Net Core Web Api projeleri için istemciye gönderilen tüm yanıtların aynı şablon ile iletilmesini sağlayarak yanıt tutarlılığını korur. 


## Kurulum

#### 1. Nuget Paketini yükleme

Bu kütühaneyi [Nuget Paketi]([https://www.nuget.org/packages/ARConsistency/](https://www.nuget.org/packages/ARConsistency/)) ile projenize dahil edebilirsiniz. 

*Package Manager Console* ya da *Nuget Package Manager* kullanarak nuget paketini projenize ekleyin.

```powershell
PM> Install-Package ARConsistency
```
Ya da .Net Cli ile

```powershell
> dotnet add package ARConsistency
```
#### 2. Settings

Projenizin "*appsettings.json*" dosyasına aşağıdaki konfigurasyonu ekleyin.
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
> **Not:** Dilerseniz bu ayarları config dosyasından okumadan bir sonraki aşamada el ile atayabilirsiniz. Bu adım isteğe bağlıdır.

#### 3. Startup İmplementasyon
"*Startup.cs*" içerisine aşağıdaki eklemeyi yapın.
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

## Örnek #1
```csharp
public IActionResult Get()
{
  return Ok(_astronauts.FirstOrDefault());
}
```
### Yanıt
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
## Örnek #2
```csharp
public IActionResult Get()
{
  return new ApiResponse("Request successful.", _astronauts.FirstOrDefault(), 200, "2.0");
}
```
### Yanıt
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

## Yanıt Türleri

ARConsistency kendisine ait üç yanıt türüyle çalışır. Bu sınıflara ait temel bilgiler aşağıdaki gibidir.

 Sınıf Adı		|HTTP Status Code				|		Açıklama		
----------------|-------------------------------|----------------------
ApiResponse		| 200 (OK)		  				| Başarılı yanıt için veri dönmeyi sağlar.						
ApiError		| 400 (BadRequest)				| Yakalanan herhangi bir hatanın istemciye gönderilmesini sağlar.
ApiException	| 500 (Internal Server Error)	| Hata fırlatır ve geçerli yordamın işleyişini sonlandırır.

> ARConsistency, bu yanıt türlerine ek olarak **Ok()**, **BadRequest()** gibi temel web api dönüş türlerini de destekler. Fakat **Ok()** dönüş tipi yalnızca veriyi içerirken **ApiResponse** dönen dataya ek Mesaj gibi bilgileri içinde barındırabilir.

> Yukarıdaki tabloda bulunan HTTP Status Code' lar varsayılan değerlerdir ve kullanım sırasında değiştirilebilir.

## Hata Yakalama

ARConsistency ile **ApiException** yanıt tipini kullanarak bir hatanın belirlemiş olduğunuz HTTP durum koduyla birlikte dönmesini sağlayabilirsiniz.  

**ApiException** dışında yakalanan tüm hata türleri **ApiException**'a çevrilerek serialize edilir ve 500 (**Internal Server Exception**) HTTP durum koduyla istemciye döndürülür.
> Tüm hata türlerini desteklemek için **EnableExceptionLogging** ayarının açık olması gerekmektedir. (bkz:Hata Loglama)

Özel hata tiplerinde 500 yerine farklı bir durum kodu dönmek için **ExceptionStatusCodeHandler** kullanabilirsiniz.  

Bunun için "*Startup.cs*" içerisinde **ExceptionStatusCodeHandler** tanımlamasını yapın.
```csharp
public void ConfigureServices(IServiceCollection services)
{
  // ...
  services.AddControllers()
      .AddApiResponseConsistency(options =>
      {
          // diğer ayarlar... (bkz:Startup İmplementasyon)
          options.ExceptionStatusCodeHandler.RegisterStatusCodedExceptionBaseType<IStatusCodedException>(type => type.StatusCode);
      });
}
```
> **IStatusCodedException** Interface' i int türünde bir status code özelliği içermelidir ve bu interface i uygulayan sınıflar System.Exception sınıfından türemelidir. (Interface ve status code property isimleri serbest şekilde belirlenebilir)

#### Örnek:

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

**ItemNotFoundException** hatası fırlatıldığında ARConsistency bu hatayı ApiException a dönüştürerek hata sınıfı içinde belirtilen 400 durum kodunu dönecektir.

## Hata Loglama

ARConsistency, ILogger arayüzü ile yakaladığı hataları loglama kabiliyetine sahiptir. Bu ayar (**EnableExceptionLogging**) siz değiştirmediğiniz sürece varsayılan olarak aktiftir.

Bu ayar açık iken pipeline de oluşan hatalar ILogger arayüzü ile loglama mekanizmasına iletilir ve hata mesajının özeti kullanıcıya **ApiExcepiton** türünde api response olarak döndürülür. **IsDebug** özelliği açıksa hata mesajına ait detaylar da yanıta dahil edilir. 

Loglama kapalı olduğunda yakalanan hata mesajları işlenmeden fırlatılır.

## Test Projesi Dökümantasyonu

Repository içersinde projeyi test edebileceğiniz bir .Net Core Web Api projesi bulunmaktadır. 
Test apisinin Postman dokümantasyonuna [buradan](https://documenter.getpostman.com/view/1473309/SzS5vS8p) ulaşabilirsiniz.

## Katkı Sağlama

Katkıda bulunmak için lütfen [CONTRIBUTING.md](https://github.com/yoldascevik/ARConsistency/blob/master/CONTRIBUTING.md) dosyasına göz atın.
