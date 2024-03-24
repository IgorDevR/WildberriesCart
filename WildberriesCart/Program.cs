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
            nomenclature = RequestUserData.RequestInput("Введите номер номенклатуры: ", true);
            await SendRequestAsync(requestNewData, nomenclature);
        } while (RequestUserData.RequestInput(
                     "Хотите повторить запрос с новым номером номенклатуры? (1 - Да / 2 - Нет): ") != "2");
    }

    private static async Task SendRequestAsync(WbUserData? wbUserData, string nomenclature)
    {
        var chrt_id = await GetDetails(wbUserData, nomenclature);
        if (chrt_id == 0)
        {
            Print.PrintToConsole("Не удалось получить информацию о товаре. Возможно неверный номер номенклатуры.");
            return;
        }

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
            Print.PrintToConsole($"Url: {response.RequestMessage.RequestUri}");
            Print.PrintToConsole($"Url: Товар успешно добавлен в корзину.");
        }
        else
        {
            Print.PrintToConsole($"Ошибка добавление товара. Статус код: {response.StatusCode}");
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

            long optionId = 0;

            if (details != null && details.data.products.Any())
                optionId = details.data.products.Select(d => d.sizes[0]).FirstOrDefault()!.optionId;

            return optionId;
        }
        catch (Exception e)
        {
            throw new Exception("Произошла ошибка при получении информции о товаре: " + e.Message);
        }
    }
}