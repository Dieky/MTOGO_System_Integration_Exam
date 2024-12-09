using System.Text.Json;

namespace APIGateway.Model
{
    public class GatewayRequest
    {
        public string Command { get; set; }
        public JsonElement Body { get; set; }
    }
}
