using AITechDATA.DataLayer;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.DataLayer.Services;
using AITechDATA.Tools;
using AITechWebAPI.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MTPermissionCenter.Abstractions;
using MTPermissionCenter.AspNetCore;
using MTPermissionCenter.EFCore;
using Parbad.Builder;
using Parbad.Gateway.ParbadVirtual;
using Parbad.Gateway.ZarinPal;
using Repositories;
using Services;
using System;
using System.Text;
using System.Text.Json;

namespace AITechWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var corsPolicy = builder.Configuration["cors:policy"].ToString();
            var cookiesecurity = builder.Configuration["cors:cookiesecurity"].ToString();

            var allowedOrigins = builder.Configuration.GetSection("cors:allowedOrigins").Get<List<string>>().ToArray();

            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

            builder.Services.AddDistributedMemoryCache();
            if (cookiesecurity == "default")
            {
                builder.Services.AddSession();
            }
            else
            {
                builder.Services.AddSession(options =>
                {
                    options.Cookie.HttpOnly = true; // ????? ????? ???? ???????
                    options.Cookie.IsEssential = true; // ????? ???? ???? ???? ?????? Session
                    options.Cookie.SameSite = SameSiteMode.None;  // ????? ????? ??????? ?? ??????????? cross-origin
                    options.Cookie.SecurePolicy = (CookieSecurePolicy)int.Parse(cookiesecurity);  // ??? HTTPS ???? ???
                });
            }


            builder.Services.AddCors(options =>
            {

                if (corsPolicy.ToLower().Contains("allowall"))
                {
                    options.AddPolicy(corsPolicy, builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .WithExposedHeaders("Set-Cookie");

                    });
                }
                else
                {
                    options.AddPolicy(corsPolicy, builder =>
                    builder.WithOrigins(allowedOrigins) // اضافه کردن localhost و آی‌پی لوکال
                           .AllowCredentials()
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                             .WithExposedHeaders("Set-Cookie"));

                }

            });


            // Add services to the container.

            var apiVersion = ToolBox.CalculateAppVersionNo();
            var apiTitle = builder.Environment.ApplicationName;

            #region PaymentGetWay

            builder.Services.AddParbad()
                    .ConfigureGateways(gateways =>
                    {
    
                        gateways
                               .AddZarinPal()
                               .WithAccounts(accounts =>
                               {
                                   accounts.AddInMemory(account =>
                                   {
                                       account.MerchantId = "b2b8419a-2de7-42f5-b44f-67d1e6c18aba";
                                       account.IsSandbox = false;
                                   });
                               });
                    })
                     .ConfigureHttpContext(httpContextBuilder => httpContextBuilder.UseDefaultAspNetCore())
                   .ConfigureStorage(storageBuilder => storageBuilder.UseMemoryCache());

            #endregion


            builder.Services.AddControllers(options =>
            {
                //options.OutputFormatters.Add()
                options.ReturnHttpNotAcceptable = true;
            })
              .AddNewtonsoftJson(options =>
              {
                  options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
              })
             .AddXmlDataContractSerializerFormatters();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();


            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = apiTitle,
                    Version = apiVersion
                });

                // Configure Swagger to use JWT authentication
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Please enter your JWT with Bearer into the field",

                    //Reference = new OpenApiReference
                    //{
                    //    Id = JwtBearerDefaults.AuthenticationScheme,
                    //    Type = ReferenceType.SecurityScheme
                    //}
                };

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);


                c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = new List<string>()
                });

            });

            var configHelper = new ConfigurationHelper();

            builder.Services.AddDbContext<AITechContext>(options =>
                options.UseSqlServer(configHelper.GetConnectionString("publicdb")));

            // MTPermissionCenter (AspNetCore)
            builder.Services.AddMTPermissionCenter();


            #region ImportDbServices

            builder.Services.AddScoped<IAddressRep, AddressRep>();
            builder.Services.AddScoped<IAdminReportRep, AdminReportRep>();
            builder.Services.AddScoped<IAssignmentRep, AssignmentRep>();
            builder.Services.AddScoped<IAwardRep, AwardRep>();
            builder.Services.AddScoped<IAttendanceRep, AttendanceRep>();
            builder.Services.AddScoped<ICategoryRep, CategoryRep>();
            builder.Services.AddScoped<ICityRep, CityRep>();
            builder.Services.AddScoped<ICourseRep, CourseRep>();
            builder.Services.AddScoped<IEventRep, EventRep>();
            builder.Services.AddScoped<IEntityScoreRep, EntityScoreRep>();
            builder.Services.AddScoped<IFileUploadRep, FileUploadRep>();
            builder.Services.AddScoped<IGroupRep, GroupRep>();
            builder.Services.AddScoped<IImageRep, ImageRep>();
            builder.Services.AddScoped<ILoginMethodRep, LoginMethodRep>();
            builder.Services.AddScoped<ILogRep, LogRep>();
            builder.Services.AddScoped<INewsRep, NewsRep>();
            builder.Services.AddScoped<IMeetingRep, MeetingRep>();
            builder.Services.AddScoped<IMinutesRep, MinutesRep>();
            builder.Services.AddScoped<INotificationRep, NotificationRep>();
            builder.Services.AddScoped<IParentRep, ParentRep>();
            builder.Services.AddScoped<IPaymentHistoryRep, PaymentHistoryRep>();
            builder.Services.AddScoped<IPermissionRep, PermissionRep>();
            builder.Services.AddScoped<IPermissionRoleRep, PermissionRoleRep>();
            builder.Services.AddScoped<IUserPermissionRep, UserPermissionRep>();
            builder.Services.AddScoped<IPreRegistrationRep, PreRegistrationRep>();
            builder.Services.AddScoped<IRoleRep, RoleRep>();
            builder.Services.AddScoped<ISessionAssignmentRep, SessionAssignmentRep>();
            builder.Services.AddScoped<ISessionRep, SessionRep>();
            builder.Services.AddScoped<ISettingRep, SettingRep>();
            builder.Services.AddScoped<IStudentDetailsRep, StudentDetailsRep>();
            builder.Services.AddScoped<ITeacherResumeRep, TeacherResumeRep>();
            builder.Services.AddScoped<ITicketMessageRep, TicketMessageRep>();
            builder.Services.AddScoped<ITicketRep, TicketRep>();
            builder.Services.AddScoped<ITokenRep, TokenRep>();
            builder.Services.AddScoped<IUserCourseRep, UserCourseRep>();
            builder.Services.AddScoped<IUserGroupRep, UserGroupRep>();
            builder.Services.AddScoped<IUserRep, UserRep>();
            builder.Services.AddScoped<ILinkedEntityRep, LinkedEntityRep>();
            builder.Services.AddScoped<IJobRequestRep, JobRequestRep>();
            builder.Services.AddScoped<IArticleRep, ArticleRep>();
            builder.Services.AddScoped<IBookRep, BookRep>();
            builder.Services.AddScoped<ISMSMessageRep, SMSMessageRep>();
            builder.Services.AddScoped<IInterviewTimeRep, InterviewTimeRep>();
            builder.Services.AddScoped<IEducationalBackgroundRep, EducationalBackgroundRep>();
            builder.Services.AddScoped<IClassGradeRep, ClassGradeRep>();
            builder.Services.AddScoped<ICommentRep, CommentRep>();
            builder.Services.AddScoped<IGroupChatMessageRep, GroupChatMessageRep>();
            builder.Services.AddScoped<IGroupChatReadStateRep, GroupChatReadStateRep>();
            builder.Services.AddScoped<IUserRoleProvider, UserRep>();
            builder.Services.AddScoped<IGroupChatReadStateRep, GroupChatReadStateRep>();
            builder.Services.AddScoped<ISubmitFormRep, SubmitFormRep>();
            builder.Services.AddScoped<IFormFieldRep, FormFieldRep>();
            builder.Services.AddScoped<IFieldInFormRep, FieldInFormRep>();
            builder.Services.AddScoped<IDiscountRep, DiscountRep>();
            builder.Services.AddScoped<IDiscountTargetRep, DiscountTargetRep>();
            builder.Services.AddScoped<IDutyRep, DutyRep>();
            builder.Services.AddScoped<IDismissalRep, DismissalRep>();
            builder.Services.AddScoped<IGadgetAccessRep, GadgetAccessRep>();


            #endregion ImportDbServices


            #region ImportMTPermissionCenterServices

            builder.Services.AddScoped<IPermissionInvalidationService, PermissionInvalidationService>();
            builder.Services.AddScoped<IPermissionService, EfPermissionService<AITechContext>>();

            #endregion ImportMTPermissionCenterServices


            builder.Services.AddTransient<ExceptionHandlingMiddleware>();



            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
  .AddJwtBearer(options =>
  {
      options.SaveToken = true;
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = builder.Configuration["Jwt:Issuer"],
          ValidAudience = builder.Configuration["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(key)
      };

      options.Events = new JwtBearerEvents
      {
          OnMessageReceived = context =>
          {
              var accessToken = context.Request.Query["access_token"];

              // مسیر هاب شما
              var path = context.HttpContext.Request.Path;
              if (!string.IsNullOrEmpty(accessToken) &&
                  path.StartsWithSegments("/hubs/group-chat"))
              {
                  context.Token = accessToken;
              }

              return Task.CompletedTask;
          }
      };
  });

            builder.Services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
            })
                .AddJsonProtocol(o =>
            {
                o.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                o.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
            }); ;


            builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            #region Pipeline

            app.UseStaticFiles();

            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{apiTitle} {apiVersion}");
                c.RoutePrefix = string.Empty; // روت اصلی سایت برای Swagger
                c.InjectJavascript("/js/swagger-token.js");
            });

            //}
            app.UseHttpsRedirection();


            app.UseCors(corsPolicy);


            app.UseSession();



            app.UseRouting();

            app.UseMiddleware<ExceptionHandlingMiddleware>();


            app.UseAuthentication();
            app.UseAuthorization();

            // IMPORTANT: after authentication
            app.UseMTPermissionCenter();

            app.MapHub<GroupChatHub>("/hubs/group-chat");

            //   app.UseParbadVirtualGateway();

            //Controller/Action/Id?
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            #endregion Pipeline

            app.Run();
        }
    }
}
