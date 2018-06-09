namespace FplBot.Api.Season
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
        
    public partial class FplSeason
    {
        [JsonProperty("new_entries")]
        public NewEntries NewEntries { get; set; }

        [JsonProperty("league")]
        public League League { get; set; }

        [JsonProperty("standings")]
        public NewEntries Standings { get; set; }

        [JsonProperty("update_status")]
        public long UpdateStatus { get; set; }
    }

    public partial class League
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("leagueban_set")]
        public List<object> LeaguebanSet { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("short_name")]
        public object ShortName { get; set; }

        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }

        [JsonProperty("closed")]
        public bool Closed { get; set; }

        [JsonProperty("forum_disabled")]
        public bool ForumDisabled { get; set; }

        [JsonProperty("make_code_public")]
        public bool MakeCodePublic { get; set; }

        [JsonProperty("rank")]
        public object Rank { get; set; }

        [JsonProperty("size")]
        public object Size { get; set; }

        [JsonProperty("league_type")]
        public string LeagueType { get; set; }

        [JsonProperty("_scoring")]
        public string Scoring { get; set; }

        [JsonProperty("reprocess_standings")]
        public bool ReprocessStandings { get; set; }

        [JsonProperty("admin_entry")]
        public long AdminEntry { get; set; }

        [JsonProperty("start_event")]
        public long StartEvent { get; set; }
    }

    public partial class NewEntries
    {
        [JsonProperty("has_next")]
        public bool HasNext { get; set; }

        [JsonProperty("number")]
        public long Number { get; set; }

        [JsonProperty("results")]
        public List<Result> Results { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("entry_name")]
        public string EntryName { get; set; }

        [JsonProperty("event_total")]
        public long EventTotal { get; set; }

        [JsonProperty("player_name")]
        public string PlayerName { get; set; }

        [JsonProperty("movement")]
        public Movement Movement { get; set; }

        [JsonProperty("own_entry")]
        public bool OwnEntry { get; set; }

        [JsonProperty("rank")]
        public long Rank { get; set; }

        [JsonProperty("last_rank")]
        public long LastRank { get; set; }

        [JsonProperty("rank_sort")]
        public long RankSort { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("entry")]
        public long Entry { get; set; }

        [JsonProperty("league")]
        public long League { get; set; }

        [JsonProperty("start_event")]
        public long StartEvent { get; set; }

        [JsonProperty("stop_event")]
        public long StopEvent { get; set; }
    }

    public enum Movement { Down, Same, Up };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new MovementConverter(),
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class MovementConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Movement) || t == typeof(Movement?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "down":
                    return Movement.Down;
                case "same":
                    return Movement.Same;
                case "up":
                    return Movement.Up;
            }
            throw new Exception("Cannot unmarshal type Movement");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (Movement)untypedValue;
            switch (value)
            {
                case Movement.Down:
                    serializer.Serialize(writer, "down"); return;
                case Movement.Same:
                    serializer.Serialize(writer, "same"); return;
                case Movement.Up:
                    serializer.Serialize(writer, "up"); return;
            }
            throw new Exception("Cannot marshal type Movement");
        }
    }
}
