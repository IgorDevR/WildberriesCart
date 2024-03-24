using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace WildberriesCart;

internal class Program
{
    private static HttpClient _client = new HttpClient();

    static Program()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://cart-storage-api.wildberries.ru/api/");
    }

    private static async Task Main(string[] args)
    {
        var requestNewData = await RequestUserData.RequestNewData();

        string nomenclature;
        do
        {
            nomenclature = RequestUserData.RequestInput("Введите номер номенклатуры: ");
            await SendRequestAsync(requestNewData, nomenclature);
        } while (RequestUserData.RequestInput(
                     "Хотите повторить запрос с новым номером номенклатуры? (1 - Да / 2 - Нет): ") != "2");
    }

    private static async Task SendRequestAsync(WbUserData? wbUserData, string nomenclature)
    {
        var chrt_id = await GetDetails(wbUserData, nomenclature);

        var clientTs = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var item = new WbAddToCartRequestDto
        {
            ChrtId = chrt_id,
            Quantity = 1,
            Cod1s = nomenclature,
            ClientTs = clientTs,
            OpType = 1,
            TargetUrl = "EX|4|MCS|IT|popular|||||"
        };

        var json = JsonConvert.SerializeObject(new List<object>() { item });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", wbUserData.BearerToken);
        _client.DefaultRequestHeaders.UserAgent.ParseAdd(wbUserData.UserAgent);

        var response = await _client.PostAsync($"basket/sync?ts={clientTs}&device_id={wbUserData.DeviceId}", content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"\nUrl: {response.RequestMessage.RequestUri}\n\n");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("Товар успешно добавлен в корзину.");
            Console.WriteLine("---------------------------------");
        }
        else
        {
            Console.WriteLine("---------------------------------");
            Console.WriteLine($"Ошибка добавление товара. Статус код: {response.StatusCode}\n");
        }
    }

    private static async Task<long> GetDetails(WbUserData? wbUserData, string nomenclature = "188431528")
    {
        string url = $"https://card.wb.ru/cards/v2/detail?nm={nomenclature}";

        try
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(wbUserData.UserAgent);

            string responseString = await client.GetStringAsync(url);
            var details = JsonConvert.DeserializeObject<WbProductDetailModel>(responseString);
            var optionId = details!.data.products.Select(d => d.sizes[0]).FirstOrDefault()!.optionId;

            return optionId;
        }
        catch (Exception e)
        {
            Console.WriteLine("Произошла ошибка при получении информции о товаре:");
            Console.WriteLine(e.Message);
            throw new Exception("Произошла ошибка при получении информции о товаре: " + e.Message);
        }
    }
}