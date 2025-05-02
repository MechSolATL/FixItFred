# MVP-Core

MVP-Core is a robust, scalable web application framework designed to streamline the management of Plumbing, Heating, Air Conditioning, and Water Filtration service requests, SEO metadata, dynamic content, email communication, and customer service flows. Built on ASP.NET Core 8, it prioritizes security, performance, and maintainability.

---

## **Features**

- **Service Request Flows**: Dynamic, question-driven service request engines per service type.
- **SEO Management**: Manage SEO metadata dynamically for pages.
- **Content Service**: Handle dynamic and static content delivery efficiently.
- **Email Service**: SMTP-based transactional emails (no third-party dependencies).
- **Secure Authentication and Authorization**: Role- and policy-based access control.
- **Session Management**: Secure and configurable session tracking.
- **CORS Support**: Enable cross-origin requests securely.

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

## Legal Notice

All code, designs, workflows, service structures, and business methods associated with Service-Atlanta.com and Mechanical Solutions Atlanta are protected under a Proprietary License.  
Unauthorized use, duplication, or resale without express permission from Virtual Concepts is strictly prohibited.

[View Protected Proprietary License](./Pages/Legal/License.cshtml)
