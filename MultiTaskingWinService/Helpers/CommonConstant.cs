namespace C9ISM.Scheduler.Helpers
{
    public static class CommonConstant
    {
        public static readonly string BaseUrl = "https://www.moneycontrol.com";

        public static readonly string BlockDealNse = BaseUrl + "/stocks/marketstats/block-deals/nse/";
        public static readonly string BlockDealBse = BaseUrl + "/stocks/marketstats/block-deals/bse/";
        public static readonly string BulkDealNse = BaseUrl + "/stocks/marketstats/bulk-deals/nse/";
        public static readonly string BulkDealBse = BaseUrl + "/stocks/marketstats/bulk-deals/bse/";
        public static readonly string BulkBlockDealXpath = "/html/body/section/section/section/div/div[2]/div[2]/table/tbody/tr";

        public static readonly string BaseApiUrl = "https://api.moneycontrol.com";
        public static readonly string SMAPILink = BaseApiUrl + "/mcapi/technicals/v2/details?scId=CompanyCode&dur=D&deviceType=W";
        public static readonly string VolDeliveryAPILink = BaseApiUrl + "/mcapi/v1/stock/price-volume?scId=CompanyCode";

        public static readonly string BaseScreenUrl = "https://www.screener.in";
        public static readonly string ScreenApi = BaseScreenUrl + "/api/company/search/?q=cname&v=3";

        public static readonly string BlockHistoryBse = "https://www.bseindia.com/" + "download/BhavCopy/Equity/EQstrdate_CSV.ZIP";
        public static readonly string BlockHistoryNse = "https://www1.nseindia.com/" + "content/historical/EQUITIES/year/month/cmstrdatebhav.csv.zip";
        public static readonly string ZipPath = @"D:\Upwork\StockiHstory\";

        public static readonly string BaseTickerTape = "https://api.tickertape.in";
        public static readonly string TickertapeURL = BaseTickerTape + "/search?text=cname&types=stock";

        public static readonly string NewsUrl = BaseTickerTape + "/stocks/feed/cname?types=news-video,news-article,opinion-article";

        public static readonly string CryptoAPI = "https://api.coingecko.com" + "/api/v3/coins/markets?vs_currency=inr&order=market_cap_desc&per_page=200&page=1&sparkline=false&price_change_percentage=1h%2C%2024h%2C%207d%2C%2014d%2C%2030d%2C%20200d";

        //public static readonly string DBConnectionString = @"Data Source=DESKTOP-TFRDGC0\SQLExpress;Initial Catalog=StockMonitor;Integrated Security=True";

        //public static readonly string BulkDealQuery = "INSERT INTO [dbo].[BlockBulkDeals] " +
        //    "([company],[client],[transType],[quantity],[price_Traded] ,[price_Closed],[Type] ,[trnasactionDate],[Exchange])" +
        //    "VALUES(@CompanyName,@Client,@TransactionType,@Quantity,@TradedPrice,@ClosedPrice,@TransactionType,@TransactionDate,@Exchange)";

    }
}
