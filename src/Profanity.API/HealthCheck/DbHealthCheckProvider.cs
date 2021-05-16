using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profanity.API.HealthCheck
{
    public class DbHealthCheckProvider
    {
        public static readonly int TimeoutInS = 1;

        public static HealthCheckResult CheckProfanityTables(string connectionString)
            => HelpCheck(connectionString, TableNames.ProfanityService);

        private static HealthCheckResult HelpCheck(string connectionString, string tablename, string? query = null)
             => RunTaskWithTimeout(
                    (Func<HealthCheckResult>)delegate { return CheckFunction(connectionString, tablename, query); }, TimeoutInS);

        private static HealthCheckResult CheckFunction(string connectionString, string tablename, string? query = null)
        {
            try
            {
                var q = query ?? $"select 1 from {tablename};";
                using (var connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(q, connection);
                    command.Connection.Open();
                    int? result;
                    if (string.IsNullOrWhiteSpace(query))
                    {
                        result = command.ExecuteNonQuery();
                        if (result != -1)
                            return HealthCheckResult.Unhealthy();

                        return HealthCheckResult.Healthy();
                    }
                    result = (int)command.ExecuteScalar();
                    if (result > 0)
                    {
                        return HealthCheckResult.Healthy();
                    }
                    else if (result == 0)
                    {
                        return HealthCheckResult.Degraded("Empty table!");
                    }
                    else
                    {
                        return HealthCheckResult.Unhealthy();
                    }
                }
            }
            catch
            {
                return HealthCheckResult.Unhealthy($"Could not find table {tablename}!");
            }
        }

        private static HealthCheckResult RunTaskWithTimeout(Func<HealthCheckResult> TaskAction, int TimeoutSeconds)
        {
            Task<HealthCheckResult> backgroundTask;
            try
            {
                backgroundTask = Task.Factory.StartNew(TaskAction);
                backgroundTask.Wait(new TimeSpan(0, 0, TimeoutSeconds));
            }
            catch (AggregateException ex)
            {
                // task failed
                var failMessage = ex.Flatten().InnerException?.Message;
                return HealthCheckResult.Unhealthy(failMessage);
            }
            catch (Exception ex)
            {
                // task failed
                var failMessage = ex.Message;
                return HealthCheckResult.Unhealthy(failMessage);
            }

            if (!backgroundTask.IsCompleted)
            {
                // task timed out
                return HealthCheckResult.Degraded("Taking longer time that expected");
            }

            // task succeeded
            return backgroundTask.Result;
        }

        private class TableNames
        {
            public const string ProfanityService = "ProfanityEntities";
        }
    }


}
