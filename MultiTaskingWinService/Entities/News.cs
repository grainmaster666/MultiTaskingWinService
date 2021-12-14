using System;
using System.Collections.Generic;

public class Item
{
    public string title { get; set; }
    public string feed_type { get; set; }
    public string link { get; set; }
    public string description { get; set; }
    public DateTime date { get; set; }
    public string image { get; set; }
}

public class NewsData
{
    public int total { get; set; }
    public List<Item> items { get; set; }
}

public class News
{
    public bool success { get; set; }
    public NewsData data { get; set; }
}
