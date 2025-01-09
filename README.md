# How to launch the app

1. Install .NET SDK from [https://dotnet.microsoft.com/en-us/download]().
1. Install Git from [https://git-scm.com/downloads]().
1. Install Microsoft SQL Server Express from [https://git-scm.com/downloads]().
1. Clone this repository by running `git clone https://github.com/JankyKonto/BeerRateApi.git` command.
1. In `appsettings.json` file you have to adjust the following things:
     ```json
       "ConnectionStrings": {
      "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database={{your_db_name}};Trusted_Connection=True;TrustServerCertificate=True"
    },
    "EmailSenderSettings": {
      "Host": "{{your_email_host}}",
      "HostAddress": "{{your_email_address}}"
    },
    "ClientAppSettings": {
        "Address": "{{frontend_app_address}}"
    }
    ```
     Important!!! You have to use Microsoft SQL Server as your database management system.
1. You have to set up an environmental variable called `GOOGLE_APP_PASSWORD` with your email account password.
1. Go to cloned project's directory and run the command: `dotnet run --project BeerRateApi`.
