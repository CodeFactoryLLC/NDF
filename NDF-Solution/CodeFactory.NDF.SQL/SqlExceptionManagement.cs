//*****************************************************************************
//* Net Delivery Framework
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************

using Microsoft.Data.SqlClient;

namespace CodeFactory.NDF.SQL
{
    /// <summary>
    /// Extensions helper class that converts standard SQL exception numbers into standard managed exceptions.
    /// </summary>
    public static class SqlExceptionManagement
    {
        /// <summary>Connection failure to the SQL Environment.</summary>
        public const int ConnectionFailureNumber = 10060;

        /// <summary>Connection failure message to return with the managed exception.</summary>
        public const string ConnectionFailureMessage = "System was not able to connect to the source cannot access data.";

        /// <summary>Network path not found or server not reachable error number.</summary>
        public const int NetworkPathNotFoundNumber = 53;

        /// <summary>Network path not found message to return with the managed exception.</summary>
        public const string NetworkPathNotFoundMessage = "The system was not able to locate the data source on the network.";

        /// <summary>Connection forcibly closed by the remote host error number.</summary>
        public const int ConnectionResetNumber = 10054;

        /// <summary>Connection reset message to return with the managed exception.</summary>
        public const string ConnectionResetMessage = "The connection to the data source was unexpectedly closed.";

        /// <summary>Timeout error number when executing a SQL operation.</summary>
        public const int TimeoutNumber = -2;

        /// <summary>Time out message to return with the managed exception.</summary>
        public const string TimeoutMessage = "The system timed out accessing the data.";

        /// <summary>Authentication failure error number from SQL.</summary>
        public const int AuthenticationErrorNumber = 18456;

        /// <summary>Error message to return for the authentication error in the managed exception.</summary>
        public const string AuthenticationErrorMessage = "Was not able to access the data source due to an internal error.";

        /// <summary>Account disabled error number from SQL.</summary>
        public const int AccountDisabledNumber = 18470;

        /// <summary>Error message to return when the SQL account is disabled.</summary>
        public const string AccountDisabledMessage = "Access to the data source has been disabled.";

        /// <summary>Permission denied error number from SQL (object level).</summary>
        public const int PermissionDeniedNumber = 229;

        /// <summary>Error message to return when object-level permission is denied.</summary>
        public const string PermissionDeniedMessage = "Access to the requested data is not permitted.";

        /// <summary>Column permission denied error number from SQL.</summary>
        public const int ColumnPermissionDeniedNumber = 230;

        /// <summary>Error message to return when column-level permission is denied.</summary>
        public const string ColumnPermissionDeniedMessage = "Access to a required data field is not permitted.";

        /// <summary>CREATE permission denied error number from SQL.</summary>
        public const int CreatePermissionDeniedNumber = 262;

        /// <summary>Error message to return when CREATE permission is denied.</summary>
        public const string CreatePermissionDeniedMessage = "The system does not have permission to perform this operation on the data source.";

        /// <summary>Cannot open database error number from SQL.</summary>
        public const int CannotOpenDatabaseNumber = 4060;

        /// <summary>Error message to return when the target database cannot be opened.</summary>
        public const string CannotOpenDatabaseMessage = "The system was unable to open the target data source. Please verify configuration settings.";

        /// <summary>Cannot insert NULL error number from SQL.</summary>
        public const int CannotInsertNullNumber = 515;

        /// <summary>Error message to return when a NULL value is inserted into a non-nullable column.</summary>
        public const string CannotInsertNullMessage = "A required data field was not provided.";

        /// <summary>Foreign key constraint violation error number from SQL.</summary>
        public const int ForeignKeyConstraintNumber = 547;

        /// <summary>Error message to return when a foreign key constraint is violated.</summary>
        public const string ForeignKeyConstraintMessage = "The data references related information that does not exist in the system.";

        /// <summary>String or binary data truncation error number from SQL.</summary>
        public const int DataTruncationNumber = 8152;

        /// <summary>Error message to return when data is too large for the target field.</summary>
        public const string DataTruncationMessage = "A data field value exceeds the maximum allowed length.";

        /// <summary>Deadlock victim error number from SQL.</summary>
        public const int DeadlockNumber = 1205;

        /// <summary>Error message to return when a transaction deadlock is detected.</summary>
        public const string DeadlockMessage = "A data conflict was detected, please try your request again.";

        /// <summary>Data fails due to a duplicate entry key number.</summary>
        public const int DuplicateEntryNumber = 2601;

        /// <summary>Error message to return if a duplicate copy of the data already exists.</summary>
        public const string DuplicateEntryMessage = "A duplicate copy of the data was found in storage, the system does not support duplicate copies of data.";

        /// <summary>Data fails due to a unique constraint violation.</summary>
        public const int UniqueKeyConstraintViolationNumber = 2627;

        /// <summary>Error message if a key constraint has been found in SQL.</summary>
        public const string KeyConstraintEntryMessage = "A key for the data that is being added was found in storage, the system does not support duplicate copies of key data.";

        /// <summary>
        /// Extension method that will evaluate a <see cref="SqlException"/> and raise the target managed exception that represents the exception.
        /// </summary>
        /// <param name="source">The <see cref="SqlException"/> to evaluate.</param>
        /// <exception cref="ValidationException">Raised if <paramref name="source"/> is null.</exception>
        /// <exception cref="AuthenticationException">Raised if the error information is authentication in nature.</exception>
        /// <exception cref="AuthorizationException">Raised if the error information is authorization/permission in nature.</exception>
        /// <exception cref="CommunicationException">Raised if the error information is connection related.</exception>
        /// <exception cref="ConfigurationException">Raised if the error indicates a data source configuration problem.</exception>
        /// <exception cref="DataValidationException">Raised if the error is due to invalid or oversized data.</exception>
        /// <exception cref="DuplicateException">Raised if duplicate data is being entered, or a key constraint is violated.</exception>
        /// <exception cref="CommunicationTimeoutException">Raised if a timeout occurred talking to SQL.</exception>
        /// <exception cref="DataException">Generic exception raised to handle all unknown error levels.</exception>
        public static void ThrowManagedException(this SqlException source)
        {
            if (source == null) throw new ValidationException(nameof(source));

            switch (source.Number)
            {
                case AuthenticationErrorNumber:
                case AccountDisabledNumber:
                    throw new AuthenticationException(AuthenticationErrorMessage);

                case PermissionDeniedNumber:
                case ColumnPermissionDeniedNumber:
                case CreatePermissionDeniedNumber:
                    throw new AuthorizationException(PermissionDeniedMessage);

                case ConnectionFailureNumber:
                case NetworkPathNotFoundNumber:
                case ConnectionResetNumber:
                    throw new CommunicationException(ConnectionFailureMessage);

                case TimeoutNumber:
                    throw new CommunicationTimeoutException(TimeoutMessage);

                case CannotOpenDatabaseNumber:
                    throw new ConfigurationException(CannotOpenDatabaseMessage);

                case DuplicateEntryNumber:
                    throw new DuplicateException(DuplicateEntryMessage);

                case UniqueKeyConstraintViolationNumber:
                    throw new DuplicateException(KeyConstraintEntryMessage);

                case CannotInsertNullNumber:
                case DataTruncationNumber:
                    throw new DataValidationException(CannotInsertNullMessage);

                case ForeignKeyConstraintNumber:
                    throw new ValidationException(ForeignKeyConstraintMessage);

                case DeadlockNumber:
                    throw new DataException(DeadlockMessage, source);

                default:
                    throw new DataException(source.Message, source);
            }
        }
    }
}
