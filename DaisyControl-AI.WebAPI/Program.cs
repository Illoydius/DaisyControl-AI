using DaisyControl_AI.Core.Comms.Discord;
using DaisyControl_AI.Core.Comms.Discord.Commands;
using DaisyControl_AI.Core.Comms.Discord.UserMessages;
namespace DaisyControl_AI.WebAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            Services.ConfigureServices(builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            // TODO: put this into a bg worker
            DiscordWorker discordWorker = new DiscordWorker(new DiscordBotCommandHandler(), new DiscordBotUserMessageHandler());

            app.Run();
        }
    }
}