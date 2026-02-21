using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ScriptureMemoryLibrary;

namespace DataAccess.Models;

/// <summary>
/// To create a log, pass in: 
/// Username [required], ActionType [required], EntityType [required],
/// EntityId, ContextDescription, JsonMetadata, 
/// SeverityLevel (optional), IsAdminAction (optional)
/// </summary>
public sealed class ActivityLog
{
    public int Id { get; set; } = 0;
    public int? UserId { get; set; }
    public Enums.ActionType ActionType { get; set; }
    public Enums.EntityType EntityType { get; set; } // Entity being acted upon
    public int? EntityId { get; set; }
    public string? ContextDescription { get; set; }
    public string? JsonMetadata { get; set; }
    public Enums.SeverityLevel SeverityLevel { get; set; } = Enums.SeverityLevel.Info;
    public bool IsAdminAction { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ActivityLog() { }

    public ActivityLog(Enums.ActionType actionType, Enums.EntityType entityType)
    {
        ActionType = actionType;
        EntityType = entityType;
    }

    public ActivityLog(int userId, Enums.ActionType actionType, Enums.EntityType entityType)
    {
        UserId = userId;
        ActionType = actionType;
        EntityType = entityType;
    }

    public ActivityLog(Enums.ActionType actionType, Enums.EntityType entityType, int entityId)
    {
        ActionType = actionType;
        EntityType = entityType;
        EntityId = entityId;
    }

    public ActivityLog(int userId, Enums.ActionType actionType, Enums.EntityType entityType, int entityId)
    {
        UserId = userId;
        ActionType = actionType;
        EntityType = entityType;
        EntityId = entityId;
    }

    /// <summary>
    /// To create a log, pass in: 
    /// Username [required], ActionType [required], EntityType [required],
    /// EntityId, ContextDescription, JsonMetadata, 
    /// SeverityLevel (optional), IsAdminAction (optional)
    /// </summary>
    public ActivityLog(
        int? userId,
        Enums.ActionType actionType,
        Enums.EntityType entityType,
        int? entityId,
        string? contextDescription,
        object? jsonMetadata,
        Enums.SeverityLevel severityLevel = Enums.SeverityLevel.Info,
        bool isAdminAction = false)
    {
        UserId = userId;
        ActionType = actionType;
        EntityType = entityType;
        EntityId = entityId;
        ContextDescription = contextDescription;
        if (jsonMetadata is not null)
            JsonMetadata = JsonSerializer.Serialize(jsonMetadata);
        SeverityLevel = severityLevel;
        IsAdminAction = isAdminAction;
    }

    public override string ToString()
    {
        StringBuilder returnString = new();

        if (Id != 0)
            returnString.Append("Id: ").Append(Id).Append("\n");
        if (UserId is not null)
            returnString.Append("UserId: ").Append(UserId).Append("\n");
        returnString.Append("ActionType: ").Append(ActionType).Append("\n");
        returnString.Append("EntityType: ").Append(EntityType).Append("\n");
        if (EntityId is not null)
            returnString.Append("EntityId: ").Append(EntityId).Append("\n");
        if (!string.IsNullOrWhiteSpace(ContextDescription))
            returnString.Append("ContextDescription: ").Append(ContextDescription).Append("\n");
        if (!string.IsNullOrWhiteSpace(JsonMetadata))
            returnString.Append("JsonMetadata: ").Append(JsonMetadata).Append("\n");
        returnString.Append("SeverityLevel: ").Append(SeverityLevel).Append("\n");
        returnString.Append("IsAdminAction: ").Append(IsAdminAction).Append("\n");
        returnString.Append("CreatedAt: ").Append(CreatedAt).Append("\n");

        return returnString.ToString();
    }
}
