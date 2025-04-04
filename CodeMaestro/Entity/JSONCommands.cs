using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

// Определение структуры команды, соответствующей JSON схеме.
    // Парсер JSON команд
    public static class JsonCommandParser {
    /// <summary>
    /// Десериализует входной JSON и возвращает список команд.
    /// </summary>
    /// <param name="json">JSON строка, содержащая команды.</param>
    /// <returns>Список десериализованных команд.</returns>
    public static List<JsonCommand> ParseCommands(string json) {
        int startIndex = json.IndexOf('{');
        if (startIndex > 0) {
            json = json.Substring(startIndex);
        }

        var options = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonCommandConverter() }
        };

        try {
            CommandContainer container = JsonSerializer.Deserialize<CommandContainer>(json, options);
            return container?.Commands ?? new List<JsonCommand>();
        }
        catch (Exception ex) {
            throw new Exception("Error parsing JSON commands: " + ex.Message, ex);
        }
    }
}
public class JsonCommandConverter : JsonConverterFactory {
    private static readonly Dictionary<string, Type> CommandTypes = new()
    {
        { "LIST", typeof(ListCommand) },
        { "READ", typeof(ReadCommand) },
        { "WRITE", typeof(WriteCommand) },
        { "RUNCODE", typeof(RunCodeCommand) },
        { "TRYCOMPILE", typeof(TryCompileCommand) }
    };

    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(JsonCommand);

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
        return (JsonConverter)Activator.CreateInstance(typeof(CommandJsonConverterInner), CommandTypes)!;
    }

    private class CommandJsonConverterInner : JsonConverter<JsonCommand> {
        private readonly Dictionary<string, Type> _commandTypes;

        public CommandJsonConverterInner(Dictionary<string, Type> commandTypes) {
            _commandTypes = commandTypes;
        }

        public override JsonCommand? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            JsonElement root = doc.RootElement;

            if (!root.TryGetProperty("name", out JsonElement nameElement)) {
                throw new JsonException("Missing 'name' property in command JSON.");
            }

            string commandName = nameElement.GetString()!;
            if (!_commandTypes.TryGetValue(commandName, out Type? commandType)) {
                throw new JsonException($"Unknown command type: {commandName}");
            }

            return (JsonCommand)JsonSerializer.Deserialize(root.GetRawText(), commandType, options)!;
        }

        public override void Write(Utf8JsonWriter writer, JsonCommand value, JsonSerializerOptions options) {
            string json = JsonSerializer.Serialize(value, value.GetType(), options);
            using JsonDocument doc = JsonDocument.Parse(json);
            doc.RootElement.WriteTo(writer);
        }
    }
}


public class CommandContainer {
    [JsonPropertyName("commands")]
    public List<JsonCommand> Commands { get; set; } = new();
}

// Базовый класс для команд
public abstract class JsonCommand {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

// Конкретные команды
public class ListCommand : JsonCommand {
    [JsonPropertyName("arguments")]
    public ListArguments Arguments { get; set; } = new();
}

public class ListArguments {
    [JsonPropertyName("relativePath")]
    public string RelativePath { get; set; } = string.Empty;
}

public class ReadCommand : JsonCommand {
    [JsonPropertyName("arguments")]
    public ReadArguments Arguments { get; set; } = new();
}

public class ReadArguments {
    [JsonPropertyName("filepath")]
    public string FilePath { get; set; } = string.Empty;

    [JsonPropertyName("start")]
    public int Start { get; set; }

    [JsonPropertyName("end")]
    public int End { get; set; }
}

public class WriteCommand : JsonCommand {
    [JsonPropertyName("arguments")]
    public WriteArguments Arguments { get; set; } = new();
}

public class WriteArguments {
    [JsonPropertyName("filepath")]
    public string FilePath { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class RunCodeCommand : JsonCommand {
    [JsonPropertyName("arguments")]
    public RunCodeArguments Arguments { get; set; } = new();
}

public class RunCodeArguments {
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}

public class TryCompileCommand : JsonCommand {
    [JsonPropertyName("arguments")]
    public object Arguments { get; set; } = new();
}