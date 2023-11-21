using System.Text.RegularExpressions;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.Write("Введите путь: ");
        string path = Convert.ToString(Console.ReadLine());
        Console.WriteLine(path);

        // чтение файла
        string fileText = await File.ReadAllTextAsync(path);

        // Заменяем имя класса
        fileText = Regex.Replace(fileText, @"(?<=\bclass\s+)\w+(?=\s*\{)", "NewClassName");

        // Удаление комментариев
        var blockComments = @"/\*(.*?)\*/";
        var lineComments = @"//(.*?)\r?\n";
        var strings = @"""((\\[^\n]|[^""\n])*)""";
        var verbatimStrings = @"@(""[^""]*"")+";

        fileText = Regex.Replace(fileText,
    blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
    me =>
    {
        if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
            return me.Value.StartsWith("//") ? Environment.NewLine : "";
        // Сохранение литеральных строк
        return me.Value;
    },
    RegexOptions.Singleline);

        // Удаление пробелов и переводов на новую строку
        fileText = Regex.Replace(fileText, @"\s+", " ");

        // Находим все идентификаторы в коде
        var identifiers = Regex.Matches(fileText, @"\b\w+\b")
        .OfType<Match>()
        .Select(m => m.Value)
        .Distinct()
        .ToArray();

        // Заменяем идентификаторы на односимвольные или двухсимвольные имена
        for (int i = 0; i < identifiers.Length; i++)
        {
            string replacement = i < 26 ? ((char)('a' + i)).ToString() : "var" + (i - 26 + 1);
            fileText = Regex.Replace(fileText, $@"\b{identifiers[i]}\b", replacement);
        }

        // Выводим текст
        Console.WriteLine(fileText);

        Console.Write("Введите путь сохранения и имя нового файла: ");
        string newPath = Convert.ToString(Console.ReadLine());
        
        // Запись строки, сохранение файла
        await File.WriteAllTextAsync(newPath, fileText);
    }
}