﻿namespace CompanionPlugin.Interfaces;

public interface ICommandSourceServiceSettings : IServiceSettings
{
    public bool SubscribeToEvents { get; set; }
}