namespace Before.Model
{
    public class OfferType : Entity
    {
        public string Name { get; set; }
        public ExpirationType ExpirationType { get; set; }
        public int DaysValid { get; set; }
        public DateTime? BeginDate { get; set; }
    }
}
