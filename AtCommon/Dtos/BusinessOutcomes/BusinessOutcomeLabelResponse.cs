namespace AtCommon.Dtos.BusinessOutcomes
{
    public class BusinessOutcomeLabelResponse
    {        
        public int Order { get; set; }
        public int Uid { get; set; }
        public string Label { get; set; }
        public string Color { get; set; }
        public void SetFields(string label, string color, int order)
        {
            this.Label = label;
            this.Color = color;
            this.Order = Order;
        }
    }
}