class RegionText
{
    public RegionText(string country, string textId)
    {
        this.textId = textId;
        this.country = country;
    }
    public string country { get; set; }
    public string textId { get; set; }
}