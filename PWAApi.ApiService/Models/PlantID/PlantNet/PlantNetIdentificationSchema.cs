﻿namespace PWAApi.ApiService.Models.PlantID.PlantNet
{
    public class PlantNetIdentificationSchema
    {
        public PlantNetQuery? Query { get; set; }
        public string? Language { get; set; }
        public string? PreferredReferential { get; set; }
        public string? SwitchToProject { get; set; }
        public string? BestMatch { get; set; }
        public string? Version { get; set; }
        public PlantNetResult[]? Results { get; set; }
        public int RemainingIdentificationRequests { get; set; }
        public Model5[]? PredictedOrgans { get; set; }

        public PlantNetIdentificationSchema() { }
    }

    public class PlantNetQuery
    {
        public string? Project { get; set; }
        public string[]? Images { get; set; }
        public string[]? Organs { get; set; }
        public bool IncludeRelatedImages { get; set; }
        public bool NoReject { get; set; }
        public string? Type { get; set; }
    }

    public class PlantNetResult
    {
        public float Score { get; set; }
        public Model4? Species { get; set; }
        public PlantNetImage[]? Images { get; set; }
        public GBIF? GBIF { get; set; }
        public POWO? POWO { get; set; }
        public IUCN? IUCN { get; set; }
    }

    public class Model4
    {
        public string? ScientificNameWithoutAuthor { get; set; }
        public string? ScientificNameAuthorship { get; set; }
        public TaxonomicRank? Genus { get; set; }
        public TaxonomicRank? Family { get; set; }
        public string[]? CommonNames { get; set; }
    }

    public class Model5
    {
        public string? Image { get; set; }
        public string? FileName { get; set; }
        public string? Organ { get; set; }
        public float Score { get; set; }
    }

    public class TaxonomicRank
    {
        public string? ScientificNameWithoutAuthor { get; set; }
        public string? ScientificNameAuthorship { get; set; }
        public string? ScientificName { get; set; }
    }

    public class PlantNetImage
    {
        public string? Organ { get; set; }
        public string? Author { get; set; }
        public string? License { get; set; }
        public PlantNetImageDate? Date { get; set; }
        public string? Citation { get; set; }
        public PlantNetUrl? Url { get; set; }
    }

    public class PlantNetImageDate
    {
        public long? Timestamp { get; set; }
        public string? String { get; set; }
    }

    public class PlantNetUrl
    {
        public string? O {  get; set; }
        public string? M { get; set; }
        public string? S { get; set; }
    }

    public class GBIF
    {
        public string? ID { get; set; }
    }

    public class POWO
    {
        public string? ID { get; set; }
    }

    public class IUCN
    {
        public string? ID { get; set; }
        public string? Category { get; set; }
    }
}
