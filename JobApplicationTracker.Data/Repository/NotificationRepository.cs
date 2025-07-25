// using Dapper;
// using JobApplicationTracker.Data.Interface;
// using System.Data;
// using JobApplicationTracker.Data.DataModels;
// using JobApplicationTracker.Data.Dtos.Responses;
//
// namespace JobApplicationTracker.Data.Repository;
//
// public class NotificationsRepository : INotificationsRepository
// {
//     private readonly IDatabaseConnectionService _connectionService;
//
//     public NotificationsRepository(IDatabaseConnectionService connectionService)
//     {
//         _connectionService = connectionService;
//     }
//
//     public async Task<IEnumerable<NotificationsDataModel>> GetAllNotificationsAsync()
//     {
//         await using var connection = await _connectionService.GetDatabaseConnectionAsync();
//
//         var sql = """
//               SELECT NotificationId,
//                      UserId,
//                      Title,
//                      Message,
//                      NotificationTypeId,
//                      IsRead,
//                      CreatedAt,
//                      LinkUrl 
//               FROM Notifications
//               """;
//
//         return await connection.QueryAsync<NotificationsDataModel>(sql).ConfigureAwait(false);
//     }
//
//     public async Task<NotificationsDataModel?> GetNotificationsByIdAsync(Guid notificationsId) // Changed parameter to Guid
//     {
//         await using var connection = await _connectionService.GetDatabaseConnectionAsync();
//
//         // SQL query to fetch a notification by ID
//         var sql = """
//               SELECT NotificationId,
//                      UserId,
//                      Title,
//                      Message,
//                      NotificationTypeId,
//                      IsRead,
//                      CreatedAt,
//                      LinkUrl 
//               FROM Notifications
//               WHERE NotificationId = @NotificationId
//               """;
//
//         var parameters = new DynamicParameters();
//         parameters.Add("@NotificationId", notificationsId, DbType.Guid); // Changed DbType to Guid
//
//         // Use QueryFirstOrDefaultAsync for single results, as it handles null if not found
//         return await connection.QueryFirstOrDefaultAsync<NotificationsDataModel>(sql, parameters).ConfigureAwait(false);
//     }
//
//     public async Task<ResponseDto> SubmitNotificationsAsync(NotificationsDataModel notificationsDto)
//     {
//         await using var connection = await _connectionService.GetDatabaseConnectionAsync();
//
//         string sql;
//         int affectedRows;
//
//         // Check for Guid.Empty instead of <= 0 for new records
//         if (notificationsDto.NotificationId <= 0)
//         {
//             sql = """
//             INSERT INTO Notifications (UserId, Title, Message, NotificationTypeId, IsRead, CreatedAt, LinkUrl)
//             VALUES (@UserId, @Title, @Message, @NotificationTypeId, @IsRead, @CreatedAt, @LinkUrl);
//             SELECT NotificationId FROM Notifications WHERE NotificationId = SCOPE_IDENTITY(); 
//             """;
//         }
//         else
//         {
//             // Update existing notification
//             sql = """
//             UPDATE Notifications
//             SET
//                 UserId = @UserId,
//                 Title = @Title,
//                 Message = @Message,
//                 NotificationTypeId = @NotificationTypeId,
//                 IsRead = @IsRead,
//                 CreatedAt = @CreatedAt, -- Be careful: updating CreatedAt is unusual for an update. Consider an UpdatedAt column if you need to track last modification.
//                 LinkUrl = @LinkUrl
//             WHERE NotificationId = @NotificationId
//             """;
//         }
//
//         var parameters = new DynamicParameters();
//         parameters.Add("@NotificationId", notificationsDto.NotificationId, DbType.Guid); // Changed DbType
//         parameters.Add("@UserId", notificationsDto.UserId, DbType.Int32);
//         parameters.Add("@Title", notificationsDto.Title, DbType.String);
//         parameters.Add("@Message", notificationsDto.Message, DbType.String);
//         parameters.Add("@NotificationTypeId", notificationsDto.NotificationTypeId, DbType.Int32);
//         parameters.Add("@IsRead", notificationsDto.IsRead, DbType.Boolean);
//         parameters.Add("@CreatedAt", notificationsDto.CreatedAt, DbType.DateTime2); // Use DateTime2 for better precision and alignment with SQL
//         parameters.Add("@LinkUrl", notificationsDto.LinkUrl, DbType.String); // Add LinkUrl parameter
//
//         // Execute the command
//         if (notificationsDto.NotificationId == Guid.Empty)
//         {
//             // For insert, we just execute. If the DB generates the ID,
//             // we won't get it back easily with Dapper's ExecuteAsync.
//             // If you *must* get the generated GUID back, you'd generate it in C#
//             // before the insert and pass it to the DB, or use a specific SQL Server
//             // OUTPUT clause and Dapper's QuerySingleOrDefault.
//             affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
//             // Since NEWID() is used, the ID is generated by the DB. We can't use SCOPE_IDENTITY.
//             // If you want the GUID back, you should generate it in C# and pass it to the insert.
//             // Example: notificationsDto.NotificationId = Guid.NewGuid();
//             // then in SQL: INSERT INTO ... VALUES (@NotificationId, ...)
//             // For now, `affectedRows` will tell you if the insert happened.
//         }
//         else
//         {
//             affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
//         }
//
//         return new ResponseDto
//         {
//             IsSuccess = affectedRows > 0,
//             Message = affectedRows > 0 ? "Notifications submitted successfully." : "Failed to submit notification."
//         };
//     }
//
//     public async Task<ResponseDto> DeleteNotificationsAsync(Guid notificationsId) // Changed parameter to Guid
//     {
//         await using var connection = await _connectionService.GetDatabaseConnectionAsync();
//
//         // SQL query to delete a notification by ID
//         var sql = """DELETE FROM Notifications WHERE NotificationId = @NotificationId""";
//
//         var parameters = new DynamicParameters();
//         parameters.Add("@NotificationId", notificationsId, DbType.Guid); // Changed DbType to Guid
//
//         var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
//
//         if (affectedRows <= 0)
//         {
//             return new ResponseDto
//             {
//                 IsSuccess = false,
//                 Message = "Failed to delete notification."
//             };
//         }
//
//         return new ResponseDto
//         {
//             IsSuccess = true,
//             Message = "Notifications deleted successfully."
//         };
//     }
// }