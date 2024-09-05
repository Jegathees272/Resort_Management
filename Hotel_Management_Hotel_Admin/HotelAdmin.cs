using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using HttpMultipartParser;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
namespace Hotel_Management_Hotel_Admin
{
    public class HotelAdmin
    {
        public APIGatewayProxyResponse AddHotel(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            var res = new APIGatewayProxyResponse()
            {
                Headers = new Dictionary<string, string>(),
                StatusCode = 200
            };

            res.Headers.Add("Access-Control-Allow-Origin", "*");
            res.Headers.Add("Access-Control-Allow-Headers", "*");
            res.Headers.Add("Access-Control-Allow-Methods", "OPTIONS, POST");

            var bodyContent  = request.IsBase64Encoded
                ? Convert.FromBase64String(request.Body) 
                : Encoding.UTF8.GetBytes(request.Body);

            using var memstream = new MemoryStream(bodyContent);

            var formData = MultipartFormDataParser.Parse(memstream);

            var hotelName = formData.GetParameterValue("hotelName");
            var hotelRating = formData.GetParameterValue("hotelRating");
            var hotelCity = formData.GetParameterValue("hotelCity");
            var hotelPrice = formData.GetParameterValue("hotelPrice");

            var file = formData.Files.FirstOrDefault();

            var userId = formData.GetParameterValue("userId");
            var idToken = formData.GetParameterValue("idToken");

            var token = new JwtSecurityToken(jwtEncodedString: idToken);
            var group = token.Claims.FirstOrDefault(x => x.Type == "cognito:group");

            if(group == null || group.Value != "Admin")
            {
                res.StatusCode = (int)HttpStatusCode.Unauthorized;
                res.Body = JsonSerializer.Serialize(new {Error = "Unauthorized. Must be a member of Admin group." });
            }

            Console.WriteLine("First Lambda Function works fine.");

            return res;

        }


    }
}
