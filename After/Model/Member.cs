using After.Services;

namespace After.Model
{
    public class Member : Entity
    {
        private readonly List<Offer> assignedOffers = new List<Offer>();

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IEnumerable<Offer> AssignedOffers => assignedOffers;
        public int NumberOfActiveOffers { get; set; }

        public async Task<Offer> AssignOffer(OfferType? offerType, CancellationToken cancellationToken, IOfferValueCalculator offerValueCalculator)
        {
            var value = await offerValueCalculator.Calculate(this, offerType, cancellationToken);

            DateTime dateExpiring = offerType.CalculateExpirationDate();

            var offer = new Offer
            {
                MemberAssigned = this,
                Type = offerType,
                Value = value,
                DateExpiring = dateExpiring
            };

            assignedOffers.Add(offer);
            NumberOfActiveOffers++;

            return offer;
        }
    }
}
