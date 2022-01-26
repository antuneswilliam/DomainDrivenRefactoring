using After.Model;
using MediatR;
using System.Text.Json;

namespace After.Services
{
    public class AssignOfferHandler : IRequestHandler<AssignOfferRequest>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IOfferValueCalculator _offerValueCalculator;

        public AssignOfferHandler(
            AppDbContext appDbContext,
            IOfferValueCalculator offerValueCalculator)
        {
            _appDbContext = appDbContext;
            _offerValueCalculator = offerValueCalculator;
        }

        public async Task<Unit> Handle(AssignOfferRequest request, CancellationToken cancellationToken)
        {
            await SeedData(request);

            var member = await _appDbContext.Members.FindAsync(new object[] { request.MemberId }, cancellationToken);
            
            var offerType = await _appDbContext.OfferTypes.FindAsync(new object[] { request.OfferTypeId }, cancellationToken);
            
            Offer offer = await member.AssignOffer(offerType, cancellationToken, _offerValueCalculator);

            await SaveOffer(offer, cancellationToken);

            return Unit.Value;
        }

        private async Task SaveOffer(Offer offer, CancellationToken cancellationToken)
        {
            await _appDbContext.Offers.AddAsync(offer, cancellationToken);

            await _appDbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedData(AssignOfferRequest request)
        {
            try
            {
                var memberId = request.MemberId;
                var offerTypeId = request.OfferTypeId;

                Member member = new()
                {
                    Id = memberId,
                    FirstName = "William",
                    LastName = "Antunes",
                    Email = "william.antunes.mv@gmail.com",
                    NumberOfActiveOffers = 0
                };

                OfferType offerType = new()
                {
                    Id = offerTypeId,
                    Name = "Offer 1",
                    ExpirationType = ExpirationType.Fixed,
                    BeginDate = DateTime.Now,
                    DaysValid = 1
                };

                _appDbContext.Add(member);
                _appDbContext.Add(offerType);

                await _appDbContext.SaveChangesAsync();
            }
            catch
            {
            }
        }
    }
}
