using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace KarnelTravels.API.Services;

public interface IVnPayService
{
    string CreatePaymentUrl(Guid orderId, decimal amount, string orderDescription, string clientIp);
    bool ValidateSignature(Dictionary<string, string> responseData, string secureHash);
    Dictionary<string, string> GetResponseData(string queryString);
}

public class VnPayService : IVnPayService
{
    private readonly IConfiguration _configuration;

    public VnPayService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreatePaymentUrl(Guid orderId, decimal amount, string orderDescription, string clientIp)
    {
        var vnp_TmnCode = _configuration["VnPay:Vnp_TmnCode"];
        var vnp_HashSecret = _configuration["VnPay:Vnp_HashSecret"];
        var vnp_Url = _configuration["VnPay:Vnp_Url"];
        var vnp_ReturnUrl = _configuration["VnPay:Vnp_ReturnUrl"];

        var vnp_Params = new Dictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", vnp_TmnCode },
            { "vnp_Amount", ((long)(amount * 100)).ToString() },
            { "vnp_BankCode", "VNBANK" },
            { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
            { "vnp_CurrCode", "VND" },
            { "vnp_IpAddr", clientIp },
            { "vnp_Locale", "vn" },
            { "vnp_OrderInfo", orderDescription },
            { "vnp_OrderType", "other" },
            { "vnp_ReturnUrl", vnp_ReturnUrl },
            { "vnp_TxnRef", orderId.ToString() },
            { "vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss") }
        };

        // Sort parameters by key
        var sortedParams = vnp_Params.OrderBy(x => x.Key).ToList();

        // Build query string
        var queryString = new StringBuilder();
        foreach (var param in sortedParams)
        {
            if (!string.IsNullOrEmpty(param.Value))
            {
                queryString.Append(WebUtility.UrlEncode(param.Key) + "=" + WebUtility.UrlEncode(param.Value) + "&");
            }
        }

        // Remove trailing &
        var rawData = queryString.ToString().TrimEnd('&');

        // Create signature
        var signature = HmacSha512(vnp_HashSecret, rawData);

        // Build final URL
        var paymentUrl = vnp_Url + "?" + rawData + "&vnp_SecureHash=" + signature;

        return paymentUrl;
    }

    public bool ValidateSignature(Dictionary<string, string> responseData, string secureHash)
    {
        var vnp_HashSecret = _configuration["VnPay:Vnp_HashSecret"];

        // Remove vnp_SecureHash from data for validation
        var dataToSign = responseData
            .Where(kvp => kvp.Key != "vnp_SecureHash" && kvp.Key != "vnp_SecureHashType")
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => WebUtility.UrlEncode(kvp.Key) + "=" + WebUtility.UrlEncode(kvp.Value))
            .Aggregate((a, b) => a + "&" + b);

        var expectedSignature = HmacSha512(vnp_HashSecret, dataToSign);

        return expectedSignature.Equals(secureHash, StringComparison.OrdinalIgnoreCase);
    }

    public Dictionary<string, string> GetResponseData(string queryString)
    {
        var result = new Dictionary<string, string>();

        if (string.IsNullOrEmpty(queryString))
            return result;

        var queryParams = HttpUtility.ParseQueryString(queryString);
        foreach (string key in queryParams)
        {
            if (key != null)
            {
                result[key] = queryParams[key] ?? "";
            }
        }

        return result;
    }

    private string HmacSha512(string key, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using (var hmac = new HMACSHA512(keyBytes))
        {
            var hash = hmac.ComputeHash(dataBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
