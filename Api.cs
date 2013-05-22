#region license
//   Please read and agree to license.md contents before using this SDK.
#endregion
using HtmlAgilityPack;
using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace mcxNOW
{
    public class Api
    {
        const string BaseUrl = "https://www.mcxnow.com/";

        readonly string baseUrl;

        private string secretKey;

        private string session;

        public Api()
            : this(BaseUrl)
        {

        }

        public Api(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }



        public Orders Orders(Currency currency)
        {
            RestRequest request = Get("orders");

            request.AddParameter("cur", currency.Code);

            return Execute<Orders>(request);
        }



        public Info Info(Currency currency)
        {
            RestRequest request = Get("info", true);

            request.AddParameter("cur", currency.Code);

            return Execute<Info>(request);
        }



        public void ExecuteTrade(Trade trade)
        {
            trade.Execute = true;
            SendTrade(trade);
        }

        public void SendTrade(Trade trade)
        {
            RestRequest request = Get("action?trade", true);

            request.AddParameter("cur", trade.Currency.Code);
            request.AddParameter("sk", secretKey);
            request.AddParameter("amt", trade.Amount.ToString(CultureInfo.InvariantCulture));
            request.AddParameter("price", trade.Price.ToString(CultureInfo.InvariantCulture));
            request.AddParameter("buy", trade.Type == Trade.Types.BUY ? 1 : 0);
            request.AddParameter("enabled", trade.Execute ? 1 : 0);

            Execute(request);
        }

        public void ConfirmTrade(string id, Currency currency)
        {
            RestRequest request = Get("action?exectrade", true);

            request.AddParameter("sk", secretKey);
            request.AddParameter("cur", currency.Code);
            request.AddParameter("id", id);

            Execute(request);
        }

        public void CancelTrade(string id, Currency currency)
        {
            RestRequest request = Get("action?canceltrade", true);

            request.AddParameter("sk", secretKey);
            request.AddParameter("cur", currency.Code);
            request.AddParameter("id", id);

            Execute(request);
        }



        public ChatResponse Chat()
        {
            RestRequest request = Get("chat");

            //Execute(request, new XmlAttributeDeserializer());
            //, new XmlDeserializer()
            //    {
            //        RootElement = "doc"
            //    }
            return Execute<ChatResponse>(request);
        }

        public void Message(string text)
        {
            RestRequest request = Get("action?chat", true);

            request.AddParameter("sk", secretKey);
            request.AddParameter("t", text);

            Execute(request);
        }



        public List<Fund> Funds()
        {
            RestRequest request = Get("user.html", true);

            IRestResponse response = Execute(request);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(response.Content);

            Regex balance = new Regex(@"^Balance: ([0-9]+\.[0-9]+) ([A-Z]{2,3})$");
            Regex incoming = new Regex(@"^Incoming: ([0-9]+\.[0-9]+) ([A-Z]{2,3})$");

            List<Fund> funds = new List<Fund>();
            Dictionary<string, Fund> map = new Dictionary<string, Fund>();
            foreach (HtmlNode node in document.DocumentNode.SelectNodes(@"//div[@class='fundbox']"))
            {
                HtmlNode balanceNode = node.SelectSingleNode(@".//div[@class='fundboxbal']");
                Match balanceMatch = balance.Match(balanceNode.ChildNodes[balanceNode.ChildNodes.Count - 1].InnerText.Trim());
                Fund fund = new Fund();
                fund.Balance = Convert.ToDecimal(balanceMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                fund.Currency = Currency.FromString(balanceMatch.Groups[2].Value);

                HtmlNode incomingNode = balanceNode.SelectSingleNode(@"./small");
                if (incomingNode != null)
                {
                    fund.Incoming = Convert.ToDecimal(incoming.Match(incomingNode.InnerText).Groups[1].Value, CultureInfo.InvariantCulture);
                }
                else
                {
                    fund.Incoming = 0.0M;
                }

                HtmlNode depositNode = node.SelectSingleNode(@"./div[@class='fundrow']/b[2]");
                fund.DepositAddress = depositNode != null ? depositNode.InnerText.Trim() : null;
                funds.Add(fund);
                map.Add(fund.Currency.Name, fund);
            }

            Regex name = new Regex(@"^[A-Za-z]+\b");
            Regex amount = new Regex(@"^([0-9]+\.[0-9]+) ([A-Z]{2,3})$");
            foreach (HtmlNode node in document.DocumentNode.SelectNodes(@"//div[@id='main']/div/div[2]/div[not(@class)][not(@id)]"))
            {
                Match nameMatch = name.Match(node.InnerText);
                if (nameMatch.Success && map.ContainsKey(nameMatch.Value))
                {
                    HtmlNodeCollection valuesNodes = node.SelectNodes(@"./b");
                    Fund fund = map[nameMatch.Value];
                    fund.MinimumDeposit = Convert.ToDecimal(amount.Match(valuesNodes[0].InnerText.Trim()).Groups[1].Value, CultureInfo.InvariantCulture);
                    fund.DepositConfirmations = Convert.ToInt32(valuesNodes[2].InnerText.Trim(), CultureInfo.InvariantCulture);
                    fund.WithdrawFee = Convert.ToDecimal(amount.Match(valuesNodes[1].InnerText.Trim()).Groups[1].Value, CultureInfo.InvariantCulture);
                }
            }

            return funds;
        }

        public void RequestDepositAddress(Currency currency)
        {
            RestRequest request = Get("action?getaddress", true);

            request.AddParameter("sk", secretKey);
            request.AddParameter("cur", currency.Code);

            Execute(request);
        }

        public void Withdraw(Currency currency, decimal amount, string address, string password = null)
        {
            RestRequest request = Post("withdraw.html", true);

            request.AddParameter("cur", currency.Code);
            request.AddParameter("amount", amount.ToString(CultureInfo.InvariantCulture));
            request.AddParameter("address", address);
            request.AddParameter("password", Password(password));

            Execute(request);
        }


        private string password;

        public void Login(string username, string password, bool remember = false)
        {
            RestRequest request = Post("?login");

            request.AddParameter("user", username);
            request.AddParameter("pass", password);

            IRestResponse response = Execute(request);

            RestResponseCookie keyCookie = response.Cookies.SingleOrDefault(x => x.Name == "mcx_key");
            if (keyCookie == null)
            {
                throw new AuthenticationFailed();
            }
            secretKey = keyCookie.Value;

            RestResponseCookie sessionCookie = response.Cookies.SingleOrDefault(x => x.Name == "mcx_sess");
            if (sessionCookie == null)
            {
                throw new AuthenticationFailed();
            }
            session = sessionCookie.Value;

            if (remember)
            {
                this.password = password;
            }
        }

        private string Password(string password)
        {
            if (password == null && this.password == null)
            {
                throw new ArgumentNullException("Remember password on login or provide password");
            }

            if (password == null)
            {
                password = this.password;
            }

            return password;
        }



        private RestRequest Get(string resource = "/", bool authenticate = false)
        {
            return CreateRestRequest(Method.GET, resource, authenticate);
        }

        private RestRequest Post(string resource = "/", bool authenticate = false)
        {
            return CreateRestRequest(Method.POST, resource, authenticate);
        }



        private RestRequest CreateRestRequest(Method method, string resource = "/", bool authenticate = false)
        {
            RestRequest restRequest = new RestRequest()
            {
                Method = method,
                Resource = resource
            };

            if (authenticate)
            {
                restRequest.AddParameter("mcx_sess", session, ParameterType.Cookie);
                restRequest.AddParameter("mcx_key", secretKey, ParameterType.Cookie);
            }

            return restRequest;
        }



        public T Execute<T>(RestRequest request, IDeserializer deserializer = null) where T : new()
        {
            RestClient client = CreateRestClient(deserializer);
            IRestResponse<T> response = client.Execute<T>(request);

            HandleResponse(response);

            return response.Data;
        }

        public IRestResponse Execute(RestRequest request, IDeserializer deserializer = null)
        {
            RestClient client = CreateRestClient(deserializer);
            IRestResponse response = client.Execute(request);

            HandleResponse(response);

            return response;
        }



        private RestClient CreateRestClient(IDeserializer deserializer = null)
        {
            RestClient restClient = new RestClient()
            {
                BaseUrl = baseUrl,
                FollowRedirects = false
            };

            restClient.AddHandler("text/xml", deserializer == null ? new XmlDeserializer() : deserializer);

            return restClient;
        }

        private static void HandleResponse(IRestResponse response)
        {
            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }

            if (response.ResponseStatus == ResponseStatus.Error
                || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                throw new ConnectionErrorException();
            }

            if (response.ResponseStatus == ResponseStatus.Completed
                && (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    || response.StatusCode == System.Net.HttpStatusCode.BadGateway))
            {
                throw new mcxNOW.UnauthorizedAccessException();
            }

            if (response.ResponseStatus == ResponseStatus.Completed
                && response.StatusCode == System.Net.HttpStatusCode.GatewayTimeout)
            {
                throw new ServerIsUnavailable();
            }

            string notLoggedIn = "<h1>You are not currently logged in.</h1>";
            if (response.ResponseStatus == ResponseStatus.Completed
                && response.StatusCode == System.Net.HttpStatusCode.OK
                && response.ContentType.IndexOf("text/html") == 0
                && response.Content.IndexOf(notLoggedIn) >= 0)
            {
                throw new mcxNOW.UnauthorizedAccessException();
            }
        }
    }

    public class UnauthorizedAccessException : System.UnauthorizedAccessException { }

    public class AuthenticationFailed : Exception { }

    public class ConnectionErrorException : Exception { }

    public class ServerIsUnavailable : Exception { }
}
