using Before.Model;
using MediatR;
using System.Text.Json;

namespace Before.Services
{
    public class AssignOfferHandler : IRequestHandler<AssignOfferRequest>
    {
        private readonly AppDbContext _appDbContext;
        private readonly HttpClient _httpClient;

        public AssignOfferHandler(
            AppDbContext appDbContext,
            HttpClient httpClient)
        {
            _appDbContext = appDbContext;
            _httpClient = httpClient;
        }

        public async Task<Unit> Handle(AssignOfferRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await SeedData(request);

                var member = await _appDbContext.Members.FindAsync(new object[] { request.MemberId }, cancellationToken);
                var offerType = await _appDbContext.OfferTypes.FindAsync(new object[] { request.OfferTypeId }, cancellationToken);

                // Calculate offer value
                _httpClient.BaseAddress = new Uri("http://localhost:5170/Test/");

                var response = await _httpClient.GetAsync(
                    $"calculate-offer-value?email={member.Email}&offerType={offerType.Name}",
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var value = await JsonSerializer.DeserializeAsync<int>(responseStream, cancellationToken: cancellationToken);

                // Calculate expiration date
                DateTime dateExpiring;

                switch (offerType.ExpirationType)
                {
                    case ExpirationType.Assignment:
                        dateExpiring = DateTime.Today.AddDays(offerType.DaysValid);
                        break;
                    case ExpirationType.Fixed:
                        dateExpiring = offerType.BeginDate?.AddDays(offerType.DaysValid)
                                       ?? throw new InvalidOperationException();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // Assign offer
                var offer = new Offer
                {
                    MemberAssigned = member,
                    Type = offerType,
                    Value = value,
                    DateExpiring = dateExpiring
                };
                member.AssignedOffers.Add(offer);
                member.NumberOfActiveOffers++;

                await _appDbContext.Offers.AddAsync(offer, cancellationToken);

                await _appDbContext.SaveChangesAsync(cancellationToken);

                return Unit.Value;

            }
            catch (Exception ex)
            {

                throw;
            }
            
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
