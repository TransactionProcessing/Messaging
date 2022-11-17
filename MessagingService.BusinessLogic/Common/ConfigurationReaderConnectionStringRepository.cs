using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingService.BusinessLogic.Common
{
    using System.Data.Common;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using MySqlConnector;
    using Shared.General;
    using Shared.Repositories;

    [ExcludeFromCodeCoverage]
    public class ConfigurationReaderConnectionStringRepository : IConnectionStringConfigurationRepository
    {
        #region Methods

        /// <summary>
        /// Creates the connection string.
        /// </summary>
        /// <param name="externalIdentifier">The external identifier.</param>
        /// <param name="connectionStringType">Type of the connection string.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task CreateConnectionString(String externalIdentifier,
                                                 String connectionStringIdentifier,
                                                 String connectionString,
                                                 CancellationToken cancellationToken)
        {
            throw new NotImplementedException("This is only required to complete the interface");
        }

        /// <summary>
        /// Deletes the connection string configuration.
        /// </summary>
        /// <param name="externalIdentifier">The external identifier.</param>
        /// <param name="connectionStringType">Type of the connection string.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task DeleteConnectionStringConfiguration(String externalIdentifier,
                                                              String connectionStringIdentifier,
                                                              CancellationToken cancellationToken)
        {
            throw new NotImplementedException("This is only required to complete the interface");
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="externalIdentifier">The external identifier.</param>
        /// <param name="connectionStringType">Type of the connection string.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<String> GetConnectionString(String externalIdentifier,
                                                      String connectionStringIdentifier,
                                                      CancellationToken cancellationToken)
        {
            String connectionString = string.Empty;
            String databaseName = string.Empty;

            String databaseEngine = ConfigurationReader.GetValue("AppSettings", "DatabaseEngine");

            databaseName = $"{connectionStringIdentifier}{externalIdentifier}";
            connectionString = ConfigurationReader.GetConnectionString(connectionStringIdentifier);

            DbConnectionStringBuilder builder = null;

            if (databaseEngine == "MySql")
            {
                builder = new MySqlConnectionStringBuilder(connectionString)
                          {
                              Database = databaseName
                          };
            }
            else
            {
                // Default to SQL Server
                builder = new SqlConnectionStringBuilder(connectionString)
                          {
                              InitialCatalog = databaseName
                          };
            }

            return builder.ToString();
        }

        #endregion
    }
}
