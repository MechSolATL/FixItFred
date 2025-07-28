using System;

// ?? Sprint 89.2: ViewModel for diagnostic results summary
public class DiagnosticResultsViewModel
{
    public string TenantName { get; set; }
    public string LicenseStatus { get; set; }
    public string InsuranceStatus { get; set; }
    public string CertStatus { get; set; }
    public string ProBadgeStatus { get; set; }
    public string OverrideEligible { get; set; }
}
