using Google.Cloud.BigQuery.V2;
using System;

public class Program
{
    public static void Main(string[] args) {

        string projectId = "micro-verbena-360703";
        string datasetId = "UsTestsQueryTest";

        BigQueryClient client = BigQueryClient.Create(projectId);
        var gcsURI = "gs://cloud-samples-data/bigquery/us-states/us-states.csv";
        var dataset = client.GetDataset(datasetId);
        var schema = new TableSchemaBuilder {
            { "name", BigQueryDbType.String },
            { "post_abbr", BigQueryDbType.String }
        }.Build();
        var destinationTableRef = dataset.GetTableReference(
            tableId: "us_states");
        // Create job configuration
        var jobOptions = new CreateLoadJobOptions()
        {
            // The source format defaults to CSV; line below is optional.
            SourceFormat = FileFormat.Csv,
            SkipLeadingRows = 1
        };
        // Create and run job
        var loadJob = client.CreateLoadJob(
            sourceUri: gcsURI, destination: destinationTableRef,
            schema: schema, options: jobOptions);
        loadJob = loadJob.PollUntilCompleted().ThrowOnAnyError();  // Waits for the job to complete.

        // Display the number of rows uploaded
        BigQueryTable table = client.GetTable(destinationTableRef);
        Console.WriteLine(
            $"Loaded {table.Resource.NumRows} rows to {table.FullyQualifiedId}");
    }
}