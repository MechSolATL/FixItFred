# MVP-Core

MVP-Core is a robust, scalable web application framework designed to simplify the management of services such as SEO, content management, email communication, and dynamic question flows. Built on ASP.NET Core, it prioritizes security, performance, and maintainability.

---

## **Features**

- **SEO Management**: Dynamically manage SEO metadata for your pages.
- **Content Service**: Handle dynamic and static content delivery efficiently.
- **Email Service**: Send transactional emails with SendGrid integration.
- **Question Service**: Manage and display dynamic question flows based on user inputs.
- **Secure Authentication and Authorization**: Role and policy-based access control.
- **Session Management**: Secure and configurable session handling.
- **CORS Support**: Enable cross-origin requests for specific domains.

---

## **Getting Started**

### **Prerequisites**

1. [.NET 6 SDK or higher](https://dotnet.microsoft.com/download)
2. [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (or Azure SQL)
3. [SendGrid Account](https://sendgrid.com/) for email services

### **Installation**

1. Clone the repository:
    ```bash
    git clone https://github.com/your-repo/mvp-core.git
    ```
2. Navigate to the project directory:
    ```bash
    cd mvp-core
    ```
3. Restore NuGet packages:
    ```bash
    dotnet restore
    ```
4. Update the `appsettings.json` with your configurations:

    ```json
    "ConnectionStrings": {
        "DefaultConnection": "Your-SQL-Server-Connection-String"
    },
    "SendGrid": {
        "ApiKey": "Your-SendGrid-API-Key"
    }
    ```
5. Apply migrations and seed the database:
    ```bash
    dotnet ef database update
    ```

6. Run the application:
    ```bash
    dotnet run
    ```

---

## **Project Structure**

### **Folders**

- **`Data`**: Contains the Entity Framework Core models and database context.
- **`Services`**: Implements business logic for SEO, email, content, and question management.
- **`Controllers`**: Manages HTTP requests and responses.
- **`Pages`**: Razor pages for the UI.

### **Key Files**

- **`Program.cs`**: Configures middleware, services, and the application pipeline.
- **`appsettings.json`**: Stores configuration data such as connection strings and API keys.
- **`ApplicationDbContext.cs`**: Configures database tables and relationships.

---

## **Usage**

### **SEO Management**
- Dynamically retrieve and manage SEO metadata for pages.
- Example:
    ```csharp
    var seoData = await _seoService.GetSeoData("HomePage");
    ```

### **Email Service**
- Send verification and transactional emails via SendGrid.
- Example:
    ```csharp
    await _emailService.SendVerificationEmailAsync(user.Email, verificationLink);
    ```

### **Dynamic Questions**
- Manage dynamic question flows based on user responses.
- Example:
    ```csharp
    var questions = await _questionService.GetQuestionsByCategory("Plumbing");
    ```

---

## **Security Features**

1. **HTTPS Enforcement**:
    - Configured with `UseHttpsRedirection()` and HSTS.
2. **Secure Cookies**:
    - Session cookies are marked as `HttpOnly`, `Secure`, and `SameSite.Strict`.
3. **Authorization**:
    - Role-based and policy-based authorization implemented for access control.

---

## **Deployment**

### **To IIS**
1. Publish the application:
    ```bash
    dotnet publish -c Release -o ./publish
    ```
2. Deploy the contents of the `./publish` directory to your IIS site.

### **To Azure**
1. Use the Azure CLI to deploy:
    ```bash
    az webapp up --name <your-app-name> --resource-group <your-resource-group>
    ```

---

## **Contributing**

1. Fork the repository.
2. Create a feature branch:
    ```bash
    git checkout -b feature-name
    ```
3. Commit changes:
    ```bash
    git commit -m "Add new feature"
    ```
4. Push to the branch:
    ```bash
    git push origin feature-name
    ```
5. Create a pull request.

---

## **License**

This project is licensed under the MIT License. See the `LICENSE` file for details.

---

## **Contact**

For any inquiries or issues, contact:
- **Email**: Admins@service-atlanta.com
- **Website**: [Mechanical Solutions Atlanta](https://service-atlanta.com)
