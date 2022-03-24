using CompanionPlugin.Classes.Models;
using CompanionPlugin.Interfaces;

using Json.Schema.Generation;

namespace InformerPlugin.Classes;

public class InformerServiceConfig : ServiceSettings, ICommandServiceSettings
{
    [Title("Список информационных сообщений")]
    public List<InformerMessage> Messages { get; set; } = new();
    [Title("Ограничение для команд")]
    public List<CommandConstraints> CommandConstraints { get; set; } = new();
}