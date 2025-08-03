# MVP-Core vOmegaFinal

**© 1997–2025 Virtual Concepts LLC, All Rights Reserved.**  
**Created & designed by Virtual Concepts LLC for Mechanical Solutions Atlanta.**  
**Platform: Service-Atlanta.com (MVP-Core vOmegaFinal)**

MVP-Core is a robust, scalable web application framework designed to streamline the management of Plumbing, Heating, Air Conditioning, and Water Filtration service requests, SEO metadata, dynamic content, email communication, and customer service flows. Built on ASP.NET Core 8, it prioritizes security, performance, and maintainability.

## Legal Notice

This software and all associated components are the **exclusive intellectual property** of Service Atlanta. No part of this system may be copied, distributed, resold, lent, or disclosed to any unauthorized party. Use is strictly limited to verified users who have completed Service Atlanta's full verification process. **Unauthorized use without written authorization is enforceable by law.**

[View Complete Terms of Use](./Docs/Legal/ServiceAtlanta_TermsOfUse.md)

---

## **Beyond-CTO Build Sweep - Sprint123_15 through Sprint123_30**

### **System Monitoring & Telemetry**
- **TelemetryTraceService**: Comprehensive logging of all user actions, CLI operations, and system events
- **Command Center**: Real-time trace log monitoring at `/admin/command-center`
- **Metrics Dashboard**: DAU, ROI, and performance analytics at `/admin/command-center-metrics`
- **Persona Drift Detection**: Automated alerts when empathy scores fall below 0.85 threshold
- **GitGuard CI**: GitHub operation monitoring with push/pull enforcement

### **Security & Compliance**
- **Geographic Filtering**: IP-based access controls for authorized service areas
- **Session Hijack Detection**: Real-time monitoring and alerting
- **Legal Enforcement**: Complete IP protection with usage monitoring
- **Audit Logging**: Comprehensive trail of all system interactions

### **Version Information**
- **Current Version**: MVP-Core vOmegaFinal
- **Release Date**: January 2025
- **Sprint Coverage**: Sprint123_15 through Sprint123_30
- **Legal Protection**: Enhanced copyright enforcement

[View Project Milestones](./Docs/Milestones/MVP-Core_Milestones.md)

---

## **Features**

- **Service Request Flows**: Dynamic, question-driven service request engines per service type.
- **SEO Management**: Manage SEO metadata dynamically for pages.
- **Content Service**: Handle dynamic and static content delivery efficiently.
- **Email Service**: SMTP-based transactional emails (no third-party dependencies).
- **Secure Authentication and Authorization**: Role- and policy-based access control.
- **Session Management**: Secure and configurable session tracking.
- **CORS Support**: Enable cross-origin requests securely.
- **Patch (AI Assistant)**: In-house LLM trained to help technicians troubleshoot error codes, generate diagnostics, and learn from field feedback.
- **PDF Packet Generator**: Service reports and estimates include branding, mission statement, and diagnostics.

---

## **Getting Started**

### **Prerequisites**

- [.NET 8.0 SDK or higher](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads)
- SMTP email account (e.g., ProtonMail, custom server)

### **Installation**

1. Clone the repository:
    ```bash
    git clone https://github.com/your-repo/mvp-core.git
    ```
2. Navigate to the project directory:
    ```bash
    cd mvp-core
    ```
3. Restore packages:
    ```bash
    dotnet restore
    ```
4. Update `appsettings.json`:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Your-SQL-Server-Connection-String"
      },
      "SMTP": {
        "Host": "smtp.protonmail.ch",
        "Port": 587,
        "Username": "service-atlanta@pm.me",
        "Password": "your-password",
        "UseSSL": true,
        "FromEmail": "ServiceRequests@service-atlanta.com",
        "FromName": "Mechanical Solutions Atlanta (MSA) | Service-Atlanta.com"
      }
    }
    ```
5. Apply database migrations:
    ```bash
    dotnet ef database update
    ```
6. Run the application:
    ```bash
    dotnet run
    ```

---

## **Project Structure**

| Folder | Purpose |
|:-------|:--------|
| `/Data` | Entity Framework Core models and database context. |
| `/Services` | Business logic (EmailService, SEOService, QuestionService, etc.) |
| `/Pages/Services` | Service-specific Razor Pages (Plumbing, Heating, etc.) |
| `/Pages/Shared` | Shared views (ThankYou page, SupportBox partial, etc.) |
| `/Helpers` | Session extensions, utility classes. |
| `/wwwroot` | Static files (CSS, JS, images). |
| `/Pages/Admin/Prompts.cshtml` | Run, test, and trace LLM prompts. |
| `/Pages/Admin/Troubleshooter.cshtml` | AI-assisted technician diagnostic UI. |
| `/Pages/About/WhyAI.cshtml` | Mission statement for internal/external trust. |

---

## **Service Flow Example**

1. Customer starts a service request (e.g., `/Services/Plumbing`).
2. Dynamic question flow adapts based on their answers.
3. Customer submits a service request.
4. Database saves the request with timestamped answers.
5. SMTP sends:
   - Confirmation Email to Customer.
   - Notification Email to Admins.
6. Thank You page shown with final confirmation.

---

## **Security Features**

- **HTTPS Enforcement**: `UseHttpsRedirection()` + HSTS active.
- **Secure Cookies**: HTTP-only, Secure, SameSite.Strict session cookies.
- **Anti-Fraud Features**: Hard server-side session expiration + soft client countdown warnings.
- **Authorization**: Admin dashboard protected by role-based policy.
- **LLM Monitoring**: Every AI prompt is evaluated under PROS standards. Abuse tracking and warning thresholds in place.

---

## **Deployment**

### **Local Server (Windows IIS)**

1. Publish the app:
    ```bash
    dotnet publish -c Release -o ./publish
    ```
2. Deploy `./publish` folder to IIS.
3. Set Application Pool to **No Managed Code** (Kestrel handles runtime).

### **Azure App Service**

```bash
az webapp up --name your-app-name --resource-group your-resource-group
```

---

## **Usage**

### **Sending Email via SMTP**

```csharp
await _emailService.SendServiceRequestConfirmationEmailAsync(customer.Email, serviceRequest);
await _emailService.NotifyAdminOfNewRequest(serviceRequest);
```

### **Saving a Service Request**

```csharp
var newRequest = new ServiceRequest {
    CustomerName = "John Doe",
    Email = "john@example.com",
    ServiceType = "Plumbing",
    Details = "Leaky faucet",
    CreatedAt = DateTime.UtcNow
};

