using APIGateway.Model;
using APIGateway.RabbitMQ;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace APIGateway.Controllers
{
    [ApiController]
    public class Gateway : Controller
    {
        private static readonly HttpClient client = new HttpClient();

        [HttpPost("api/gateway")]
        public async Task<ActionResult> RequestHandler([FromBody] GatewayRequest request)
        {
            string responseString = null;
            string orderId = null;
            HttpResponseMessage response;
            const string restaurantBaseURL = "https://localhost:7127/restaurant/";
            const string customerBaseURL = "https://localhost:7295/customer/";
            const string courierBaseURL = "https://localhost:7210/courier/";
            const string reviewsComplaintsBaseURL = "https://localhost:7174/";
            HttpContent content;

            switch (request.Command)
            {

                case "add_customer":
                    content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");
                    response = await client.PostAsync($"{customerBaseURL}add/", content);
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = await response.Content.ReadAsStringAsync();
                    }
                    break;

                case "login_customer":
                    content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");
                    response = await client.PostAsync($"{customerBaseURL}login/", content);
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = await response.Content.ReadAsStringAsync();
                        responseString = PrettyJson(responseString);
                    }
                    break;

                case "update_customer":
                    content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");
                    response = await client.PutAsync($"{customerBaseURL}update/", content);
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = await response.Content.ReadAsStringAsync();
                    }
                    break;

                case "get_customer_by_id":
                    response = await client.GetAsync($"{customerBaseURL}{request.Body.GetProperty("customerId").ToString()}");
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = await response.Content.ReadAsStringAsync();
                        responseString = PrettyJson(responseString);
                    }
                    break;

                case "place_order":
                    content = new StringContent(request.Body.GetProperty("orderlines").ToString(), Encoding.UTF8, "application/json");
                    response = await client.PostAsync($"{customerBaseURL}placeorder/{request.Body.GetProperty("customerId").ToString()}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        orderId = await response.Content.ReadAsStringAsync();
                        var orderResponse = await client.GetAsync($"{customerBaseURL}order/{orderId}");
                        if (orderResponse.IsSuccessStatusCode)
                        {
                            string orderJSON = await orderResponse.Content.ReadAsStringAsync();
                            RabbitMQSender.SendOrder(orderJSON);
                            responseString = "Order placed";
                        }
                    }
                    break;

                case "get_customer_orders":
                    response = await client.GetAsync($"{customerBaseURL}orders/{request.Body.GetProperty("customerId")}");
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = await response.Content.ReadAsStringAsync();
                        responseString = PrettyJson(responseString);
                    }
                    break;
                
                case "get_customer_order":
                    response = await client.GetAsync($"{customerBaseURL}order/{request.Body.GetProperty("orderId")}");
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = await response.Content.ReadAsStringAsync();
                        responseString = PrettyJson(responseString);
                    }
                    break;

                case "add_restaurant":
                    content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");
                    response = await client.PostAsync($"{restaurantBaseURL}add", content);
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = "Restaurant was added";
                    }
                    break;

                case "get_restaurant":
                    response = await client.GetAsync($"{restaurantBaseURL}{request.Body.GetProperty("restaurantId")}");
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = await response.Content.ReadAsStringAsync();
                        responseString = PrettyJson(responseString);
                    }
                    break;

                case "login_restaurant":
                    content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");
                    response = await client.PostAsync($"{restaurantBaseURL}login", content);
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = await response.Content.ReadAsStringAsync();
                        responseString = PrettyJson(responseString);
                    }
                    if (!response.IsSuccessStatusCode)
                    {
                        responseString = await response.Content.ReadAsStringAsync();
                    }
                    break;

                case "update_restaurant":
                    content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");
                    response = await client.PutAsync($"{restaurantBaseURL}update", content);
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = "Succesfully updated";
                    }
                    break;

                case "add_menu":
                    content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");
                    response = await client.PostAsync($"{restaurantBaseURL}addmenu", content);
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = "Succesfully added to the menu";
                    }
                    break;

                case "update_menu":
                    content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");
                    response = await client.PutAsync($"{restaurantBaseURL}updatemenu", content);
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = "Succesfully updated the menu";
                    }
                    break;

                case "orders_24_hours":
                    content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");
                    response = await client.GetAsync($"{restaurantBaseURL}orders/{request.Body.GetProperty("restaurantId")}");
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = await response.Content.ReadAsStringAsync();
                        responseString = PrettyJson(responseString);
                    }
                    break;

                case "accept_order":
                    orderId = request.Body.GetProperty("orderId").ToString();
                    RabbitMQReceiver.AcceptOrder(orderId);
                    response = await client.GetAsync($"{restaurantBaseURL}accept/{orderId}");
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = "Order accepted";
                    }
                    break;

                case "being_delivered":
                    orderId = request.Body.GetProperty("orderId").ToString();
                    response = await client.GetAsync($"{restaurantBaseURL}pickup/{orderId}");
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = "Order has been picked up and is on its way";
                    }
                    break;

                case "order_delivered":
                    orderId = request.Body.GetProperty("orderId").ToString();
                    response = await client.GetAsync($"{restaurantBaseURL}delivered/{orderId}");
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = "Order has been delivered";
                    }
                    break;

                case "add_courier":
                    content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");
                    response = await client.PostAsync($"{courierBaseURL}add/", content);
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = "Courier added";
                    }
                    break;

                case "assign_courier":
                    content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");
                    response = await client.PostAsync($"{courierBaseURL}assign/", content);
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = "Courier Assigned";
                    }
                    break;

                case "add_review":
                    content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");
                    response = await client.PostAsync($"{reviewsComplaintsBaseURL}review/add", content);
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = "Review added";
                    }
                    break;

                case "add_complaint":
                    content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");
                    response = await client.PostAsync($"{reviewsComplaintsBaseURL}complaint/add", content);
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = "Complaint added";
                    }
                    break;

                default:
                    break;
            }
            if (responseString != null)
            {
                return Ok(responseString);
            }
            return BadRequest("No command was provided");
        }

        private static string PrettyJson(string json)
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                return JsonSerializer.Serialize(doc.RootElement, options);
            }
        }

    }
}
