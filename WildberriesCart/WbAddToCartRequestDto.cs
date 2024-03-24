using Newtonsoft.Json;

namespace WildberriesCart;

public class WbAddToCartRequestDto
{
    [JsonProperty("chrt_id")]
    public long ChrtId { get; set; }

    [JsonProperty("client_ts")]
    public long ClientTs { get; set; }

    [JsonProperty("cod_1s")]
    public string Cod1s { get; set; }

    [JsonProperty("op_type")]
    public int OpType { get; set; }

    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    [JsonProperty("target_url")]
    public string TargetUrl { get; set; } = "EX|4|MCS|IT|popular|||||";

    public override string ToString()
    {
        return ($"chrt_id: {ChrtId},\n" +
                $"Quantity: {Quantity},\n" +
                $"Cod1s: {Cod1s},\n" +
                $"ClientTs: {ClientTs},\n" +
                $"OpType: {OpType},\n" +
                $"TargetUrl: {TargetUrl},\n\n");
    }
}