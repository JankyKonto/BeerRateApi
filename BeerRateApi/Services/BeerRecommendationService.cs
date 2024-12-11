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

public class BeerRecommendationService : BaseService, IBeerRecommendationService
{
    public BeerRecommendationService(AppDbContext dbContext, ILogger logger, IMapper mapper) 
        : base(dbContext, logger, mapper)
    {

    }

    public IEnumerable<BeerDTO> RecommendSimilarBeers(int beerId, int numberOfRecommendations = 3)
    {
        var beerToRecommend = DbContext.Beers.FirstOrDefault(b => b.Id == beerId);
        if (beerToRecommend == null) throw new ArgumentException("Beer not found.");

        // Prepare data for clustering
        var mlContext = new MLContext();
        var beerFeatures = DbContext.Beers.Select(b => new BeerFeature
        {
            Id = b.Id,
            AlcoholAmount = (float)b.AlcoholAmount,
            Ibu = b.Ibu,
            Kind = b.Kind
        }).ToList();

        var dataView = mlContext.Data.LoadFromEnumerable(beerFeatures);

        // Define clustering pipeline
        var pipeline = mlContext.Transforms.Concatenate("Features", nameof(BeerFeature.AlcoholAmount), nameof(BeerFeature.Ibu), nameof(BeerFeature.Kind))
            .Append(mlContext.Clustering.Trainers.KMeans("Features", numberOfClusters: Math.Min(DbContext.Beers.ToList().Count, 5)));

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
            .ToList();



        return similarBeers;
    }

    // Classes for ML.NET
    private class BeerFeature
    {
        public int Id { get; set; }
        public float AlcoholAmount { get; set; }
        public float Ibu { get; set; }
        public float Kind { get; set; }
    }

    private class ClusterPrediction : BeerFeature
    {
        [ColumnName("PredictedLabel")]
        public uint PredictedClusterId { get; set; }
    }
}
