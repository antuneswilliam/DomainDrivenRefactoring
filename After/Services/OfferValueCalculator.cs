using After.Model;
using System.Text.Json;

namespace After.Services
{
    public interface IOfferValueCalculator
    {
        Task<int> Calculate(Member member, OfferType offerType, CancellationToken cancellationToken);
    }

    public class OfferValueCalculator : IOfferValueCalculator
    {
        private readonly HttpClient _httpClient;

        public OfferValueCalculator(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> Calculate(Member member, OfferType offerType, CancellationToken cancellationToken)
        {
            _httpClient.BaseAddress = new Uri("http://localhost:5080/Test/");

            var response = await _httpClient.GetAsync(
                $"calculate-offer-value?email={member.Email}&offerType={offerType.Name}",
                cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);

            return await JsonSerializer.DeserializeAsync<int>(responseStream, cancellationToken: cancellationToken);
        }
    }
}