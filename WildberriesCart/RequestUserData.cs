using Newtonsoft.Json;

namespace WildberriesCart;

public class RequestUserData
{
    public static async Task<WbUserData?> RequestNewData()
    {
        string filePath = "appData.txt";
        WbUserData? data;
        if (File.Exists(filePath))
        {
            Console.Write("Хотите использовать сохраненные данные? (1 - Да / 2 - Нет): ");
            string answer = Console.ReadLine().Trim().ToLower();

            if (answer == "1")
            {
                data = await LoadData(filePath);
                return data;
            }
        }

        data = new WbUserData();
        data.DeviceId = RequestInput("Введите Device_id: ");
        data.BearerToken = RequestInput("Введите Bearer Token: ");
        await SaveDataAsync(filePath, data);
        return data;
    }

    public static string RequestInput(string message, bool isOnlyNumbers = false)
    {
        string? input;
        do
        {
            Console.Write(message);
            input = Console.ReadLine();
            if (isOnlyNumbers && !IsDigitsOnly(input))
            {
                Console.WriteLine("Ввод должен содержать только цифры. Попробуйте еще раз.");
                input = null;
            }
        } while (string.IsNullOrEmpty(input));

        return input;
    }

    private static bool IsDigitsOnly(string str)
    {
        foreach (char c in str)
        {
            if (c < '0' || c > '9')
                return false;
        }

        return true;
    }

    private static async Task SaveDataAsync(string filePath, WbUserData? data)
    {
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        await using StreamWriter file = File.CreateText(filePath);
        await file.WriteAsync(json);
    }

    private static async Task<WbUserData?> LoadData(string filePath)
    {
        using StreamReader file = File.OpenText(filePath);
        var json = await file.ReadToEndAsync();
        return JsonConvert.DeserializeObject<WbUserData>(json);
    }
}