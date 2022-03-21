using CompanionPlugin.Classes.Models;
using CompanionPlugin.Interfaces;

using Json.Schema.Generation;

namespace CryingHomunculusPlugin.Classes;

public class HomunculusServiceConfig : ServiceSettings, ICommandServiceSettings
{
    [Title("Таймаут команд")]
    public int Timeout { get; set; }
    [Title("Путь к аудиофайлам")]
    public string AudioPath { get; set; }
    [Title("Громкость")]
    [ExclusiveMinimum(0)]
    [ExclusiveMaximum(1)]
    public float Volume { get; set; }
    [Title("Настройки речи")]
    public SpeechSettings Speech { get; set; } = new();
    [Title("Голосовые команды")]
    public List<VoiceCommand> Commands { get; set; } = new();
    [Title("Ограничение для команд")]
    public List<CommandConstraints> CommandConstraints { get; set; } = new();
}