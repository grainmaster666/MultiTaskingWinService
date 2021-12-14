using Newtonsoft.Json;
using System.Collections.Generic;

class Company
{
    public int ID { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
}
public class PeroidData
{
    public string value { get; set; }
    public string displayValue { get; set; }
    public string url { get; set; }
    public string shareUrl { get; set; }
}

public class PivotLevel2
{
    public string pivotPoint { get; set; }
    public string r1 { get; set; }
    public string r2 { get; set; }
    public string r3 { get; set; }
    public string s1 { get; set; }
    public string s2 { get; set; }
    public string s3 { get; set; }
}

public class PivotLevel
{
    public string key { get; set; }
    public PivotLevel2 pivotLevel { get; set; }
}

public class Summary
{
    public string sentiments { get; set; }
    public int bearishCount { get; set; }
    public int bullishCount { get; set; }
    public int neutralCount { get; set; }
    public string indication { get; set; }
    public string detailUrl { get; set; }
}

public class Overall
{
    public int totalBearish { get; set; }
    public int totalBullish { get; set; }
    public int totalNeutral { get; set; }
    public string indication { get; set; }
}

public class Sma
{
    public string key { get; set; }
    public string value { get; set; }
    public string indication { get; set; }
}

public class Ema
{
    public string key { get; set; }
    public string value { get; set; }
    public string indication { get; set; }
}

public class Data
{
    public string currentPeriod { get; set; }
    public List<PeroidData> peroidData { get; set; }
    public List<PivotLevel> pivotLevels { get; set; }
    public List<Summary> summary { get; set; }
    public Overall overall { get; set; }
    public string displayLock { get; set; }
    public string proNote { get; set; }
    public List<Sma> sma { get; set; }
    public List<Ema> ema { get; set; }
}

public class Root
{
    public int success { get; set; }
    public Data data { get; set; }
}

/*Volume and delivery Model class*/ 
public class Price
{
    [JsonProperty("1 Week")]
    public double _1Week { get; set; }

    [JsonProperty("1 Month")]
    public double _1Month { get; set; }

    [JsonProperty("3 Months")]
    public double _3Months { get; set; }
    public double YTD { get; set; }

    [JsonProperty("1 Year")]
    public double _1Year { get; set; }

    [JsonProperty("3 Years")]
    public double _3Years { get; set; }
}

public class Today
{
    public int delivery { get; set; }
    public int cvol { get; set; }
    public string cvol_display_text { get; set; }
    public string delivery_display_text { get; set; }
    public string delivery_tooltip_text { get; set; }
    public string cvol_tooltip_text { get; set; }
}

public class Yesterday
{
    public int delivery { get; set; }
    public int cvol { get; set; }
    public string cvol_display_text { get; set; }
    public string delivery_display_text { get; set; }
    public string cvol_tooltip_text { get; set; }
    public string delivery_tooltip_text { get; set; }
}

public class _1WeekAvg
{
    public double delivery { get; set; }
    public double cvol { get; set; }
    public string cvol_display_text { get; set; }
    public string delivery_display_text { get; set; }
    public string cvol_tooltip_text { get; set; }
    public string delivery_tooltip_text { get; set; }
}

public class _1MonthAvg
{
    public double delivery { get; set; }
    public double cvol { get; set; }
    public string cvol_display_text { get; set; }
    public string delivery_display_text { get; set; }
    public string cvol_tooltip_text { get; set; }
    public string delivery_tooltip_text { get; set; }
}

public class Volume
{
    public Today Today { get; set; }
    public Yesterday Yesterday { get; set; }

    [JsonProperty("1 Week Avg")]
    public _1WeekAvg _1WeekAvg { get; set; }

    [JsonProperty("1 Month Avg")]
    public _1MonthAvg _1MonthAvg { get; set; }
}

public class StockPriceVolumeData
{
    public Price price { get; set; }
    public Volume volume { get; set; }
}

public class Data2
{
    public StockPriceVolumeData stock_price_volume_data { get; set; }
}

public class Root2
{
    public int success { get; set; }
    public Data2 data { get; set; }
}





