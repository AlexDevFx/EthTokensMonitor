using System.Threading.Channels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Nethereum.Metamask;
using Nethereum.Metamask.Blazor;
using Nethereum.Siwe;
using Nethereum.UI;
using TokensMonitor.Authentication;
using TokensMonitor.Configuration;
using TokensMonitor.Errors;
using TokensMonitor.Notifications;
using TokensMonitor.Wallet;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(config => config.AddConsole());
builder.Services.Configure<MonitorAppConfig>(builder.Configuration,
    opt => { opt.BindNonPublicProperties = true; });

builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opt =>
    {
        opt.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if(context.Request.Query.ContainsKey("access_token"))
                {
                    string? token = context.Request.Query["access_token"][0];

                    if (!string.IsNullOrEmpty(token))
                    {
                        if(!context.Request.Headers.ContainsKey(HeaderNames.Authorization))
                            context.Request.Headers.Add(HeaderNames.Authorization, $"Bearer {token}");
                    }
                }
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                context.HttpContext.Response.Redirect("/");
                return Task.CompletedTask;
            }
        };
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = KeyBuilder.CreateKey(builder.Configuration["Auth:SecretKey"])
        };
    });

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

builder.Services.AddScoped<IMetamaskInterop, MetamaskBlazorInterop>();
builder.Services.AddScoped<MetamaskHostProvider>();
builder.Services.AddScoped(services =>
{
    var metamaskHostProvider = services.GetService<MetamaskHostProvider>();
    var selectedHostProvider = new SelectedEthereumHostProviderService();
    selectedHostProvider.SetSelectedEthereumHostProvider(metamaskHostProvider);
    return selectedHostProvider;
});
builder.Services.AddScoped<IEthereumHostProvider, MetamaskHostProvider>();
builder.Services.AddScoped<ISessionStorage, InMemorySessionNonceStorage>();
builder.Services.AddScoped<NethereumSiweAuthenticatorService>();
builder.Services.AddScoped<AuthMessageService>();
builder.Services.AddScoped<IUserContext, UserContext>();

builder.Services.AddScoped(sp => new SiweMessageService(sp.GetRequiredService<ISessionStorage>(), null, null));
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<TokensService>();
builder.Services.AddScoped<WalletService>();
builder.Services.AddSingleton<MonitorScheduler>();

builder.Services.AddHostedService<TokensWatcher>();
builder.Services.AddHostedService<NotificationsSender>();

var notificationsChannel = Channel.CreateBounded<NotificationRequest>(new BoundedChannelOptions(1000)
{
    FullMode = BoundedChannelFullMode.Wait
});
builder.Services.AddSingleton(notificationsChannel.Reader);
builder.Services.AddSingleton(notificationsChannel.Writer);

var tokensWatcherChannel = Channel.CreateBounded<TokensWatcherTask>(new BoundedChannelOptions(1000)
{
    FullMode = BoundedChannelFullMode.Wait,
    SingleReader = true
});

builder.Services.AddSingleton(tokensWatcherChannel.Reader);
builder.Services.AddSingleton(tokensWatcherChannel.Writer);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseMiddleware<AuthenticationMiddleware>();

app.MapRazorPages();
app.MapControllers();
app.MapHub<WalletMonitorHub>("/wallet-monitor");

app.Run();