await _dbContext.ServiceRequests.AddAsync(newRequest);
await _dbContext.SaveChangesAsync();
```

---

## **Contributing**

1. Fork the repo.
2. Create a branch:
    ```bash
    git checkout -b feature/my-feature
    ```
3. Commit changes:
    ```bash
    git commit -m "Add my feature"
    ```
4. Push:
    ```bash
    git push origin feature/my-feature
    ```
5. Submit a Pull Request.

---

## **License**

This project is licensed under the **MIT License**.

---

## **Contact**

- **Email**: Admins@service-atlanta.com
- **Website**: [https://service-atlanta.com](https://service-atlanta.com)

---

# ✅ Final Notes

- No more SendGrid dependency — everything 100% SMTP server based.
- Strong security practices.
- Solid separation of concerns (Data / Services / Pages).
- Future-proof structure for expansion.

---

## **Attribution & Creative Legacy**

This platform was developed in collaboration with AI under the creative direction of **Dino**, the original architect of the MechSolATL initiative and creator of Service-Atlanta.com. All core logic, flow design, and architectural choices reflect Dino’s leadership and contribution. AI was used solely to assist — not to originate — and any derivative ideas must reference this foundation.

---

## **AI Attribution Notice**

All LLM-based enhancements (Patch) are built internally using OpenAI APIs and strict governance under PROS standards. Patch is not a replacement for technician insight but an enhancement powered by your own field knowledge.

---

## **Testing Notice**

Test project uses EF Core InMemory + xUnit. See /MVP_Core.Tests/README.md for usage.

---

## **Omega Sweep Automation** 🔄

### **Overview**
Omega Sweep is an integrated automation system that validates empathy metrics, build integrity, and deployment readiness across the MVP-Core platform. It ensures that all changes maintain high empathy scores and technical quality standards.

### **Features**
- **Automated CI/CD Integration**: Triggers on pushes to `master`, `release/*` branches, or changes to `Revitalize/`, `CLI/`, `Tests/` directories
- **Empathy Score Validation**: Tests empathy metrics with configurable thresholds
- **Build Integrity Checks**: Validates compilation and dependency injection
- **CLI Signal Testing**: Ensures CLI-to-Overlay communication
- **Merge Protection**: Blocks merges on failed validations

### **Usage**

#### **Local Development CLI**
```bash
# Run full Omega Sweep validation
./Tools/OmegaSweep/omega-sweep.sh run

# Run empathy tests with custom threshold
./Tools/OmegaSweep/omega-sweep.sh empathy --empathy-score-threshold=80.0

# Generate empathy score report
./Tools/OmegaSweep/omega-sweep.sh score-report

# Test CLI signal (debugging)
./Tools/OmegaSweep/omega-sweep.sh signal-test

# Show all available options
./Tools/OmegaSweep/omega-sweep.sh help
```

#### **Available CLI Options**
- `--empathy-score-threshold=N`: Set minimum empathy score (default: 75.0)
- `--verbose`: Enable detailed output
- `--dry-run`: Show what would be executed without running
- `--test-signal`: Test mode for CI signal validation

#### **Git Hook Integration**
The pre-commit hook automatically validates changes affecting Omega Sweep relevant files:
```bash
# Hook location
.idea/hooks/pre-commit

# Validates before each commit:
# - Build compilation
# - Empathy test results
# - DI conflict detection
```

#### **GitHub Actions Workflow**
The CI workflow (`.github/workflows/omega_sweep.yml`) automatically:
1. Builds the project with error detection
2. Runs empathy-categorized tests
3. Checks for dependency injection conflicts
4. Tests CLI-to-Overlay signal connectivity
5. Generates empathy score differential reports
6. Blocks merge if any validation fails

### **Logging and Monitoring**
- **Main Log**: `Logs/Revitalize_OmegaSweep_Log.md` - Detailed execution logs
- **Run History**: `Logs/OmegaSweep_RunHistory.md` - Historical run tracking
- **Empathy Reports**: Auto-generated score reports with trend analysis

### **Merge Blocking Conditions**
Omega Sweep will block merges if:
- ❌ Any empathy test fails
- ❌ Razor build errors detected  
- ❌ Dependency injection conflicts found
- ❌ CLI-to-Overlay signal test fails

### **Admin Dashboard Integration**
Access empathy analytics through the admin dashboard:
- **Persona Analytics**: View empathy scores by persona type
- **Trend Monitoring**: Track empathy improvements over time
- **Compliance Reports**: Export data for compliance review

---

## Legal Notice

All code, designs, workflows, service structures, and business methods associated with Service-Atlanta.com and Mechanical Solutions Atlanta are protected under a Proprietary License.  
Unauthorized use, duplication, or resale without express permission from Virtual Concepts is strictly prohibited.

[View Protected Proprietary License](./Pages/Legal/License.cshtml)

---

> “We’re building a self-learning technician platform where every job teaches the next. It’s powered by AI, guided by experience, and built on the values that define real pros.”
