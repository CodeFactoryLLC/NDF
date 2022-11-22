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
        /// <summary>
        /// Connection failure to the SQL Environment.
        /// </summary>
        public const int ConnectionFailureNumber = 10060;

        /// <summary>
        /// Connection failure message to return with the managed exception.
        /// </summary>
        public const string ConnectionFailureMessage = "System was not able to connect to the source cannot access data.";

        /// <summary>
        /// Timeout error number when executing a SQL operation.
        /// </summary>
        public const int TimeoutNumber = -2;

        /// <summary>
        /// Time out message to return with the managed exception.
        /// </summary>
        public const string TimeoutMessage = "The system timed out accessing the data.";

        /// <summary>
        /// Authentication failure error number from SQL.
        /// </summary>
        public const int AuthenticationErrorNumber = 18456;

        /// <summary>
        /// Error message to return for the authentication error in the managed exception.
        /// </summary>
        public const string AuthenticationErrorMessage = "Was note able to access the data source due to an internal error";

        /// <summary>
        /// Data fails due to a duplicate entry key number.
        /// </summary>
        public const int DuplicateEntryNumber = 2601;

        /// <summary>
        /// Error message to return if a duplicate copy of the data already exists.
        /// </summary>
        public const string DuplicateEntryMessage = "A duplicate copy of the data was found in storage, the system does not support duplicate copies of data.";

        /// <summary>
        /// Data fails does to a unique constraint violation. 
        /// </summary>
        public const int UniqueKeyConstraintViolationNumber = 2627;

        /// <summary>
        /// Error message if a key constraint has been found in SQL.
        /// </summary>
        public const string KeyConstraintEntryMessage = "A key for the data that is being added was found in storage, the system does not support duplicate copies of key data.";

        /// <summary>
        /// Extension method that will evaluate a <see cref="SqlException"/> and raise the target managed exception that represents the exception.
        /// </summary>
        /// <param name="source">The <see cref="SqlException"/> to evaluate.</param>
        /// <exception cref="AuthenticationException">Raised if the error information is authentication in nature.</exception>
        /// <exception cref="CommunicationException">Raised if the error information is connection related.</exception>
        /// <exception cref="DuplicateException">Raised if duplicate data is being entered, or a key constraint is violated.</exception>
        /// <exception cref="CommunicationTimeoutException">Raised if a timeout occurred talk to SQL.</exception>
        /// <exception cref="DataException">Generic exception that is raised to handle all unknown error levels.</exception>
        public static void ThrowManagedException(this SqlException source)
        {
            switch (source.Number)
            {
                case AuthenticationErrorNumber:

                    throw new AuthenticationException(AuthenticationErrorMessage);
                    break;

                case ConnectionFailureNumber:

                    throw new CommunicationException(ConnectionFailureMessage);
                    break;

                case DuplicateEntryNumber:
                    throw new DuplicateException(DuplicateEntryMessage);
                    break;

                case TimeoutNumber:

                    throw new CommunicationTimeoutException(TimeoutMessage);
                    break;

                case UniqueKeyConstraintViolationNumber:

                    throw new DuplicateException(KeyConstraintEntryMessage);
                    break;

                default:

                    throw new DataException();
                    break;
            }
        }
    }
}
