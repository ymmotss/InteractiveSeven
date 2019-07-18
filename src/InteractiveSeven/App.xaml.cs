﻿using InteractiveSeven.Core;
using InteractiveSeven.Core.Data;
using InteractiveSeven.Core.IntervalMessages;
using InteractiveSeven.Core.Memory;
using InteractiveSeven.Core.Models;
using InteractiveSeven.Core.Payments;
using InteractiveSeven.Core.Services;
using InteractiveSeven.Core.Settings;
using InteractiveSeven.Core.ViewModels;
using InteractiveSeven.Core.Workloads;
using InteractiveSeven.Services;
using InteractiveSeven.Startup;
using InteractiveSeven.Twitch;
using InteractiveSeven.Twitch.Commands;
using InteractiveSeven.Twitch.IntervalMessages;
using InteractiveSeven.Twitch.Payments;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Collections.Generic;
using System.Windows;
using InteractiveSeven.Core.Emitters;
using InteractiveSeven.Web.Hubs;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;

namespace InteractiveSeven
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private WorkloadCoordinator _workloadCoordinator;

        private IHost _host;

        private static void InitializeSettings()
        {
            new SettingsStore().EnsureExists();
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            InitializeSettings();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File("logs\\i7log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            _host = Host.CreateDefaultBuilder(e.Args)
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder.UseStartup<InteractiveSeven.Web.Startup>();
                    //webHostBuilder.UseContentRoot()
                })
                .ConfigureServices(ConfigureServices)
                .Build();

            _host.Start();

            var dataLoader = _host.Services.GetService<DataLoader>();
            dataLoader.LoadPreviousData();

            _workloadCoordinator = _host.Services.GetService<WorkloadCoordinator>();

            _host.Services.GetRequiredService<MainWindow>().Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(typeof(IList<>), typeof(List<>));

            services.AddSingleton<WorkloadCoordinator>();

            services.AddSingleton<MenuColorViewModel>();
            services.AddSingleton<NameBiddingViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<MainWindowViewModel>();

            services.AddSingleton<IMenuHubEmitter, MenuHubEmitter>();

            services.AddSingleton<IClock, SystemClock>();
            services.AddSingleton<IIntervalMessagingService, IntervalMessagingService>();
            services.AddSingleton<IEquipmentAccessor, EquipmentAccessor>();
            services.AddSingleton<IMemoryAccessor, MemoryAccessor>();
            services.AddSingleton<IGameMomentAccessor, GameMomentAccessor>();
            services.AddSingleton<IMenuColorAccessor, MenuColorAccessor>();
            services.AddSingleton<IGilAccessor, GilAccessor>();
            services.AddSingleton<IInventoryAccessor, InventoryAccessor>();
            services.AddSingleton<IMateriaAccessor, MateriaAccessor>();
            services.AddSingleton<INameAccessor, NameAccessor>();
            services.AddSingleton<IStatusAccessor, StatusAccessor>();
            services.AddSingleton<ITwitchClient, TwitchClient>();
            services.AddSingleton<IDialogService, DialogService>();

            services.RegisterEquipmentData();

            services.RegisterBattleCommand<StatusEffectCommand>();

            services.RegisterNonBattleCommand<WeaponCommand>();
            services.RegisterNonBattleCommand<ArmletCommand>();
            services.RegisterNonBattleCommand<AccessoryCommand>();
            services.RegisterNonBattleCommand<PauperCommand>();

            services.RegisterTwitchCommand<PaletteCommand>();
            services.RegisterTwitchCommand<RainbowCommand>();
            services.RegisterTwitchCommand<MakoCommand>();
            services.RegisterTwitchCommand<ItemCommand>();
            services.RegisterTwitchCommand<MateriaCommand>();
            services.RegisterTwitchCommand<CostsCommand>();
            services.RegisterTwitchCommand<GiveGilCommand>();
            services.RegisterTwitchCommand<NameBidsCommand>();
            services.RegisterTwitchCommand<MenuCommand>();
            services.RegisterTwitchCommand<NameCommand>();
            services.RegisterTwitchCommand<RefreshCommand>();
            services.RegisterTwitchCommand<BalanceCommand>();
            services.RegisterTwitchCommand<HelpCommand>();
            services.RegisterTwitchCommand<I7Command>();

            services.AddSingleton<IChatBot, ChatBot>();

            services.AddSingleton(typeof(IDataStore<>), typeof(FileDataStore<>));

            services.AddSingleton<DataLoader>();

            services.AddSingleton<ISettingsStore, SettingsStore>();
            services.AddSingleton<GilBank>();
            services.AddSingleton<PaymentProcessor>();
            services.AddSingleton<ColorPaletteCollection>();
            services.AddSingleton<MainWindow>();

            services.AddLogging(config =>
            {
                config.AddSerilog();
            });

            services.AddMvcCore();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            _host.Dispose();
        }
    }
}
