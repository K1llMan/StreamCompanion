using Json.Schema.Generation;

namespace CryingHomunculusPlugin.Classes;

public class VoiceCommand
{
    [Title("Алиасы для команды")]
    public string[] Aliases { get; set; }
    [Title("Описание")]
    public string Description { get; set; }
    [Title("Путь к аудиофайлу относительно AudioPath")]
    public string FilePath { get; set; }
}