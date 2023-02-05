using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Security.Claims;
using Adverthouse.Core.Infrastructure;
using Adverthouse.Core.Configuration;
using back_end.Swagger;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using back_end.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
 

builder.Services.AddInfrastructure(builder.Configuration); 

builder.Services.AddCors();
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);

var _appSettings = Singleton<AppSettings>.Instance;
var key = Encoding.ASCII.GetBytes(_appSettings.AdditionalSettings["APISecurityKey"].Value<string>());

builder.Services.AddAuthentication(x =>
{
    x.DefaultScheme = "mainAuth";
    x.DefaultSignInScheme = "mainAuth";
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;                
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
        ClockSkew = TimeSpan.Zero
    };
}).AddCookie("mainAuth", "mainAuth",options => {
    options.Cookie.Name = "mainAuth";
    options.LoginPath = new PathString("/QCManage");
    options.ExpireTimeSpan = TimeSpan.FromMinutes(14400);
    options.AccessDeniedPath = "/QCManage/Error/UnAuthorized";
    options.LogoutPath = "/QCManage/Home/Logout";
});

builder.Services.AddSwaggerGen();

/* Versioning */
builder.Services.AddApiVersioning(options =>  
{  
    options.DefaultApiVersion = new ApiVersion(1,0);  
    options.AssumeDefaultVersionWhenUnspecified = true;  
    options.ReportApiVersions = true;  
});  

builder.Services.AddVersionedApiExplorer(options =>  
{  
    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service  
    // note: the specified format code will format the version as "'v'major[.minor][-status]"  
    options.GroupNameFormat = "'v'VVV";  

    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat  
    // can also be used to control the format of the API version in route templates  
    options.SubstituteApiVersionInUrl = true;  
});  
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();  
builder.Services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());  

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

 
app.UseRouting();

app.UseSwagger();    

using (var scope = app.Services.CreateScope())
{           
    
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    context.SeedData();       
}

using (var scope = app.Services.CreateScope())
{           
    
    var provider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
     app.UseSwaggerUI(  
        options =>  
        {  
            // build a swagger endpoint for each discovered API version  
            foreach (var description in provider.ApiVersionDescriptions)  
            {  
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());  
            }  
        });  
}

// global cors policy
app.UseCors(x => x
    .SetIsOriginAllowed(origin => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

app.UseCookiePolicy();

app.Use(async (context, next) =>
{
    var principal = new ClaimsPrincipal();

    var result1 = await context.AuthenticateAsync("mainAuth");
    if (result1?.Principal != null)
    {
        principal.AddIdentities(result1.Principal.Identities);
        context.User = principal;
    }                               

    await next();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
