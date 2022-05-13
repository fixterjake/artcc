using Microsoft.AspNetCore.Authorization;

namespace ZDC.Shared.Extensions;

public static class AuthorizationOptionsExtension
{
    public static void AddWashingtonArtccPolicies(this AuthorizationOptions options)
    {
        options.AddMemberPolicy();
        options.AddEventSignupPolicy();
        options.AddTrainingSignupPolicy();
        options.AddSeniorStaffPolicy();
        options.AddFullStaffPolicy();
        options.AddStaffPolicy();
        options.AddTrainingStaffPolicy();
        options.AddRosterPolicy();
        options.AddInstructorPolicy();
        options.AddAirportPolicy();
        options.AddEventsPolicy();
        options.AddFilesPolicy();
        options.AddWebPolicy();
        options.AddCommentPolicy();
        options.AddFeedbackPolicy();
        options.AddOtsPolicy();
        options.AddTrainingTicketsPolicy();
        options.AddManageTrainingTicketsPolicy();
        options.AddManageRostersPolicy();
    }

    public static void AddMemberPolicy(this AuthorizationOptions options)
    {
        options.AddClaimPolicy("IsMember", "IsMember", $"{true}");
    }

    public static void AddEventSignupPolicy(this AuthorizationOptions options)
    {
        options.AddClaimPolicy("CanSignupEvents", "CanSignupEvents", $"{true}");
    }

    public static void AddTrainingSignupPolicy(this AuthorizationOptions options)
    {
        options.AddClaimPolicy("CanSignupTraining", "CanSignupTraining", $"{true}");
    }

    public static void AddSeniorStaffPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("IsSeniorStaff", new[] { "ATM", "DATM", "TA", "WM" });
    }

    public static void AddFullStaffPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("IsFullStaff", new[] { "ATM", "DATM", "TA", "WM", "EC", "FE" });
    }

    public static void AddStaffPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("IsStaff",
            new[] { "ATM", "DATM", "TA", "ATA", "WM", "AWM", "EC", "AEC", "FE", "AFE" });
    }

    public static void AddTrainingStaffPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("IsTrainingStaff",
            new[] { "TA", "ATA", "WM", "INS", "MTR" });
    }

    public static void AddInstructorPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("IsInstructor",
            new[] { "TA", "ATA", "WM", "INS" });
    }

    public static void AddRosterPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanRoster",
            new[] { "ATM", "DATM", "TA", "ATA", "WM", "AWM", "EC", "AEC", "FE", "AFE", "INS", "MTR" });
    }

    public static void AddAirportPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanAirports",
            new[] { "ATM", "DATM", "TA", "ATA", "WM", "FE", "AFE" });
    }

    public static void AddEventsPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanEvents",
            new[] { "ATM", "DATM", "TA", "ATA", "WM", "EC", "AEC", "EVENTS" });
    }

    public static void AddFilesPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanFiles",
            new[] { "ATM", "DATM", "TA", "ATA", "WM", "FE", "AFE" });
    }

    public static void AddWebPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanWeb",
            new[] { "ATM", "DATM", "TA", "WM", "AWM" });
    }

    public static void AddCommentPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanComment",
            new[] { "ATM", "DATM", "TA", "ATA", "WM", "AWM", "EC", "AEC", "FE", "AFE", "INS", "MTR" });
    }

    public static void AddFeedbackPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanFeedback",
            new[] { "ATM", "DATM", "TA", "ATA", "WM" });
    }

    public static void AddOtsPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanOts",
            new[] { "ATM", "DATM", "TA", "ATA", "WM", "INS" });
    }

    public static void AddTrainingTicketsPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanTrainingTickets",
            new[] { "ATM", "DATM", "TA", "ATA", "WM", "INS", "MTR" });
    }

    public static void AddManageTrainingTicketsPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanManageTrainingTickets",
            new[] { "ATM", "DATM", "TA", "ATA", "WM" });
    }

    public static void AddManageRostersPolicy(this AuthorizationOptions options)
    {
        options.AddRolePolicy("CanManageRoster",
            new[] { "ATM", "DATM", "TA", "ATA", "WM", "AWM", "EC", "AEC", "FE", "AFE", "INS", "MTR" });
    }

    public static void AddRolePolicy(this AuthorizationOptions options, string policyName,
        IEnumerable<string> roles)
    {
        options.AddPolicy(policyName, policy => { policy.RequireRole(roles); });
    }

    public static void AddClaimPolicy(this AuthorizationOptions options, string policyName, string claim,
        string value)
    {
        options.AddPolicy(policyName, policy => { policy.RequireClaim(claim, value); });
    }
}
