using System;
using System.Collections.Generic;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
public class Quote
{
    public string sid { get; set; }
    public double price { get; set; }
    public double close { get; set; }
    public double change { get; set; }
    public double high { get; set; }
    public double low { get; set; }
    public int volume { get; set; }
    public DateTime date { get; set; }
}

public class Stock
{
    public double? marketCap { get; set; }
    public string ticker { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public string sector { get; set; }
    public string slug { get; set; }
    public string sid { get; set; }
    public string match { get; set; }
    public Quote quote { get; set; }
}

public class TickerData
{
    public int total { get; set; }
    public List<Stock> stocks { get; set; }
    public List<object> brands { get; set; }
    public List<object> indices { get; set; }
}

public class Tickertape
{
    public bool success { get; set; }
    public TickerData data { get; set; }
}


