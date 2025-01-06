namespace BeerRateApi.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BeerRateApi.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using BeerRateApi.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using BeerRateApi.Interfaces;

/// <summary>
/// Service for providing beer recommendations based on clustering.
/// </summary>
public class BeerRecommendationService : BaseService, IBeerRecommendationService
{
    /// <summary>
    /// Encoded dictionary mapping countries to unique float values.
    /// </summary>
    private readonly Dictionary<string, float> _countriesEncoded;

    /// <summary>
    /// Initializes a new instance of the <see cref="BeerRecommendationService"/> class.
    /// </summary>
    /// <param name="dbContext">Database context for accessing data.</param>
    /// <param name="logger">Logger for logging errors and information.</param>
    /// <param name="mapper">Mapper for object-to-object mapping.</param>
    public BeerRecommendationService(AppDbContext dbContext, ILogger logger, IMapper mapper) 
        : base(dbContext, logger, mapper)
    {
        _countriesEncoded = new Dictionary<string, float>();
        float i = 0.0f;
        dbContext.Beers.ToList().ForEach(beer =>
        {
            if (!_countriesEncoded.ContainsKey(beer.OriginCountry))
            {
                _countriesEncoded.Add(beer.OriginCountry, i);
            }
            i++;
        });
    }
    /// <summary>
    /// Recommends similar beers based on clustering.
    /// </summary>
    /// <param name="beerId">The ID of the beer to find similar beers for.</param>
    /// <param name="numberOfRecommendations">The number of recommendations to return. Default is 3.</param>
    /// <returns>A collection of recommended beers as DTOs.</returns>
    /// <exception cref="ArgumentException">Thrown when the beer with the given ID is not found.</exception>
    public async Task<IEnumerable<BeerDTO>> RecommendSimilarBeers(int beerId, int numberOfRecommendations = 3)
    {
        var beerToRecommend = await DbContext.Beers.FirstOrDefaultAsync(b => b.Id == beerId);
        int beerCounter = await DbContext.Beers.Where(b=>b.IsConfirmed).CountAsync();
        if (beerToRecommend == null) throw new ArgumentException("Beer not found.");

        // Prepare data for clustering
        var mlContext = new MLContext();
        var beerFeatures = (await DbContext.Beers.ToListAsync()).Where(b=>b.IsConfirmed).Select(b => new BeerFeature
        {
            Id = b.Id,
            
            AlcoholAmount = (float)b.AlcoholAmount,
            Kind = b.Kind,
            OriginCountry = _countriesEncoded[b.OriginCountry],
            Ibu = b.Ibu,
            AverageTasteRate = b.AverageTasteRate,
            AverageAromaRate = b.AverageAromaRate,
            AverageFoamRate = b.AverageFoamRate,
            AverageColorRate = b.AverageColorRate
        }).ToList();

        var dataView = mlContext.Data.LoadFromEnumerable(beerFeatures);

        // Define clustering pipeline
        var pipeline = mlContext.Transforms.Concatenate("Features",nameof(BeerFeature.Kind), nameof(BeerFeature.AlcoholAmount), nameof(BeerFeature.Ibu), nameof(BeerFeature.AverageTasteRate),
            nameof(BeerFeature.AverageAromaRate), nameof(BeerFeature.AverageFoamRate), nameof(BeerFeature.AverageColorRate), nameof(BeerFeature.OriginCountry))
            .Append(mlContext.Clustering.Trainers.KMeans("Features", 
            numberOfClusters: beerCounter % numberOfRecommendations != 0 ? beerCounter / numberOfRecommendations-1 : beerCounter / numberOfRecommendations));

        // Train model
        var model = pipeline.Fit(dataView);
        var predictions = model.Transform(dataView);

        // Extract cluster assignments
        var clusteringResults = mlContext.Data.CreateEnumerable<ClusterPrediction>(predictions, reuseRowObject: false).ToList();

        // Find cluster for the target beer
        var beerCluster = clusteringResults.FirstOrDefault(p => p.Id == beerToRecommend.Id)?.PredictedClusterId;
        if (beerCluster == null) return new List<BeerDTO>();

        // Find beers in the same cluster



        var similarBeers = clusteringResults
            .Where(p => p.PredictedClusterId == beerCluster && p.Id != beerId)
            .Select(p => DbContext.Beers.First(b => b.Id == p.Id))
            .Take(numberOfRecommendations)
            .Select(beer => Mapper.Map<BeerDTO>(beer))
            ;



        return similarBeers;
    }
    /// <summary>
    /// Class representing beer features used for clustering.
    /// </summary>
    private class BeerFeature
    {
        public int Id { get; set; }
        public float Kind { get; set; }
        public float OriginCountry {  get; set; }
        public float AlcoholAmount { get; set; }
        public float Ibu { get; set; }
        public float AverageTasteRate { get; set; }
        public float AverageAromaRate { get; set; }
        public float AverageFoamRate { get; set; }
        public float AverageColorRate { get; set; }
    }

    /// <summary>
    /// Class representing clustering predictions.
    /// </summary>
    private class ClusterPrediction : BeerFeature
    {
        /// <summary>
        /// The predicted cluster ID for the beer.
        /// </summary>
        [ColumnName("PredictedLabel")]
        public uint PredictedClusterId { get; set; }
    }
}
